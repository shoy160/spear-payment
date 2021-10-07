using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Spear.Payment.Alipay.Domain;
using Spear.Payment.Alipay.Request;
using Spear.AutoMapper;
using Spear.Core;
using Spear.Core.Dependency;
using Spear.Core.Exceptions;
using Spear.Core.Extensions;
using Spear.Gateway.Payment.Filters;
using Spear.Gateway.Payment.ViewModels;
using Spear.Payment.Contracts;
using Spear.Payment.Contracts.Dtos;
using Spear.Payment.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spear.Payment.Payment;

namespace Spear.Gateway.Payment.Controllers
{
    /// <summary> 交易接口 </summary>
    [Route("trade")]
    public class TradeController : DController
    {
        private readonly ITradeContract _tradeContract;
        private readonly IProjectContract _projectContract;
        private readonly ILogger _logger;
        private string PaymentHost => "sites:pay".Config<string>();

        public TradeController(ITradeContract tradeContract, IProjectContract projectContract)
        {
            _tradeContract = tradeContract;
            _projectContract = projectContract;
            _logger = CurrentIocManager.CreateLogger<TradeController>();
        }

        /// <summary> 创建交易 </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<DResult<string>> Create([FromBody] VTradeCreateInput input)
        {
            var result = await CreateTrade(input);
            if (input.Scan)
                return Succ($"{PaymentHost}/#/scan/{result.Id}");
            return Succ($"{PaymentHost}/#/{result.Id}");
        }

        /// <summary> 交易详情 </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}"), ApiExplorerSettings(IgnoreApi = true)]
        public async Task<DResult<VTrade>> Detail(string id)
        {
            var detail = await _tradeContract.DetailAsync(id);
            if (detail == null)
                throw new BusiException("交易不存在");
            var model = detail.MapTo<VTrade>();
            if (string.IsNullOrWhiteSpace(model.RedirectUrl))
            {
                var project = await _projectContract.DetailByIdAsync(detail.ProjectId);
                model.RedirectUrl = project.RedirectUrl;
            }

            return Succ(model);
        }

        /// <summary> 交易支付 </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("{id}"), ApiExplorerSettings(IgnoreApi = true)]
        public async Task<DResult> Payment(string id, [FromBody] VTradePaymentInput input)
        {
            var trade = await _tradeContract.DetailAsync(id);
            if (trade.Status != TradeStatus.WaitPay)
                return Error("支付状态异常");
            var channels = await _projectContract.ChannelsAsync(trade.ProjectId);
            if (!channels.ContainsKey(input.Mode))
                return Error("未开通的支付方式");
            var channel = Channel(input.Mode, input.Type, channels);
            var gateway = input.Mode.CreateGateway(channel.Config);
            if (gateway == null)
                return Error("支付渠道配置异常");
            var openId = string.Empty;
            if (input.Type == PaymentType.Public)
            {
                var platform = await _tradeContract.GetPlatform(id);
                openId = platform?.OpenId;
            }
            var result = gateway.Payment(trade, input.Mode, input.Type, openId);
            await _tradeContract.ChangeModeAsync(id, input.Mode, input.Type.ToString().ToLower());
            if (input.Type == PaymentType.H5)
                return Succ(result);
            var dict = new Dictionary<string, object>();
            dict.FromUrl(result);
            return Succ(dict);
        }

        /// <summary> 多码合一 </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("modes/{id}")]
        public async Task<DResults<VTradePaymentMode>> Modes(string id)
        {
            if (id.Length != 32)
            {
                var result = await _tradeContract.GetPaymentsAsync(id, PaymentType.Scan);
                return Succ(result.Select(t => new VTradePaymentMode
                {
                    Mode = t.Key,
                    Url = t.Value
                }), -1);
            }
            var trade = await _tradeContract.DetailAsync(id);
            if (trade.Status != TradeStatus.WaitPay)
                return Errors<VTradePaymentMode>("支付状态异常");
            var channels = await _projectContract.ChannelsAsync(trade.ProjectId);
            return Succ(channels.Select(t => new VTradePaymentMode
            {
                Mode = t.Key
            }), -1);
        }

        /// <summary> 多码合一 </summary>
        /// <param name="id"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        [HttpGet("scan/{id}")]
        public async Task<DResult<string>> Scan(string id, PaymentMode mode)
        {
            const PaymentType type = PaymentType.Scan;
            string result;
            if (id.Length != 32)
            {
                result = await _tradeContract.GetPaymentAsync(id, mode, type);
                if (!string.IsNullOrWhiteSpace(result))
                    return Succ(result);
                throw new BusiException("支付方式异常");
            }
            var trade = await _tradeContract.DetailAsync(id);
            if (trade.Status != TradeStatus.WaitPay)
                return Error<string>("支付状态异常");
            var channels = await _projectContract.ChannelsAsync(trade.ProjectId);
            if (!channels.ContainsKey(mode))
                return Error<string>("未开通的支付方式");
            var channel = Channel(mode, type, channels);
            var gateway = mode.CreateGateway(channel.Config);
            if (gateway == null)
                return Error<string>("支付渠道配置异常");
            result = gateway.Payment(trade, mode, type);
            await _tradeContract.ChangeModeAsync(id, mode, type.ToString().ToLower());
            return Succ(result);
        }

        /// <summary> 交易验证 </summary>
        /// <param name="tradeNo"></param>
        /// <returns></returns>
        [HttpGet("verify/{tradeNo}")]
        public async Task<DResult> VerifyPaid(string tradeNo)
        {
            var trade = await _tradeContract.DetailByNoAsync(tradeNo);
            if (trade.Status != TradeStatus.WaitPay)
                return Error($"支付状态不匹配,当前状态:{trade.Status.GetText()}");
            var type = string.Concat(trade.Type.Substring(0, 1).ToUpper(), trade.Type.Substring(1));
            var channel =
                await _projectContract.ChannelAsync(trade.ProjectId, trade.Mode, type.CastTo<PaymentType?>(null));
            if (channel == null)
                return Error("未开通的支付方式");
            var gateway = trade.Mode.CreateGateway(channel.Config);
            if (gateway == null)
                return Error("支付渠道配置异常");
            TradePaidInputDto inputDto;
            if (trade.Mode == PaymentMode.Alipay)
            {
                var req = new QueryRequest();
                req.AddGatewayData(new QueryModel { OutTradeNo = tradeNo });
                var resp = gateway.Execute(req);
                if (resp.Code != "10000")
                {
                    return Error($"{resp.Code},{resp.SubCode}-{resp.SubMessage}");
                }

                inputDto = new TradePaidInputDto
                {
                    TradeNo = resp.OutTradeNo,
                    Amount = (long)(resp.TotalAmount * 100),
                    User = resp.BuyerUserId,
                    Account = resp.BuyerLogonId,
                    PaidTime = resp.SendPayDate,
                    OutTradeNo = resp.TradeNo,
                    Mode = PaymentMode.Alipay
                };
            }
            else
            {
                var wechatReq = new Spear.Payment.Wechat.Request.QueryRequest();
                wechatReq.AddGatewayData(new Spear.Payment.Wechat.Domain.QueryModel { OutTradeNo = tradeNo });
                var resp = gateway.Execute(wechatReq);
                if (resp.ReturnCode != "SUCCESS")
                {
                    if (!string.IsNullOrWhiteSpace(resp.ReturnMsg))
                        return Error(resp.ReturnMsg);
                    return Error($"{resp.ErrCode},{resp.ErrCodeDes}");
                }

                if (resp.TradeState != "SUCCESS")
                {
                    return Error($"交易未完成，状态:{resp.TradeStateDesc}");
                }
                var time = resp.TimeEnd;//20180427144321
                time = time.Insert(4, "-").Insert(7, "-").Insert(10, " ").Insert(13, ":").Insert(16, ":");

                inputDto = new TradePaidInputDto
                {
                    TradeNo = resp.OutTradeNo,
                    Amount = resp.TotalAmount,
                    User = resp.OpenId,
                    PaidTime = DateTime.Parse(time),
                    OutTradeNo = resp.TradeNo,
                    Mode = PaymentMode.Wechat
                };
            }

            var dto = _tradeContract.TradePaidAsync(inputDto).GetAwaiter().GetResult();
            await dto.PushNotify();
            return Success;
        }

        /// <summary> 交易退款 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("refund"), Project]
        public async Task<DResult> RefundAsync(VTradeRefundInput input)
        {
            var trade = await _tradeContract.DetailAsync(input.ProjectCode, input.OrderNo);
            if (trade.Status != TradeStatus.Paid)
                return Error($"支付状态不匹配,当前状态:{trade.Status.GetText()}");
            var amount = input.Amount == -1 ? trade.Amount : input.Amount;
            if (amount <= 0 || amount > trade.Amount)
                return Error("退款金额异常");
            var type = string.Concat(trade.Type.Substring(0, 1).ToUpper(), trade.Type.Substring(1));
            var channel =
                await _projectContract.ChannelAsync(trade.ProjectId, trade.Mode, type.CastTo<PaymentType?>(null));
            if (channel == null)
                return Error("未开通的支付方式");
            var gateway = trade.Mode.CreateGateway(channel.Config);
            if (gateway == null)
                return Error("支付渠道配置异常");
            var refundDto = new TradeRefundInputDto
            {
                TradeId = trade.Id,
                RefundNo = string.IsNullOrWhiteSpace(input.RefundNo) ? $"R{trade.TradeNo}" : input.RefundNo,
                Reason = input.Reason
            };
            if (amount > 0) refundDto.Amount = amount;
            if (trade.Mode == PaymentMode.Alipay)
            {
                var alipayReq = new RefundRequest();
                alipayReq.AddGatewayData(new RefundModel
                {
                    OutTradeNo = trade.TradeNo,
                    OutRefundNo = refundDto.RefundNo,
                    RefundAmount = amount / 100D,
                    RefundReason = input.Reason
                });
                var resp = gateway.Execute(alipayReq);
                if (resp.Code != "10000")
                    return Error($"{resp.Code},{resp.SubCode}-{resp.SubMessage}");
                refundDto.OutRefundNo = resp.TradeNo;
            }
            else
            {
                var wechatReq = new Spear.Payment.Wechat.Request.RefundRequest();
                wechatReq.AddGatewayData(new Spear.Payment.Wechat.Domain.RefundModel
                {
                    OutTradeNo = trade.TradeNo,
                    OutRefundNo = refundDto.RefundNo,
                    TotalAmount = (int)trade.Amount,
                    RefundAmount = (int)amount,
                    RefundDesc = input.Reason
                });
                var resp = gateway.Execute(wechatReq);
                if (resp.ReturnCode != "SUCCESS")
                {
                    if (!string.IsNullOrWhiteSpace(resp.ReturnMsg))
                        return Error(resp.ReturnMsg);
                    return Error($"{resp.ErrCode},{resp.ErrCodeDes}");
                }

                refundDto.OutRefundNo = resp.RefundNo;
            }

            var result = await _tradeContract.RefundAsync(refundDto);
            if (result > 0)
            {
                return Success;
            }
            return Error("退款失败");
        }
    }
}

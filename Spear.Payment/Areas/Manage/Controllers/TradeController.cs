using Spear.AutoMapper;
using Spear.Core;
using Spear.Core.Dependency;
using Spear.Core.EventBus;
using Spear.Core.Extensions;
using Spear.Gateway.Payment.Areas.Manage.Models;
using Spear.Payment.Contracts;
using Spear.Payment.Contracts.Dtos;
using Spear.Payment.Contracts.Enums;
using Microsoft.AspNetCore.Mvc;
using Spear.Payment.Alipay.Domain;
using Spear.Payment.Alipay.Request;
using System;
using System.Linq;
using System.Threading.Tasks;
using Spear.Payment.Payment;

namespace Spear.Gateway.Payment.Areas.Manage.Controllers
{
    /// <summary>
    /// 交易
    /// </summary>
    [Route("[area]/trade")]
    public class TradeController : DController
    {
        private readonly ITradeContract _contract;
        private readonly IProjectContract _projectContract;

        public TradeController(ITradeContract contract, IProjectContract projectContract)
        {
            _contract = contract;
            _projectContract = projectContract;
        }

        /// <summary> 交易列表 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<DResults<VTrade>> TradeList([FromBody] TradeSearchInputDto input)
        {
            input.ProjectId = Client.ProjectId;
            var data = await _contract.QueryAsync(input);
            var result = data.MapPagedList<VTrade, TradeDto>();
            if (result.Total == 0)
                return DResult.Succ(result);
            var projectIds = result.List.Select(i => i.ProjectId).ToList();
            var projects = await _projectContract.QueryByIdsAsync(projectIds);
            result.List.Foreach(item =>
            {
                var project = projects.FirstOrDefault(t => t.Id == item.ProjectId);
                if (project == null) return;
                item.ProjectName = project.Name;
                item.Notify = !string.IsNullOrWhiteSpace(project.NotifyUrl);
            });
            return DResult.Succ(result);
        }

        /// <summary> 交易异步通知 </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("notify/{id}")]
        public async Task<DResult> Notify(string id)
        {
            var trade = await _contract.DetailAsync(id);
            if (trade.Status != TradeStatus.Paid && trade.Status != TradeStatus.Refund)
                return Error("交易未成功，不能发起异步通知");
            var project = await _projectContract.DetailByIdAsync(trade.ProjectId);
            if (string.IsNullOrWhiteSpace(project.NotifyUrl))
                return Error("交易相关项目未配置异步通知地址");
            await trade.TradeNotify(project.NotifyUrl, project.Secret);
            return Success;
        }

        /// <summary> 通知列表 </summary>
        /// <param name="page">分页页码</param>
        /// <param name="size">单页数量</param>
        /// <param name="type">通知类型</param>
        /// <param name="tradeNo">交易单号</param>
        /// <returns></returns>
        [HttpGet("notifys")]
        public async Task<DResults<VTradeNotify>> NotifyList(int page, int size,
            NotifyType? type, string tradeNo)
        {
            var data = await _contract.NotifyListAsync(page, size, type, tradeNo, Client.ProjectId);
            var result = data.MapPagedList<VTradeNotify, TradeNotifyDto>();
            return DResult.Succ(result);
        }

        /// <summary> 交易验证 </summary>
        /// <param name="tradeNo"></param>
        /// <returns></returns>
        [HttpGet("verify/{tradeNo}")]
        public async Task<DResult> VerifyPaid(string tradeNo)
        {
            var trade = await _contract.DetailByNoAsync(tradeNo);
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
                if (!string.IsNullOrWhiteSpace(resp.ErrCode))
                {
                    return Error($"{resp.ErrCode},{resp.ErrCodeDes}");
                }
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

            var dto = await _contract.TradePaidAsync(inputDto);
            await dto.PushNotify();
            return Success;
        }

        /// <summary> 交易退款 </summary>
        /// <param name="tradeNo"></param>
        /// <param name="amount"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        [HttpPut("refund/{tradeNo}")]
        public async Task<DResult> RefundAsync(string tradeNo, long amount = -1, string reason = null)
        {
            var trade = await _contract.DetailByNoAsync(tradeNo);
            if (trade.Status != TradeStatus.Paid)
                return Error($"支付状态不匹配,当前状态:{trade.Status.GetText()}");
            amount = amount == -1 ? trade.Amount : amount;
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
                RefundNo = $"R{tradeNo}",
                Reason = reason
            };
            if (amount > 0) refundDto.Amount = amount;
            if (trade.Mode == PaymentMode.Alipay)
            {
                var alipayReq = new RefundRequest();
                alipayReq.AddGatewayData(new RefundModel
                {
                    OutTradeNo = tradeNo,
                    OutRefundNo = refundDto.RefundNo,
                    RefundAmount = amount / 100D,
                    RefundReason = reason
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
                    OutTradeNo = tradeNo,
                    OutRefundNo = refundDto.RefundNo,
                    TotalAmount = (int)trade.Amount,
                    RefundAmount = (int)amount,
                    RefundDesc = reason
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

            var result = await _contract.RefundAsync(refundDto);
            if (result > 0)
            {
                return Success;
            }
            return Error("退款失败");
        }
    }
}

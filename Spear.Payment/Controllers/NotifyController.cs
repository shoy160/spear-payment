using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Spear.Core.Dependency;
using Spear.Core.Extensions;
using Spear.Gateway.Payment.Controllers;
using Spear.Payment.Alipay;
using Spear.Payment.Contracts;
using Spear.Payment.Contracts.Dtos;
using Spear.Payment.Contracts.Enums;
using Spear.Payment.Core.Events;
using Spear.Payment.Core.Exceptions;
using Spear.Payment.Core.Gateways;
using Spear.Payment.Core.Notify;
using Spear.Payment.Payment;
using System;
using System.Threading.Tasks;
using NotifyType = Spear.Payment.Core.Notify.NotifyType;

namespace Spear.Payment.Controllers
{
    /// <summary> 支付回调 </summary>
    [Route("notify"), ApiExplorerSettings(IgnoreApi = true)]
    public class NotifyController : DController
    {
        private readonly ITradeContract _tradeContract;
        private readonly IChannelContract _channelContract;
        private readonly ILogger _logger;
        private string _redirectUrl;

        public NotifyController(ITradeContract tradeContract, IChannelContract channelContract)
        {
            _logger = CurrentIocManager.CreateLogger<NotifyController>();
            _tradeContract = tradeContract;
            _channelContract = channelContract;
        }

        private BaseGateway GetGateway(string appId)
        {
            if (string.IsNullOrWhiteSpace(appId))
                return null;
            var channel = _channelContract.DetailByAppIdAsync(appId).SyncRun();
            return channel?.Mode.CreateGateway(channel.Config);
        }

        /// <summary> 回调 </summary>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public async Task Index()
        {
            var notifyDto = new TradeNotifyInputDto
            {
                Type = Contracts.Enums.NotifyType.Receive,
                Url = CurrentIocManager.ContextWrap.RawUrl,
                Content = string.Empty,
                Result = "success"
            };
            try
            {
                // 订阅支付通知事件
                var notify = new Notify(GetGateway);
                notify.PaySucceed += Notify_PaySucceed;
                notify.RefundSucceed += Notify_RefundSucceed;
                notify.CancelSucceed += Notify_CancelSucceed;
                notify.UnknownNotify += Notify_UnknownNotify;
                notify.UnknownGateway += Notify_UnknownGateway;
                // 接收并处理支付通知
                var result = await notify.ReceivedAsync();
                notifyDto.Result = result.Status ? "接收成功" : $"{result.Code},{result.Message}";

                if (!string.IsNullOrWhiteSpace(_redirectUrl))
                {
                    Response.Redirect(_redirectUrl);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                notifyDto.Content = ex.Message;
                throw;
            }
            finally
            {
                if (HttpContext.Items.TryGetValue("request_data", out var data) && data is string content)
                    notifyDto.Content = content;
                if (HttpContext.Items.TryGetValue("tradeId", out var value) && value is string tradeId)
                    notifyDto.TradeId = tradeId;
                await _tradeContract.NotifyAsync(notifyDto);
            }
        }

        private async Task<bool> Notify_PaySucceed(object sender, PaySucceedEventArgs e)
        {
            // 支付成功时时的处理代码
            /* 建议添加以下校验。
             * 1、需要验证该通知数据中的OutTradeNo是否为商户系统中创建的订单号，
             * 2、判断Amount是否确实为该订单的实际金额（即商户订单创建时的金额），
             */

            var inputDto = new TradePaidInputDto();
            if (e.GatewayType == typeof(AlipayGateway))
            {
                var resp = (Spear.Payment.Alipay.Response.NotifyResponse)e.NotifyResponse;

                inputDto.TradeNo = resp.OutTradeNo;
                inputDto.Amount = (long)(resp.TotalAmount * 100);
                inputDto.User = resp.BuyerId;
                inputDto.Account = resp.BuyerLogonId;
                inputDto.PaidTime = resp.GmtPayment;
                inputDto.OutTradeNo = resp.TradeNo;
                inputDto.Mode = PaymentMode.Alipay;
            }
            else if (e.GatewayType == typeof(Spear.Payment.Wechat.WechatGateway))
            {
                var resp = (Spear.Payment.Wechat.Response.NotifyResponse)e.NotifyResponse;
                inputDto.TradeNo = resp.OutTradeNo;
                inputDto.Amount = (long)resp.TotalAmount;
                inputDto.User = resp.OpenId;
                var time = resp.TimeEnd;//20180427144321
                time = time.Insert(4, "-").Insert(7, "-").Insert(10, " ").Insert(13, ":").Insert(16, ":");
                inputDto.PaidTime = DateTime.Parse(time);
                inputDto.OutTradeNo = resp.TradeNo;
                inputDto.Mode = PaymentMode.Wechat;
            }
            var dto = await _tradeContract.TradePaidAsync(inputDto);
            HttpContext.Items.Add("tradeId", dto.Id);
            await dto.PushNotify();
            if (e.NotifyType == NotifyType.Sync)
            {
                _redirectUrl = dto.RedirectUrl;
            }

            //处理成功返回true
            return true;
        }

        private async Task<bool> Notify_RefundSucceed(object arg1, RefundSucceedEventArgs e)
        {
            // 订单退款时的处理代码
            TradeDto tradeDto = null;
            var dto = new TradeRefundInputDto();
            if (e.GatewayType == typeof(AlipayGateway))
            {
                var resp = (Spear.Payment.Alipay.Response.NotifyResponse)e.NotifyResponse;
                tradeDto = await _tradeContract.DetailByNoAsync(resp.OutTradeNo);
                dto.OutRefundNo = resp.TradeNo;
                dto.RefundTime = resp.GmtRefund;
                dto.Amount = (long)(resp.TotalAmount * 100);
            }
            else if (e.GatewayType == typeof(Spear.Payment.Wechat.WechatGateway))
            {
                var resp = (Spear.Payment.Wechat.Response.NotifyResponse)e.NotifyResponse;
                tradeDto = await _tradeContract.DetailByNoAsync(resp.OutTradeNo);
                dto.OutRefundNo = resp.RefundNo;
                dto.RefundTime = resp.SuccessTime;
                dto.Amount = resp.RefundAmount;
            }
            if (tradeDto != null)
            {
                dto.TradeId = tradeDto?.Id;
                var result = await _tradeContract.RefundSuccessAsync(dto);
                if (result > 0)
                {
                    tradeDto = await _tradeContract.DetailAsync(tradeDto.Id);
                    await tradeDto.PushNotify();
                    return true;
                }
            }
            return false;
        }

        private async Task<bool> Notify_CancelSucceed(object arg1, CancelSucceedEventArgs arg2)
        {
            // 订单撤销时的处理代码
            return await Task.FromResult(true);
        }

        private async Task<bool> Notify_UnknownNotify(object sender, UnKnownNotifyEventArgs e)
        {
            return await Task.FromResult(true);
        }

        private void Notify_UnknownGateway(object sender, UnknownGatewayEventArgs e)
        {
            // 无法识别支付网关时的处理代码
            throw new GatewayException("无法识别的支付网关");
        }
    }
}

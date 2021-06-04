using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PaySharp.Alipay;
using PaySharp.Core;
using PaySharp.Wechatpay;
using Spear.Core.Dependency;
using Spear.Core.EventBus;
using Spear.Core.Exceptions;
using Spear.Core.Extensions;
using Spear.Core.Helper;
using Spear.Core.Helper.Http;
using Spear.Core.Serialize;
using Spear.Core.Timing;
using Spear.Gateway.Payment.ViewModels;
using Spear.Payment.Contracts;
using Spear.Payment.Contracts.Dtos;
using Spear.Payment.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Spear.Gateway.Payment.Payment
{
    internal static class PaymentExtensions
    {
        private const string DefaultIp = "127.0.0.1";
        private static readonly ILogger Logger = CurrentIocManager.CreateLogger(typeof(PaymentExtensions));

        /// <summary> 客户端IP </summary>
        public static string RemoteIp(this HttpContext httpContext)
        {
            if (httpContext == null)
                return DefaultIp;
            var sb = new StringBuilder();
            sb.AppendLine();
            foreach (var header in httpContext.Request.Headers)
            {
                sb.AppendLine($"{header.Key}:{header.Value}");
            }

            Logger.LogDebug(sb.ToString());

            string GetIpFromHeader(string key)
            {

                if (!httpContext.Request.Headers.TryGetValue(key, out var ips))
                    return string.Empty;
                foreach (var ip in ips)
                {
                    if (RegexHelper.IsIp(ip))
                        return ip;
                }

                return string.Empty;
            }

            //获取代理IP
            var userHostAddress = GetIpFromHeader("X-Real-IP");
            if (!string.IsNullOrWhiteSpace(userHostAddress))
                return userHostAddress;
            userHostAddress = GetIpFromHeader("X-Forwarded-For");
            if (!string.IsNullOrWhiteSpace(userHostAddress))
                return userHostAddress;
            userHostAddress = GetIpFromHeader("HTTP_X_FORWARDED_FOR");
            if (!string.IsNullOrWhiteSpace(userHostAddress))
                return userHostAddress;
            userHostAddress = GetIpFromHeader("REMOTE_ADDR");
            if (!string.IsNullOrWhiteSpace(userHostAddress))
                return userHostAddress;

            userHostAddress = httpContext.Connection.RemoteIpAddress.ToString();
            return RegexHelper.IsIp(userHostAddress) ? userHostAddress : DefaultIp;
        }

        /// <summary> 创建支付网关 </summary>
        /// <param name="mode"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static BaseGateway CreateGateway(this PaymentMode mode, object config)
        {
            try
            {
                switch (mode)
                {
                    case PaymentMode.Alipay:
                        var alipayMerchant =
                            JsonHelper.Json<PaySharp.Alipay.Merchant>(config.ToString());
                        return new AlipayGateway(alipayMerchant);
                    case PaymentMode.Wechat:
                        var wechatpayMerchant =
                            JsonHelper.Json<PaySharp.Wechatpay.Merchant>(config.ToString());
                        return new WechatpayGateway(wechatpayMerchant);
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, ex.Message);
            }
            return null;
        }

        private static void Check(this PaySharp.Wechatpay.Response.BaseResponse resp)
        {
            if (resp.ReturnCode == "FAIL")
                throw new BusiException(resp.ReturnMsg);
            if (resp.ResultCode == "FAIL")
                throw new BusiException(resp.ErrCodeDes);
        }

        private static void Check(this PaySharp.Alipay.Response.BaseResponse resp)
        {
            if (resp.Code != "10000")
                throw new BusiException($"{resp.SubCode}:{resp.SubMessage}");
        }


        /// <summary> 统一支付入口 </summary>
        /// <param name="gateway"></param>
        /// <param name="trade"></param>
        /// <param name="mode"></param>
        /// <param name="type"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public static string Payment(this IGateway gateway, TradeDto trade, PaymentMode mode, PaymentType type, string openId = null)
        {
            switch (mode)
            {
                case PaymentMode.Alipay:
                    switch (type)
                    {
                        case PaymentType.App:
                            var appPayRequest = new PaySharp.Alipay.Request.AppPayRequest();
                            appPayRequest.AddGatewayData(new PaySharp.Alipay.Domain.AppPayModel
                            {
                                OutTradeNo = trade.TradeNo,
                                TotalAmount = trade.Amount / 100D,
                                Subject = trade.Title,
                                Body = trade.Body
                            });
                            appPayRequest.ReturnUrl = trade.RedirectUrl;
                            var appPayResponse = gateway.Execute(appPayRequest);
                            return appPayResponse.OrderInfo;
                        case PaymentType.H5:
                            var request = new PaySharp.Alipay.Request.WapPayRequest();
                            request.AddGatewayData(new PaySharp.Alipay.Domain.WapPayModel
                            {
                                OutTradeNo = trade.TradeNo,
                                TotalAmount = trade.Amount / 100D,
                                Subject = trade.Title,
                                Body = trade.Body,
                                QuitUrl = trade.RedirectUrl
                            });
                            request.ReturnUrl = trade.RedirectUrl;
                            var response = gateway.Execute(request);
                            return response.Url;
                        case PaymentType.Scan:
                            var scanPayRequest = new PaySharp.Alipay.Request.ScanPayRequest();
                            scanPayRequest.AddGatewayData(new PaySharp.Alipay.Domain.ScanPayModel
                            {
                                OutTradeNo = trade.TradeNo,
                                TotalAmount = trade.Amount / 100D,
                                Subject = trade.Title,
                                Body = trade.Body
                            });

                            var scanPayResponse = gateway.Execute(scanPayRequest);
                            scanPayResponse.Check();
                            return scanPayResponse.QrCode;
                    }

                    break;
                case PaymentMode.Wechat:
                    switch (type)
                    {
                        case PaymentType.Public:
                            var publicPayRequest = new PaySharp.Wechatpay.Request.PublicPayRequest();
                            publicPayRequest.AddGatewayData(new PaySharp.Wechatpay.Domain.PublicPayModel
                            {
                                OutTradeNo = trade.TradeNo,
                                TotalAmount = (int)trade.Amount,
                                Body = trade.Title,
                                OpenId = openId
                            });
                            publicPayRequest.ReturnUrl = trade.RedirectUrl;
                            var publicPayResponse = gateway.Execute(publicPayRequest);
                            publicPayResponse.Check();
                            return publicPayResponse.OrderInfo;
                        case PaymentType.H5:
                            var request = new PaySharp.Wechatpay.Request.WapPayRequest();
                            var clientIp = CurrentIocManager.ContextWrap.RemoteIpAddress;
                            Logger.LogInformation($"[h5]client-ip:{clientIp}");
                            var sceneInfo = new SceneInfo(trade.Title, trade.RedirectUrl);
                            request.AddGatewayData(new PaySharp.Wechatpay.Domain.WapPayModel
                            {
                                OutTradeNo = trade.TradeNo,
                                TotalAmount = (int)trade.Amount,
                                Body = trade.Title,
                                SceneInfo = JsonHelper.ToJson(sceneInfo),
                                SpbillCreateIp = clientIp
                            });
                            request.ReturnUrl = trade.RedirectUrl;
                            var response = gateway.Execute(request);
                            response.Check();
                            var url = response.MwebUrl;
                            if (!string.IsNullOrWhiteSpace(trade.RedirectUrl))
                                url = CurrentIocManager.Context.SetQuery("redirect_url", trade.RedirectUrl, url);
                            return url;
                        case PaymentType.Scan:
                            var scanPayRequest = new PaySharp.Wechatpay.Request.ScanPayRequest();
                            scanPayRequest.AddGatewayData(new PaySharp.Wechatpay.Domain.ScanPayModel
                            {
                                OutTradeNo = trade.TradeNo,
                                TotalAmount = (int)trade.Amount,
                                Body = trade.Title
                            });

                            var scanPayResponse = gateway.Execute(scanPayRequest);
                            scanPayResponse.Check();
                            return scanPayResponse.CodeUrl;
                    }
                    break;
            }

            throw new BusiException("支付方式暂不支持");
        }

        private static void Sign(IDictionary<string, object> dict, string privateKey)
        {
            dict.Remove("sign");
            if (string.IsNullOrWhiteSpace(privateKey))
            {
                dict.AddOrUpdate("sign", "nokey");
                return;
            }

            var timestamp = Clock.Now.ToMillisecondsTimestamp();
            var sign = dict.Sign(privateKey, timestamp);
            dict.AddOrUpdate("sign", sign);
        }

        /// <summary> 交易异步通知 </summary>
        /// <param name="trade"></param>
        /// <param name="url"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static async Task TradeNotify(this TradeDto trade, string url, string secret)
        {
            var dto = new TradeNotifyInputDto
            {
                TradeId = trade.Id,
                Type = NotifyType.Send,
                Url = url
            };
            try
            {
                var type = trade.Type.Substring(0, 1).ToUpper() + trade.Type.Substring(1);
                var dict = new Dictionary<string, object>
                {
                    {"orderNo", trade.OrderNo},
                    {"amount", trade.Amount},
                    {"mode", trade.Mode},
                    {"type", type.CastTo<PaymentType>()},
                    {"status", trade.Status},
                    {"extend", trade.Extend},
                    {"tradeNo", trade.OutTradeNo}
                };
                Sign(dict, secret);
                dto.Content = JsonHelper.ToJson(dict);

                var resp = await HttpHelper.Instance.PostAsync(url, dict);
                var content = await resp.Content.ReadAsStringAsync();
                var status = resp.IsSuccessStatusCode && string.Equals(content, "success", StringComparison.CurrentCultureIgnoreCase);
                dto.Result = status ? "通知成功" : content;
                if (!status)
                    throw new Exception($"支付回调异常:{content}");
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(dto.Content))
                {
                    var contract = CurrentIocManager.Resolve<ITradeContract>();
                    await contract.NotifyAsync(dto);
                }
            }
        }

        /// <summary>
        /// 推送通知事件
        /// </summary>
        /// <param name="trade"></param>
        /// <returns></returns>
        public static async Task PushNotify(this TradeDto trade)
        {
            if (string.IsNullOrWhiteSpace(trade?.ProjectId))
                return;
            const string notifyKey = "payment_notify";
            await CurrentIocManager.Resolve<IEventBus>().Publish(notifyKey, trade);
        }

        /// <summary>
        /// 推送通知事件
        /// </summary>
        /// <param name="tradeNo"></param>
        /// <returns></returns>
        public static async Task PushTradeNotify(this string tradeNo)
        {
            var trade = await CurrentIocManager.Resolve<ITradeContract>().DetailByNoAsync(tradeNo);
            await trade.PushNotify();
        }

    }
}

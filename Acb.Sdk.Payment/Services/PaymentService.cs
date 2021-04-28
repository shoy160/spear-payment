using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Acb.Sdk.Payment.Enums;
using Acb.Sdk.Payment.Models;
using Acb.SdkCore;
using Newtonsoft.Json;

#if NETSTANDARD2_0
using Microsoft.AspNetCore.Http;
#endif

namespace Acb.Sdk.Payment.Services
{
    public class PaymentService : SdkService, IPaymentContract
    {

#if NETSTANDARD2_0
        public PaymentService(PaymentOptions config, IHttpContextAccessor contextAccessor)
            : base(config, contextAccessor)
        {
        }
#else
        public PaymentService(PaymentOptions config) : base(config)
        {
        }
#endif

        /// <summary> 创建收银台 </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        public async Task<SdkResult<string>> CreateCashier(CashierInput payment)
        {
            var dict = new Dictionary<string, object>
            {
                {"OrderNo", payment.OrderNo},
                {"Amount", payment.Amount},
                {"Title", payment.Title},
                {"Body", payment.Body},
                {"Extend", payment.Extend},
                {"ProjectCode", Config.Code},
                {"RedirectUrl", payment.RedirectUrl},
                {"Scan",payment.Scan }
            };
            dict.Sign(Config.Secret);
            var resp = await HttpHelper.PostAsync("trade", dict);
            var result = resp.Get<SdkResult<string>>();
            return result;
        }

        /// <summary> 创建支付 </summary>
        /// <param name="mode"></param>
        /// <param name="type"></param>
        /// <param name="paymentParams"></param>
        /// <returns></returns>
        public async Task<SdkResult<string>> Pay(PaymentMode mode, PaymentType type, PayInput paymentParams)
        {
            var dict = new Dictionary<string, object>
            {
                {"OrderNo", paymentParams.OrderNo},
                {"Amount", paymentParams.Amount},
                {"Title", paymentParams.Title},
                {"Body", paymentParams.Body},
                {"Extend", paymentParams.Extend},
                {"ProjectCode", Config.Code},
                {"RedirectUrl", paymentParams.RedirectUrl}
            };
            if (mode == PaymentMode.Wechat)
            {
                switch (type)
                {
                    case PaymentType.Applet:
                    case PaymentType.Public:
                        dict.Add("OpenId", paymentParams.OpenId);
                        break;
                    case PaymentType.H5:
                        var sceneInfo = JsonConvert.SerializeObject(paymentParams.SceneInfo);
                        dict.Add("SceneInfo", sceneInfo);
                        break;
                }
            }
            dict.Sign(Config.Secret);
            var api = $"{mode}/{type}".ToLower();
            try
            {
                var result = await HttpHelper.GetAsync(api, dict);
                var content = result.Result;
                if (content.StartsWith("{"))
                {
                    return Json<SdkResult<string>>(content);
                }
                else
                {
                    return new SdkResult<string> { Code = 0, Data = content };
                }
            }
            catch (Exception ex)
            {
                if (ex is SdkException sdkEx)
                    return new SdkResult<string> { Code = sdkEx.Code, Message = sdkEx.Message };
                return new SdkResult<string> { Code = -1, Message = "创建支付异常" };
            }
        }

        /// <summary> 回调验证 </summary>
        /// <returns></returns>
        public async Task<NotifyModel> NotifyVerify(NotifyModel model = null)
        {
            if (model == null)
            {
#if NETSTANDARD2_0
                if (Current == null || !string.Equals("post", Current.Request.Method, StringComparison.CurrentCultureIgnoreCase))
                    throw new SdkException("异步通知方式异常", 10000);
#else
            if (Current == null || !string.Equals("post", Current.Request.HttpMethod, StringComparison.CurrentCultureIgnoreCase))
                throw new SdkException("异步通知方式异常", 10000);

#endif
                var body = Body;
                if (body.CanSeek)
                    body.Seek(0, SeekOrigin.Begin);

                using (var reader = new StreamReader(body))
                {
                    var html = await reader.ReadToEndAsync();
                    model = Json<NotifyModel>(html);
                }
            }
            if (model == null)
                throw new Exception("回调参数异常");
            var dict = new Dictionary<string, object>
            {
                {"orderNo", model.OrderNo},
                {"amount", model.Amount},
                {"mode", model.Mode},
                {"type", model.Type},
                {"status",model.Status },
                {"extend", model.Extend},
                {"tradeNo" ,model.TradeNo}
            };
            var checkSign = dict.VerifySign(Config.Secret, model.Sign);
            if (checkSign) return model;
            throw new SdkException("签名验证失败");
        }
    }
}

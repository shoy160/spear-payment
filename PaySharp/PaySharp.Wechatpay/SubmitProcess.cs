using Acb.Core.Logging;
using PaySharp.Core;
using PaySharp.Core.Exceptions;
using PaySharp.Core.Request;
using PaySharp.Core.Response;
using PaySharp.Core.Utils;
using PaySharp.Wechatpay.Domain;
using PaySharp.Wechatpay.Request;
using PaySharp.Wechatpay.Response;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Acb.Core.Exceptions;

namespace PaySharp.Wechatpay
{
    internal static class SubmitProcess
    {
        private static string _gatewayUrl;

        internal static TResponse Execute<TModel, TResponse>(Merchant merchant, Request<TModel, TResponse> request, string gatewayUrl = null) where TResponse : IResponse
        {
            AddMerchant(merchant, request, gatewayUrl);

            var sign = BuildSign(request.GatewayData, merchant.Key, request.GatewayData.GetStringValue("sign_type") == "HMAC-SHA256");
            request.GatewayData.Add("sign", sign);

            X509Certificate2 cert = null;
            if (((BaseRequest<TModel, TResponse>)request).IsUseCert)
            {
                //证书密码默认为您的商户ID
                if (string.IsNullOrWhiteSpace(merchant.SslCertPassword))                
                    merchant.SslCertPassword = merchant.MchId;                
                cert = Util.CreateCert(merchant.SslCertPath, merchant.SslCertPassword);
                if (cert == null)
                    throw new BusiException("商户证书异常，不能执行操作");
                //cert = new X509Certificate2(merchant.SslCertPath, merchant.SslCertPassword, X509KeyStorageFlags.MachineKeySet);
            }

            string result = null;
            Task.Run(async () =>
            {
                result = await HttpUtil
                 .PostAsync(request.RequestUrl, request.GatewayData.ToXml(), cert);
            })
            .GetAwaiter()
            .GetResult();

            BaseResponse baseResponse;
            if (!(request is BillDownloadRequest || request is FundFlowDownloadRequest))
            {
                var gatewayData = new GatewayData();
                gatewayData.FromXml(result);

                baseResponse = (BaseResponse)(object)gatewayData.ToObject<TResponse>(StringCase.Snake);
                baseResponse.Raw = result;
                baseResponse.GatewayData = gatewayData;
                if (baseResponse.ReturnCode == "SUCCESS")
                {
                    sign = gatewayData.GetStringValue("sign");

                    if (!string.IsNullOrEmpty(sign) && !CheckSign(gatewayData, merchant.Key, sign))
                    {
                        throw new GatewayException("签名验证失败");
                    }

                    baseResponse.Sign = sign;
                    baseResponse.Execute(merchant, request);
                }
            }
            else
            {
                baseResponse = (BaseResponse)Activator.CreateInstance(typeof(TResponse));
                baseResponse.Raw = result;
                baseResponse.Execute(merchant, request);
            }

            return (TResponse)(object)baseResponse;
        }

        internal static TResponse AuthExecute<TModel, TResponse>(Merchant merchant, Request<TModel, TResponse> request, string gatewayUrl = null) where TResponse : IResponse
        {
            AddMerchant(merchant, request, gatewayUrl);

            string result = null;
            Task.Run(async () =>
            {
                result = await HttpUtil
                 .GetAsync($"{request.RequestUrl}?{request.GatewayData.ToUrl()}");
            })
            .GetAwaiter()
            .GetResult();

            var gatewayData = new GatewayData();
            gatewayData.FromJson(result);

            OAuthResponse baseResponse = (OAuthResponse)(object)gatewayData.ToObject<TResponse>(StringCase.Snake);
            baseResponse.Raw = result;
            baseResponse.GatewayData = gatewayData;

            return (TResponse)(object)baseResponse;
        }

        private static void AddMerchant<TModel, TResponse>(Merchant merchant, Request<TModel, TResponse> request, string gatewayUrl) where TResponse : IResponse
        {
            if (!string.IsNullOrEmpty(gatewayUrl))
            {
                _gatewayUrl = gatewayUrl;
            }

            if (!request.RequestUrl.StartsWith("http"))
            {
                request.RequestUrl = _gatewayUrl + request.RequestUrl;
            }
            request.GatewayData.Add(merchant, StringCase.Snake);
            //if (typeof(TModel) == typeof(AppletPayModel))
            //{
            //    request.GatewayData["appid"] = merchant.AppletId;
            //}
            ((BaseRequest<TModel, TResponse>)request).Execute(merchant);
        }

        internal static string BuildSign(GatewayData gatewayData, string key, bool isHMACSHA256 = false)
        {
            gatewayData.Remove("sign");

            var data = $"{gatewayData.ToUrl(false)}&key={key}";
            //LogManager.Logger("wechat_sign").Info($"un_sign:{data}");
            return isHMACSHA256 ? EncryptUtil.HmacSha256(data, key) : EncryptUtil.Md5(data);
        }

        internal static bool CheckSign(GatewayData gatewayData, string key, string sign)
        {
            return BuildSign(gatewayData, key) == sign;
        }
    }
}

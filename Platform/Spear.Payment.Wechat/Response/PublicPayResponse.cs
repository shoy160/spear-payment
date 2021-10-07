using Spear.Payment.Core.Gateways;
using Spear.Payment.Core.Request;
using Spear.Payment.Core.Utils;
using System;

namespace Spear.Payment.Wechat.Response
{
    public class PublicPayResponse : BaseResponse
    {
        /// <summary>
        /// 交易类型
        /// </summary>
        public string TradeType { get; set; }

        /// <summary>
        /// 微信生成的预支付回话标识，用于后续接口调用中使用，该值有效期为2小时
        /// </summary>
        public string PrepayId { get; set; }

        /// <summary>
        /// 用于唤起App的订单参数
        /// </summary>
        public string OrderInfo { get; set; }

        internal override void Execute<TModel, TResponse>(Merchant merchant, Request<TModel, TResponse> request)
        {
            if (ResultCode == "SUCCESS")
            {
                var gatewayData = new GatewayData();
                var appId = merchant.AppId;
                //if (typeof(TModel) == typeof(AppletPayModel))
                //    appId = merchant.AppletId;
                gatewayData.Add("appId", appId);
                gatewayData.Add("timeStamp", DateTime.Now.ToTimeStamp());
                gatewayData.Add("nonceStr", Util.GenerateNonceStr());
                gatewayData.Add("package", $"prepay_id={PrepayId}");
                gatewayData.Add("signType", "MD5");
                var sign = SubmitProcess.BuildSign(gatewayData, merchant.Key);
                //string data = $"{gatewayData.ToUrl(false)}&key={merchant.Key}";
                //Console.WriteLine($"un_sign:{data}");
                //string sign = EncryptUtil.Md5(data);
                gatewayData.Add("paySign", sign);
                OrderInfo = gatewayData.ToJson();
            }
        }
    }
}

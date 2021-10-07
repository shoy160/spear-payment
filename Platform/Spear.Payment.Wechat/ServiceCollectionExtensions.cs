using Microsoft.Extensions.Configuration;
using Spear.Payment.Core.Gateways;
using System;

namespace Spear.Payment.Wechat
{
    public static class ServiceCollectionExtensions
    {
        public static IGateways UseWechatpay(this IGateways gateways, Action<Merchant> action)
        {
            if (action != null)
            {
                var merchant = new Merchant();
                action(merchant);
                gateways.Add(new WechatGateway(merchant));
            }

            return gateways;
        }

        public static IGateways UseWechatpay(this IGateways gateways, IConfiguration configuration)
        {
            var merchants = configuration.GetSection("PaySharp:Wechatpays").Get<Merchant[]>();
            if (merchants != null)
            {
                for (int i = 0; i < merchants.Length; i++)
                {
                    var wechatpayGateway = new WechatGateway(merchants[i]);
                    var gatewayUrl = configuration.GetSection($"PaySharp:Wechatpays:{i}:GatewayUrl").Value;
                    if (!string.IsNullOrEmpty(gatewayUrl))
                    {
                        wechatpayGateway.GatewayUrl = gatewayUrl;
                    }

                    gateways.Add(wechatpayGateway);
                }
            }

            return gateways;
        }
    }
}
using Microsoft.Extensions.Configuration;
using Spear.Payment.Core.Gateways;
using System;

namespace Spear.Payment.Wechat
{
    public static class ServiceCollectionExtensions
    {
        public static IGateways UseWechat(this IGateways gateways, Action<Merchant> action)
        {
            if (action != null)
            {
                var merchant = new Merchant();
                action(merchant);
                gateways.Add(new WechatGateway(merchant));
            }

            return gateways;
        }

        public static IGateways UseWechat(this IGateways gateways, IConfiguration configuration)
        {
            var merchants = configuration.GetSection("Payment:Wechatpays").Get<Merchant[]>();
            if (merchants != null)
            {
                for (int i = 0; i < merchants.Length; i++)
                {
                    var wechatpayGateway = new WechatGateway(merchants[i]);
                    var gatewayUrl = configuration.GetSection($"Payment:Wechatpays:{i}:GatewayUrl").Value;
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
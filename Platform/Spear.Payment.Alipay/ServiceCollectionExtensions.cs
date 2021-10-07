using Microsoft.Extensions.Configuration;
using Spear.Payment.Core.Gateways;
using System;

namespace Spear.Payment.Alipay
{
    public static class ServiceCollectionExtensions
    {
        public static IGateways UseAlipay(this IGateways gateways, Action<Merchant> action)
        {
            if (action != null)
            {
                var merchant = new Merchant();
                action(merchant);
                gateways.Add(new AlipayGateway(merchant));
            }

            return gateways;
        }

        public static IGateways UseAlipay(this IGateways gateways, IConfiguration configuration)
        {
            var merchants = configuration.GetSection("PaySharp:Alipays").Get<Merchant[]>();
            if (merchants != null)
            {
                for (int i = 0; i < merchants.Length; i++)
                {
                    var alipayGateway = new AlipayGateway(merchants[i]);
                    var gatewayUrl = configuration.GetSection($"PaySharp:Alipays:{i}:GatewayUrl").Value;
                    if (!string.IsNullOrEmpty(gatewayUrl))
                    {
                        alipayGateway.GatewayUrl = gatewayUrl;
                    }

                    gateways.Add(alipayGateway);
                }
            }

            return gateways;
        }
    }
}

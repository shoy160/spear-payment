using Spear.SdkCore;

namespace Spear.Sdk.Payment
{
    /// <summary> 支付配置 </summary>
    public class PaymentOptions : ISdkConfig
    {
        /// <summary> 支付网关 </summary>
        public string Gateway { get; set; } = "https://pay.i-cbao.com";
        /// <summary> 项目编码 </summary>
        public string Code { get; set; }
        /// <summary> 项目密钥 </summary>
        public string Secret { get; set; }
    }
}

using Spear.Payment.Contracts.Enums;

namespace Spear.Gateway.Payment.ViewModels
{
    /// <summary> 交易支付参数 </summary>
    public class VTradePaymentInput
    {
        /// <summary> 支付方式 </summary>
        public PaymentMode Mode { get; set; }

        /// <summary> 支付类型 </summary>
        public PaymentType Type { get; set; }
    }

    internal class SceneInfo
    {
        public H5Info h5_info { get; set; }

        public SceneInfo(string wapName, string wapUrl)
        {
            h5_info = new H5Info
            {
                wap_name = wapName,
                wap_url = wapUrl
            };
        }
    }

    internal class H5Info
    {
        public string type { get; } = "Wap";
        public string wap_name { get; set; }
        public string wap_url { get; set; }
    }
}

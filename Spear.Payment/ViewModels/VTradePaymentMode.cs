using Spear.Payment.Contracts.Enums;

namespace Spear.Gateway.Payment.ViewModels
{
    public class VTradePaymentMode
    {
        /// <summary> 支付方式 </summary>
        public PaymentMode Mode { get; set; }
        /// <summary> 支付参数/链接 </summary>
        public string Url { get; set; }
    }
}

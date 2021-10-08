using Spear.Payment.Sdk.Enums;

namespace Spear.Payment.Sdk.Models
{
    public class NotifyModel
    {
        /// <summary> 商户订单号 </summary>
        public string OrderNo { get; set; }
        /// <summary> 金额(分) </summary>
        public long Amount { get; set; }
        /// <summary> 扩展信息 </summary>
        public string Extend { get; set; }
        /// <summary> 支付方式 </summary>
        public PaymentMode Mode { get; set; }
        /// <summary> 支付类型 </summary>
        public PaymentType Type { get; set; }
        public TradeStatus Status { get; set; }
        /// <summary> 交易号 </summary>
        public string TradeNo { get; set; }
        /// <summary> 签名 </summary>
        public string Sign { get; set; }
    }
}

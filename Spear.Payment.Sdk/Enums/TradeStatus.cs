using System.ComponentModel;

namespace Spear.Sdk.Payment.Enums
{
    /// <summary> 交易状态 </summary>
    public enum TradeStatus : byte
    {
        /// <summary> 待支付 </summary>
        [Description("待支付")] WaitPay = 0,
        /// <summary> 已支付 </summary>
        [Description("已支付")] Paid = 1,
        /// <summary> 已关闭 </summary>
        [Description("已关闭")] Close = 2,
        /// <summary> 已退款 </summary>
        [Description("已退款")] Refund = 3
    }
}

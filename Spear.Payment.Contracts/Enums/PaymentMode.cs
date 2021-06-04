using System.ComponentModel;

namespace Spear.Payment.Contracts.Enums
{
    /// <summary> 支付方式 </summary>
    public enum PaymentMode : byte
    {
        /// <summary> 支付宝 </summary>
        [Description("支付宝")]
        Alipay = 0,
        /// <summary> 微信 </summary>
        [Description("微信")]
        Wechat = 1
    }
}

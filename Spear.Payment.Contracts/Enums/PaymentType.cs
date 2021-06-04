using System.ComponentModel;

namespace Spear.Payment.Contracts.Enums
{
    /// <summary> 支付类型 </summary>
    public enum PaymentType
    {
        /// <summary> 网页支付 </summary>
        [Description("网页支付")] Web,

        /// <summary> H5支付 </summary>
        [Description("H5支付")] H5,

        /// <summary> App支付 </summary>
        [Description("App支付")] App,

        /// <summary> 公众号支付 </summary>
        [Description("公众号支付")] Public,

        /// <summary> 扫码支付 </summary>
        [Description("扫码支付")] Scan,

        /// <summary> 条码支付 </summary>
        [Description("条码支付")] Barcode,

        /// <summary> 小程序支付 </summary>
        [Description("小程序支付")] Applet
    }
}

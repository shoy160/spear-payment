using System.ComponentModel.DataAnnotations;

namespace Spear.Gateway.Payment.ViewModels
{
    /// <summary> 条码支付参数 </summary>
    public class VBarcodePaymentInput : VPaymentInput
    {
        /// <summary> 支付授权码 </summary>
        [Required(ErrorMessage = "支付授权码不能为空")]
        public string AuthCode { get; set; }
    }
}

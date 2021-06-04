using System.ComponentModel.DataAnnotations;

namespace Spear.Gateway.Payment.ViewModels
{
    /// <summary> 带用户标识的支付参数 </summary>
    public class VUserTagPaymentByInput : VPaymentInput
    {
        /// <summary> 用户标识，此参数为微信用户在商户对应appid下的唯一标识。 </summary>
        [Required(ErrorMessage = "用户标识不能为空")]
        public string OpenId { get; set; }
    }
}

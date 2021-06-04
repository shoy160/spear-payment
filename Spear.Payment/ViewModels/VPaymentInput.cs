using System.ComponentModel.DataAnnotations;

namespace Spear.Gateway.Payment.ViewModels
{
    /// <summary> 基础支付参数 </summary>
    public class VPaymentInput : VPaymentSignature
    {
        /// <summary> 子系统订单号 </summary>
        [Required(ErrorMessage = "订单号不能为空")]
        [StringLength(64, MinimumLength = 6, ErrorMessage = "订单号应为6-64位不重复的字符")]
        public string OrderNo { get; set; }

        /// <summary> 金额(分) </summary>
        [Required(ErrorMessage = "支付金额异常")]
        [Range(1, 100000000, ErrorMessage = "支付金额必须大于1分")]
        public int Amount { get; set; }

        /// <summary> 支付标题 </summary>
        public string Title { get; set; }

        /// <summary> 支付描述 </summary>
        public string Body { get; set; }

        /// <summary> 扩展信息,通知时原样返回 </summary>
        public string Extend { get; set; }

        /// <summary> 跳转链接 </summary>
        public string RedirectUrl { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Acb.Gateway.Payment.ViewModels
{
    /// <summary> 交易查询接口 </summary>
    public class VQueryInput : VPaymentSignature
    {
        /// <summary> 订单号 </summary>
        [Required(ErrorMessage = "请输入订单号")]
        public string OrderNo { get; set; }
    }
}

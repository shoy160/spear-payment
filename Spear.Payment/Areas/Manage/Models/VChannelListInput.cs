using Spear.Payment.Contracts.Enums;
using Spear.WebApi.ViewModels;

namespace Spear.Gateway.Payment.Areas.Manage.Models
{
    /// <summary> 渠道列表参数 </summary>
    public class VChannelListInput : VPageInput
    {
        /// <summary> 搜索关键字(商户ID/渠道名称) </summary>
        public string Keyword { get; set; }
        /// <summary> 支付方式 </summary>
        public PaymentMode? Mode { get; set; }
        /// <summary> 渠道状态 </summary>
        public ChannelStatus? Status { get; set; }
    }
}

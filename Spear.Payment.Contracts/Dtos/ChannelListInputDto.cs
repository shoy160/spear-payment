using Spear.Core.Domain.Dtos;
using Spear.Payment.Contracts.Enums;

namespace Spear.Payment.Contracts.Dtos
{
    public class ChannelListInputDto : PageInputDto
    {
        /// <summary> 搜索关键字(商户ID/渠道名称) </summary>
        public string Keyword { get; set; }
        /// <summary> 支付方式 </summary>
        public PaymentMode? Mode { get; set; }
        /// <summary> 渠道状态 </summary>
        public ChannelStatus? Status { get; set; }
    }
}

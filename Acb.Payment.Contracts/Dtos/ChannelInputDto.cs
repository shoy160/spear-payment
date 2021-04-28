using Acb.Core.Domain.Dtos;
using Acb.Payment.Contracts.Enums;

namespace Acb.Payment.Contracts.Dtos
{
    public class ChannelInputDto : DDto
    {
        public string Name { get; set; }
        public string Remark { get; set; }
        public PaymentMode Mode { get; set; }
        public PaymentType[] Types { get; set; }
        public string AppId { get; set; }
        public object Config { get; set; }
        public bool IsDefault { get; set; }
    }
}

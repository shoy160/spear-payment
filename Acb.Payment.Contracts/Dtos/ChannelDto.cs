using System;
using Acb.Payment.Contracts.Enums;

namespace Acb.Payment.Contracts.Dtos
{
    public class ChannelDto : ChannelInputDto
    {
        public string Id { get; set; }
        public DateTime CreateTime { get; set; }
        public ChannelStatus Status { get; set; }
    }
}

using System;
using Spear.Payment.Contracts.Enums;

namespace Spear.Payment.Contracts.Dtos
{
    public class ChannelDto : ChannelInputDto
    {
        public string Id { get; set; }
        public DateTime CreateTime { get; set; }
        public ChannelStatus Status { get; set; }
    }
}

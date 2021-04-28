﻿using Acb.Core.Domain.Dtos;
using Acb.Payment.Contracts.Enums;
using System;

namespace Acb.Payment.Contracts.Dtos
{
    public class PlatformDto : DDto
    {
        public string id { get; set; }
        public PaymentMode Type { get; set; }
        public string ChannelId { get; set; }
        public string OpenId { get; set; }
        public string UnionId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpireIn { get; set; }
        public DateTime CreateTime { get; set; }
    }
}

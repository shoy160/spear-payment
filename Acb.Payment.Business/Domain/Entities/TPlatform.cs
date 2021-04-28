using Acb.Core.Domain.Entities;
using Acb.Core.Serialize;
using System;

namespace Acb.Payment.Business.Domain.Entities
{
    [Naming("t_platform", NamingType = NamingType.UrlCase)]
    public class TPlatform : BaseEntity<string>
    {
        public byte Type { get; set; }
        public string OpenId { get; set; }
        public string UnionId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpireIn { get; set; }
        public DateTime CreateTime { get; set; }
        public string ChannelId { get; set; }
    }
}

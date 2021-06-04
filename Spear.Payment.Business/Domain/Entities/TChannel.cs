using Spear.Core.Domain.Entities;
using Spear.Core.Serialize;
using System;

namespace Spear.Payment.Business.Domain.Entities
{
    /// <summary> 支付渠道 </summary>
    [Naming(NamingType.UrlCase, Name = "t_channel")]
    public class TChannel : BaseEntity<string>
    {
        /// <summary> 名称 </summary>
        public string Name { get; set; }

        /// <summary> 备注 </summary>
        public string Remark { get; set; }

        /// <summary> 支付方式 </summary>
        public byte Mode { get; set; }

        /// <summary> 支付类型 </summary>
        public string Types { get; set; }

        /// <summary> 合作商户ID </summary>
        public string AppId { get; set; }

        /// <summary> 支付配置 </summary>
        public string Config { get; set; }

        /// <summary> 创建时间 </summary>
        public DateTime CreateTime { get; set; }

        /// <summary> 是否默认 </summary>
        public bool IsDefault { get; set; }

        /// <summary> 状态 </summary>
        public byte Status { get; set; }
    }
}

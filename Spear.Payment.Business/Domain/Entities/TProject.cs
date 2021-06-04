using Spear.Core.Domain.Entities;
using Spear.Core.Serialize;
using System;

namespace Spear.Payment.Business.Domain.Entities
{
    /// <summary> 项目表 </summary>
    [Naming(NamingType.UrlCase, Name = "t_project")]
    public class TProject : BaseEntity<string>
    {
        /// <summary> 项目编号 </summary>
        public string Code { get; set; }
        /// <summary> 项目名称 </summary>
        public string Name { get; set; }
        /// <summary> 支付密钥 </summary>
        public string Secret { get; set; }
        /// <summary> 支付渠道 </summary>
        public string Channels { get; set; }
        /// <summary> 通知URL </summary>
        public string NotifyUrl { get; set; }
        /// <summary> 跳转URL </summary>
        public string RedirectUrl { get; set; }
        /// <summary> 队列名称 </summary>
        public string QueueName { get; set; }
        /// <summary> 状态 </summary>
        public byte Status { get; set; }
        /// <summary> 创建时间 </summary>
        public DateTime CreateTime { get; set; }
    }
}

using System;

namespace Acb.Payment.Contracts.Dtos
{
    public class ProjectDto : ProjectInputDto
    {
        public string Id { get; set; }
        /// <summary> 状态 </summary>
        public byte Status { get; set; }
        /// <summary> 支付密钥 </summary>
        public string Secret { get; set; }
        /// <summary> 创建时间 </summary>
        public DateTime CreateTime { get; set; }
    }
}

using System;
using Acb.Core.Domain.Dtos;
using Acb.Payment.Contracts.Enums;

namespace Acb.Payment.Contracts.Dtos
{
    public class AccountDto : DDto
    {
        public string Id { get; set; }
        /// <summary> 帐号 </summary>
        public string Account { get; set; }

        /// <summary> 角色 </summary>
        public byte Role { get; set; }
        /// <summary> 状态 </summary>
        public CommonStatus Status { get; set; }

        /// <summary> 创建时间 </summary>
        public DateTime CreateTime { get; set; }

        /// <summary> 最后登录时间 </summary>
        public DateTime? LastLoginTime { get; set; }

        /// <summary> 项目ID </summary>
        public string ProjectId { get; set; }

        /// <summary> 头像 </summary>
        public string Avatar { get; set; }
    }
}

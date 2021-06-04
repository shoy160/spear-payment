using Spear.Core.Domain.Entities;
using Spear.Core.Serialize;
using System;

namespace Spear.Payment.Business.Domain.Entities
{
    [Naming("t_account", NamingType = NamingType.UrlCase)]
    public class TAccount : BaseEntity<string>
    {
        /// <summary> 帐号 </summary>
        public string Account { get; set; }

        /// <summary> 密码 </summary>
        public string Password { get; set; }

        /// <summary> 密码盐 </summary>
        public string PasswordSalt { get; set; }

        /// <summary> 角色 </summary>
        public byte Role { get; set; }

        /// <summary> 状态 </summary>
        public byte Status { get; set; }

        /// <summary> 创建时间 </summary>
        public DateTime CreateTime { get; set; }

        /// <summary> 最后登录时间 </summary>
        public DateTime? LastLoginTime { get; set; }

        /// <summary> 项目ID </summary>
        public string ProjectId { get; set; }
        ///// <summary> 昵称 </summary>
        //public string Nick { get; set; }
        /// <summary> 头像 </summary>
        public string Avatar { get; set; }
    }
}

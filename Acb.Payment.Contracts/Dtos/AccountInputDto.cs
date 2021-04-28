using System;
using Acb.Core.Domain.Dtos;
using Acb.Payment.Contracts.Enums;

namespace Acb.Payment.Contracts.Dtos
{
    public class AccountInputDto : DDto
    {
        public string Id { get; set; }

        /// <summary> 帐号 </summary>
        public string Account { get; set; }

        public string Password { get; set; }

        /// <summary> 角色 </summary>
        public AccountRole Role { get; set; }

        /// <summary> 项目ID </summary>
        public string ProjectId { get; set; }

    }
}

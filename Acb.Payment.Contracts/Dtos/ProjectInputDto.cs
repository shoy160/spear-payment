using Acb.Core.Domain.Dtos;
using System;
using System.Collections.Generic;

namespace Acb.Payment.Contracts.Dtos
{
    public class ProjectInputDto : DDto
    {
        /// <summary> 项目编号 </summary>
        public string Code { get; set; }
        /// <summary> 项目名称 </summary>
        public string Name { get; set; }
        /// <summary> 支付渠道 </summary>
        public IEnumerable<string> Channels { get; set; }
        /// <summary> 通知URL </summary>
        public string NotifyUrl { get; set; }
        /// <summary> 跳转URL </summary>
        public string RedirectUrl { get; set; }
        /// <summary> 队列名称 </summary>
        public string QueueName { get; set; }

        public string Password { get; set; }
    }
}

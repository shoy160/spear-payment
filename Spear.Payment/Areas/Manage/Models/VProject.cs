using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Spear.Core.Extensions;
using Spear.Payment.Contracts.Enums;

namespace Spear.Gateway.Payment.Areas.Manage.Models
{
    public class VProjectInput
    {
        /// <summary> 项目编号 </summary>
        [Required(ErrorMessage = "请输入项目编号")]
        public string Code { get; set; }
        /// <summary> 项目名称 </summary>
        [Required(ErrorMessage = "请输入项目名称")]
        public string Name { get; set; }
        /// <summary> 支付渠道 </summary>
        [Required(ErrorMessage = "请选择支付渠道")]
        public IEnumerable<string> Channels { get; set; }
        /// <summary> 登录密码 </summary>
        public string Password { get; set; }
        /// <summary> 通知URL </summary>
        public string NotifyUrl { get; set; }
        /// <summary> 跳转URL </summary>
        public string RedirectUrl { get; set; }
        /// <summary> 队列名称 </summary>
        public string QueueName { get; set; }
    }

    public class VProject
    {
        /// <summary> 项目ID </summary>
        public string id { get; set; }
        /// <summary> 项目编号 </summary>
        public string Code { get; set; }
        /// <summary> 项目名称 </summary>
        public string Name { get; set; }
        /// <summary> 支付渠道 </summary>
        public List<string> Channels { get; set; }
        /// <summary> 支付渠道 </summary>
        public IEnumerable<VChannel> ChannelModels { get; set; }
        /// <summary> 通知URL </summary>
        public string NotifyUrl { get; set; }
        /// <summary> 跳转URL </summary>
        public string RedirectUrl { get; set; }
        /// <summary> 队列名称 </summary>
        public string QueueName { get; set; }
        /// <summary> 状态 </summary>
        public byte Status { get; set; }
        /// <summary> 状态 </summary>
        public string StatusCn => ((ProjectStatus)Status).GetText();
        /// <summary> 支付密钥 </summary>
        public string Secret { get; set; }
        /// <summary> 创建时间 </summary>
        public DateTime CreateTime { get; set; }
    }
}

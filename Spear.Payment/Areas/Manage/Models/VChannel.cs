using System;
using Spear.Payment.Contracts.Enums;
using System.ComponentModel.DataAnnotations;
using Spear.Core.Extensions;

namespace Spear.Gateway.Payment.Areas.Manage.Models
{
    public class VChannelUpdateInput
    {
        /// <summary> 名称 </summary>
        public string Name { get; set; }

        /// <summary> 渠道应用ID </summary>
        public string AppId { get; set; }

        /// <summary> 描述 </summary>
        public string Remark { get; set; }

        /// <summary> 支付类型 </summary>
        public PaymentType[] Types { get; set; }

        /// <summary> 配置 </summary>
        public object Config { get; set; }
    }

    public class VChannelInput
    {
        /// <summary> AppId </summary>
        [Required(ErrorMessage = "请输入AppId")]
        public string AppId { get; set; }
        /// <summary> 名称 </summary>
        [Required(ErrorMessage = "请输入渠道名称")]
        public string Name { get; set; }

        /// <summary> 描述 </summary>
        public string Remark { get; set; }

        /// <summary> 支付方式 </summary>
        [Required(ErrorMessage = "请选择支付方式")]
        public PaymentMode Mode { get; set; }

        /// <summary> 支付类型 </summary>
        public PaymentType[] Types { get; set; }

        /// <summary> 配置 </summary>
        [Required(ErrorMessage = "请输入渠道配置")]
        public object Config { get; set; }
    }

    public class VChannel
    {
        /// <summary> ID </summary>
        public string id { get; set; }

        /// <summary> AppId </summary>
        public string AppId { get; set; }

        /// <summary> 名称 </summary>
        public string Name { get; set; }

        /// <summary> 描述 </summary>
        public string Remark { get; set; }

        /// <summary> 支付方式：0支付宝、1微信 </summary>
        public PaymentMode Mode { get; set; }

        /// <summary> 支付方式 </summary>
        public string ModeCn => Mode.GetText();

        /// <summary> 支持的支付类型 </summary>
        public PaymentType[] Types { get; set; }

        /// <summary> 配置 </summary>
        public object Config { get; set; }

        /// <summary> 是否默认 </summary>
        public bool IsDefault { get; set; }

        /// <summary> 创建时间 </summary>
        public DateTime CreateTime { get; set; }
        /// <summary> 状态 </summary>
        public ChannelStatus Status { get; set; }

        /// <summary> 状态描述 </summary>
        public string StatusCn => Status.GetText();
    }

}

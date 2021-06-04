using Spear.Core.Domain.Dtos;
using Spear.Payment.Contracts.Enums;
using System;

namespace Spear.Payment.Contracts.Dtos
{
    public class TradeInputDto : DDto
    {
        /// <summary> 支付方式 </summary>
        public PaymentMode Mode { get; set; }
        /// <summary> 支付类型 </summary>
        public string Type { get; set; }
        /// <summary> 项目ID </summary>
        public string ProjectId { get; set; }
        /// <summary> 订单号 </summary>
        public string OrderNo { get; set; }
        /// <summary> 支付金额(分) </summary>
        public long Amount { get; set; }
        /// <summary> 支付标题 </summary>
        public string Title { get; set; }
        /// <summary> 支付描述 </summary>
        public string Body { get; set; }
        /// <summary> 扩展信息 </summary>
        public string Extend { get; set; }
        /// <summary> 原始支付参数 </summary>
        public string RawParams { get; set; }
        /// <summary> 渠道ID </summary>
        public string ChannelId { get; set; }

        /// <summary> 跳转链接 </summary>
        public string RedirectUrl { get; set; }
    }
}

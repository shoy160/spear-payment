using Spear.Payment.Contracts.Enums;
using System;
using Spear.Core.Extensions;

namespace Spear.Gateway.Payment.Areas.Manage.Models
{
    public class VTrade
    {
        /// <summary> 交易ID </summary>
        public string id { get; set; }

        /// <summary> 支付方式 </summary>
        public PaymentMode Mode { get; set; }

        /// <summary> 支付方式 </summary>
        public string ModeCn => Mode.GetText();

        /// <summary> 支付类型 </summary>
        public string Type { get; set; }

        /// <summary> 项目ID </summary>
        public string ProjectId { get; set; }

        /// <summary> 项目名称 </summary>
        public string ProjectName { get; set; }
        /// <summary> 是否设置了异步通知 </summary>
        public bool Notify { get; set; }

        /// <summary> 交易号 </summary>
        public string TradeNo { get; set; }

        /// <summary> 商户订单号 </summary>
        public string OrderNo { get; set; }

        /// <summary> 支付金额(分) </summary>
        public long Amount { get; set; }

        /// <summary> 支付金额(元) </summary>
        public decimal AmountY => (decimal)Amount / 100;

        /// <summary> 支付标题 </summary>
        public string Title { get; set; }

        /// <summary> 支付描述 </summary>
        public string Body { get; set; }

        /// <summary> 扩展信息 </summary>
        public string Extend { get; set; }

        /// <summary> 支付状态 </summary>
        public TradeStatus Status { get; set; }

        /// <summary> 支付状态 </summary>
        public string StatusCn => Status.GetText();

        /// <summary> 原始支付参数 </summary>
        public string RawParams { get; set; }

        /// <summary> 创建时间 </summary>
        public DateTime CreateTime { get; set; }

        /// <summary> 第三方支付交易号 </summary>
        public string OutTradeNo { get; set; }

        /// <summary> 支付用户 </summary>
        public string PaidUser { get; set; }

        /// <summary> 支付帐号 </summary>
        public string PaidAccount { get; set; }

        /// <summary> 支付时间 </summary>
        public DateTime? PaidTime { get; set; }

        /// <summary> 支付渠道ID </summary>
        public string ChannelId { get; set; }

        /// <summary> 跳转链接 </summary>
        public string RedirectUrl { get; set; }
    }
}

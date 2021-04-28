using Acb.Core.Domain.Entities;
using Acb.Core.Serialize;
using System;

namespace Acb.Payment.Business.Domain.Entities
{
    /// <summary> 交易表 </summary>
    [Naming(NamingType.UrlCase, Name = "t_trade")]
    public class TTrade : BaseEntity<string>
    {
        /// <summary> 支付方式 </summary>
        public byte Mode { get; set; }

        /// <summary> 支付类型 </summary>
        public string Type { get; set; }

        /// <summary> 项目ID </summary>
        public string ProjectId { get; set; }

        /// <summary> 交易号 </summary>
        public string TradeNo { get; set; }
        /// <summary> 商户订单号 </summary>
        public string OrderNo { get; set; }

        /// <summary> 支付金额(分) </summary>
        public long Amount { get; set; }

        /// <summary> 支付标题 </summary>
        public string Title { get; set; }

        /// <summary> 支付描述 </summary>
        public string Body { get; set; }

        /// <summary> 扩展信息 </summary>
        public string Extend { get; set; }

        /// <summary> 支付状态 </summary>
        public byte Status { get; set; }

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
        public string PlatformId { get; set; }

        ///<summary> 扫码支付链接 </summary>
        [Naming("scan_url")]
        public string ScanUrl { get; set; }

        ///<summary> 退款订单号 </summary>
        [Naming("refund_no")]
        public string RefundNo { get; set; }

        ///<summary> 支付平台退款单号 </summary>
        [Naming("out_refund_no")]
        public string OutRefundNo { get; set; }

        ///<summary> 退款金额 </summary>
        [Naming("refund_amount")]
        public long? RefundAmount { get; set; }

        ///<summary> 退款原因 </summary>
        [Naming("refund_reason")]
        public string RefundReason { get; set; }

        ///<summary> 退款时间 </summary>
        [Naming("refund_time")]
        public DateTime? RefundTime { get; set; }
    }
}

using Spear.Payment.Contracts.Enums;
using System;

namespace Spear.Gateway.Payment.ViewModels
{
    public class VTrade
    {
        public string id { get; set; }

        /// <summary> 支付方式 </summary>
        public PaymentMode Mode { get; set; }
        /// <summary> 交易状态 </summary>
        public TradeStatus Status { get; set; }

        /// <summary> 支付类型 </summary>
        public string Type { get; set; }

        /// <summary> 订单号 </summary>
        public string OrderNo { get; set; }

        /// <summary> 支付金额(分) </summary>
        public long Amount { get; set; }

        /// <summary> 支付标题 </summary>
        public string Title { get; set; }

        /// <summary> 支付描述 </summary>
        public string Body { get; set; }

        /// <summary> 跳转链接 </summary>
        public string RedirectUrl { get; set; }
        /// <summary> 平台ID </summary>
        public string PlatformId { get; set; }
        /// <summary> 支付用户 </summary>
        public string PaidUser { get; set; }

        /// <summary> 支付帐号 </summary>
        public string PaidAccount { get; set; }

        /// <summary> 第三方支付交易号 </summary>
        public string OutTradeNo { get; set; }

        /// <summary> 支付时间 </summary>
        public DateTime? PaidTime { get; set; }
    }
}

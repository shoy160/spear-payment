using Acb.Core.Domain.Dtos;
using Acb.Payment.Contracts.Enums;
using System;

namespace Acb.Payment.Contracts.Dtos
{
    public class TradePaidInputDto : DDto
    {
        /// <summary> 交易号 </summary>
        public string TradeNo { get; set; }

        /// <summary> 第三方支付交易号 </summary>
        public string OutTradeNo { get; set; }

        /// <summary> 支付帐号 </summary>
        public string Account { get; set; }

        /// <summary> 支付用户 </summary>
        public string User { get; set; }

        /// <summary> 支付时间 </summary>
        public DateTime PaidTime { get; set; }

        /// <summary> 支付金额 </summary>
        public long Amount { get; set; }

        public PaymentMode Mode { get; set; }
    }
}

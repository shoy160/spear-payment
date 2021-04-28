using System;
using Acb.Payment.Contracts.Enums;

namespace Acb.Payment.Contracts.Dtos
{
    public class TradeDto : TradeInputDto
    {
        public string Id { get; set; }

        /// <summary> 交易号 </summary>
        public string TradeNo { get; set; }
        
        /// <summary> 支付状态 </summary>
        public TradeStatus Status { get; set; }

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
        public string PlatformId { get; set; }
    }
}

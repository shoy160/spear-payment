using Acb.Core.Domain.Entities;
using System;
using Acb.Core.Serialize;

namespace Acb.Payment.Business.Domain.Entities
{
    [Naming("t_trade_payment", NamingType = NamingType.UrlCase)]
    public class TTradePayment : BaseEntity<string>
    {
        /// <summary> 交易ID </summary>
        public string TradeId { get; set; }
        /// <summary> 支付方式 </summary>
        public byte Mode { get; set; }
        /// <summary> 支付类型 </summary>
        public string Type { get; set; }
        /// <summary> 支付链接或参数 </summary>
        public string Value { get; set; }
        /// <summary> 创建时间 </summary>
        public DateTime CreateTime { get; set; }
    }
}

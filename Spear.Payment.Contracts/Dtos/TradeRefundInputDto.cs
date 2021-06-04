using System;

namespace Spear.Payment.Contracts.Dtos
{
    public class TradeRefundInputDto
    {
        public string TradeId { get; set; }
        public long? Amount { get; set; }
        public string Reason { get; set; }
        public string RefundNo { get; set; }
        public string OutRefundNo { get; set; }
        public DateTime? RefundTime { get; set; }
    }
}

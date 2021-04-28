using Acb.Core.Domain.Entities;
using Acb.Core.Serialize;
using System;

namespace Acb.Payment.Business.Domain.Entities
{
    /// <summary> 交易通知表 </summary>
    [Naming(NamingType.UrlCase, Name = "t_trade_notify")]
    public class TTradeNotify : BaseEntity<string>
    {
        /// <summary> 交易ID </summary>
        public string TradeId { get; set; }
        /// <summary> 类型 </summary>
        public byte Type { get; set; }
        /// <summary> 内容 </summary>
        public string Content { get; set; }
        /// <summary> 结果 </summary>
        public string Result { get; set; }
        /// <summary> 状态 </summary>
        public byte Status { get; set; }
        /// <summary> 时间 </summary>
        public DateTime CreateTime { get; set; }
        public string Url { get; set; }
    }
}

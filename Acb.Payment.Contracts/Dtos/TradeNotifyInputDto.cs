using Acb.Core.Domain.Dtos;
using System;
using Acb.Payment.Contracts.Enums;

namespace Acb.Payment.Contracts.Dtos
{
    public class TradeNotifyInputDto : DDto
    {
        /// <summary> 交易ID </summary>
        public string TradeId { get; set; }
        /// <summary> 类型 </summary>
        public NotifyType Type { get; set; }
        /// <summary> 内容 </summary>
        public string Content { get; set; }
        /// <summary> 结果 </summary>
        public string Result { get; set; }
        ///// <summary> 状态 </summary>
        //public byte Status { get; set; }
        public string Url { get; set; }
    }
}

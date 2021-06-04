using Spear.Core.Extensions;
using Spear.Payment.Contracts.Enums;
using System;

namespace Spear.Gateway.Payment.Areas.Manage.Models
{
    public class VTradeNotify
    {
        /// <summary> 通知ID </summary>
        public string id { get; set; }
        /// <summary> 交易ID </summary>
        public string TradeId { get; set; }
        /// <summary> 交易号 </summary>
        public string TradeNo { get; set; }
        /// <summary> 时间 </summary>
        public DateTime CreateTime { get; set; }
        /// <summary> 类型 </summary>
        public NotifyType Type { get; set; }
        /// <summary> 类型 </summary>
        public string TypeCn => Type.GetText();
        /// <summary> 内容 </summary>
        public string Content { get; set; }
        /// <summary> 结果 </summary>
        public string Result { get; set; }
        /// <summary> 状态 </summary>
        public NotifyStatus Status { get; set; }
        /// <summary> 状态 </summary>
        public string StatusCn => Status.GetText();
        /// <summary> Url </summary>
        public string Url { get; set; }
    }
}

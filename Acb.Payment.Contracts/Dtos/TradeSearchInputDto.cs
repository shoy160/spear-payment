﻿using Acb.Core.Domain.Dtos;
using Acb.Payment.Contracts.Enums;
using System;

namespace Acb.Payment.Contracts.Dtos
{
    public class TradeSearchInputDto : PageAndTimeDto
    {
        /// <summary> 支付方式 </summary>
        public PaymentMode? Mode { get; set; }
        /// <summary> 状态 </summary>
        public TradeStatus? Status { get; set; }
        /// <summary> 渠道ID </summary>
        public string ChannelId { get; set; }
        /// <summary> 项目ID </summary>
        public string ProjectId { get; set; }
        /// <summary> 交易号 </summary>
        public string TradeNo { get; set; }
        /// <summary> 商户订单号 </summary>
        public string OrderNo { get; set; }
        /// <summary> 支付订单号 </summary>
        public string OutTradeNo { get; set; }
        /// <summary> 支付时间起（含） </summary>
        public DateTime? PaidTimeBegin { get; set; }
        /// <summary> 支付时间止 </summary>
        public DateTime? PaidTimeEnd { get; set; }
    }
}

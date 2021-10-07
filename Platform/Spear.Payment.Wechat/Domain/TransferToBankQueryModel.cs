﻿using Spear.Payment.Core;
using Spear.Payment.Core.Utils;
using System.ComponentModel.DataAnnotations;
using Spear.Payment.Core.Attributes;

namespace Spear.Payment.Wechat.Domain
{
    public class TransferToBankQueryModel
    {
        /// <summary>
        /// 商户订单号
        /// </summary>
        [ReName(Name = "partner_trade_no")]
        [Required(ErrorMessage = "请设置商户订单号")]
        [StringLength(32, ErrorMessage = "商户订单号最大长度为32位")]
        public string OutTradeNo { get; set; }

        /// <summary>
        /// 随机字符串
        /// </summary>
        public string NonceStr { get; } = Util.GenerateNonceStr();
    }
}

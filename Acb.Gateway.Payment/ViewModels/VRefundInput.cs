namespace Acb.Gateway.Payment.ViewModels
{
    public class VRefundInput : VPaymentSignature
    {
        /// <summary> 订单号 </summary>
        public string OrderNo { get; set; }
        /// <summary> 总金额(分) </summary>
        public long Amount { get; set; }
        /// <summary> 退款订单号 </summary>
        public string RefundOrderNo { get; set; }
        /// <summary> 退款金额 </summary>
        public long RefundAmount { get; set; }
        /// <summary> 退款描述 </summary>
        public string RefundDesc { get; set; }

    }
}

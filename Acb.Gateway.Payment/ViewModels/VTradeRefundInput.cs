namespace Acb.Gateway.Payment.ViewModels
{
    public class VTradeRefundInput : VPaymentSignature
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 退款订单号
        /// </summary>
        public string RefundNo { get; set; }

        /// <summary>
        /// 退款金额(分)
        /// </summary>
        public long Amount { get; set; } = -1;

        /// <summary>
        /// 退款原因
        /// </summary>
        public string Reason { get; set; }
    }
}

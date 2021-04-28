namespace Acb.Sdk.Payment.Models
{
    public class CashierInput
    {
        /// <summary> 子系统订单号(必填) </summary>
        public string OrderNo { get; set; }

        /// <summary> 金额(分，必填) </summary>
        public long Amount { get; set; }

        /// <summary> 支付标题 </summary>
        public string Title { get; set; }

        /// <summary> 支付描述 </summary>
        public string Body { get; set; }

        /// <summary> 扩展信息,通知时原样返回 </summary>
        public string Extend { get; set; }

        /// <summary> 跳转url </summary>
        public string RedirectUrl { get; set; }
        /// <summary> 是否是多码合一 </summary>
        public bool Scan { get; set; }

        public CashierInput() { }

        /// <summary> Ctor </summary>
        /// <param name="amount">金额(分，必填)</param>
        /// <param name="orderNo">业务订单号(必填)</param>
        public CashierInput(long amount, string orderNo)
        {
            Amount = amount;
            OrderNo = orderNo;
        }
    }
}

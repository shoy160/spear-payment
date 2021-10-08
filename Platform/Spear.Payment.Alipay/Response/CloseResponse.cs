using Spear.Payment.Core.Request;

namespace Spear.Payment.Alipay.Response
{
    public class CloseResponse : BaseResponse
    {
        /// <summary>
        /// 支付宝交易号
        /// </summary>
        public string TradeNo { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        public string OutTradeNo { get; set; }

        internal override void Execute<TModel, TResponse>(Merchant merchant, Request<TModel, TResponse> request)
        {
        }
    }
}

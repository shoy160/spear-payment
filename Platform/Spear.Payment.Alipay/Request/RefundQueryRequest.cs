using Spear.Payment.Alipay.Domain;
using Spear.Payment.Alipay.Response;

namespace Spear.Payment.Alipay.Request
{
    public class RefundQueryRequest : BaseRequest<RefundQueryModel, RefundQueryResponse>
    {
        public RefundQueryRequest()
            : base("alipay.trade.fastpay.refund.query")
        {
        }
    }
}

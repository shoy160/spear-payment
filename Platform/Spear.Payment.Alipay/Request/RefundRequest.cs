using Spear.Payment.Alipay.Domain;
using Spear.Payment.Alipay.Response;

namespace Spear.Payment.Alipay.Request
{
    public class RefundRequest : BaseRequest<RefundModel, RefundResponse>
    {
        public RefundRequest()
            : base("alipay.trade.refund")
        {
        }
    }
}

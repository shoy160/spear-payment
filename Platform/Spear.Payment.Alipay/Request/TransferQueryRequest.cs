using Spear.Payment.Alipay.Domain;
using Spear.Payment.Alipay.Response;

namespace Spear.Payment.Alipay.Request
{
    public class TransferQueryRequest : BaseRequest<TransferQueryModel, TransferQueryResponse>
    {
        public TransferQueryRequest()
            : base("alipay.fund.trans.order.query")
        {
        }
    }
}

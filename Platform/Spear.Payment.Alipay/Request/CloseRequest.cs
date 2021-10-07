using Spear.Payment.Alipay.Domain;
using Spear.Payment.Alipay.Response;

namespace Spear.Payment.Alipay.Request
{
    public class CloseRequest : BaseRequest<CloseModel, CloseResponse>
    {
        public CloseRequest()
            : base("alipay.trade.close")
        {
        }
    }
}

using Spear.Payment.Alipay.Domain;
using Spear.Payment.Alipay.Response;
using Spear.Payment.Alipay.Domain;

namespace Spear.Payment.Alipay.Request
{
    public class ScanPayRequest : BaseRequest<ScanPayModel, ScanPayResponse>
    {
        public ScanPayRequest()
            : base("alipay.trade.precreate")
        {
        }
    }
}

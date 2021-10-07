using Spear.Payment.Alipay.Request;
using Spear.Payment.Alipay.Domain;
using Spear.Payment.Alipay.Response;

namespace Spear.Payment.Alipay.Request
{
    public class WapPayRequest : BaseRequest<WapPayModel, WapPayResponse>
    {
        public WapPayRequest()
            : base("alipay.trade.wap.pay")
        {
        }
    }
}

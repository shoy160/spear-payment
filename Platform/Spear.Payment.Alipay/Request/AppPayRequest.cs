using Spear.Payment.Alipay.Request;
using Spear.Payment.Alipay.Domain;
using Spear.Payment.Alipay.Response;

namespace Spear.Payment.Alipay.Request
{
    public class AppPayRequest : BaseRequest<AppPayModel, AppPayResponse>
    {
        public AppPayRequest()
            : base("alipay.trade.app.pay")
        {
        }
    }
}

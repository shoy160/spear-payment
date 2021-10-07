using Spear.Payment.Alipay.Request;
using Spear.Payment.Alipay.Domain;
using Spear.Payment.Alipay.Response;

namespace Spear.Payment.Alipay.Request
{
    public class WebPayRequest : BaseRequest<WebPayModel, WebPayResponse>
    {
        public WebPayRequest()
            : base("alipay.trade.page.pay")
        {
        }
    }
}

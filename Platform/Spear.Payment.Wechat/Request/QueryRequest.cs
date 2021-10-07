using Spear.Payment.Wechat.Domain;
using Spear.Payment.Wechat.Response;

namespace Spear.Payment.Wechat.Request
{
    public class QueryRequest : BaseRequest<QueryModel, QueryResponse>
    {
        public QueryRequest()
        {
            RequestUrl = "/pay/orderquery";
        }

        internal override void Execute(Merchant merchant)
        {
            GatewayData.Remove("notify_url");
        }
    }
}

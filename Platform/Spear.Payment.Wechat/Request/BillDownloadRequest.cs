using Spear.Payment.Wechat.Domain;
using Spear.Payment.Wechat.Response;

namespace Spear.Payment.Wechat.Request
{
    public class BillDownloadRequest : BaseRequest<BillDownloadModel, BillDownloadResponse>
    {
        public BillDownloadRequest()
        {
            RequestUrl = "/pay/downloadbill";
        }

        internal override void Execute(Merchant merchant)
        {
            GatewayData.Remove("notify_url");
        }
    }
}

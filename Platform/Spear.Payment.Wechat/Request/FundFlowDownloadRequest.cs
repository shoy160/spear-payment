using Spear.Payment.Wechat.Domain;
using Spear.Payment.Wechat.Response;

namespace Spear.Payment.Wechat.Request
{
    public class FundFlowDownloadRequest : BaseRequest<FundFlowDownloadModel, FundFlowDownloadResponse>
    {
        public FundFlowDownloadRequest()
        {
            RequestUrl = "/pay/downloadfundflow";
            IsUseCert = true;
        }

        internal override void Execute(Merchant merchant)
        {
            GatewayData.Remove("notify_url");
            GatewayData.Add("sign_type", "HMAC-SHA256");
        }
    }
}

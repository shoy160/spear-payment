using PaySharp.Core.Utils;
using PaySharp.Wechatpay.Response;

namespace PaySharp.Wechatpay.Request
{
    public class SandboxKeyRequest : BaseRequest<object, SandboxKeyResponse>
    {
        public SandboxKeyRequest()
        {
            RequestUrl = "https://api.mch.weixin.qq.com/sandboxnew/pay/getsignkey";
            IsUseCert = false;
        }

        internal override void Execute(Merchant merchant)
        {
            GatewayData.Remove("appid");
            GatewayData.Remove("notify_url");
            GatewayData.Add("nonce_str", Util.GenerateNonceStr());
        }
    }
}

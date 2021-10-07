using Spear.Payment.Core.Request;

namespace Spear.Payment.Wechat.Response
{
    public class CloseResponse : BaseResponse
    {
        internal override void Execute<TModel, TResponse>(Merchant merchant, Request<TModel, TResponse> request)
        {
        }
    }
}

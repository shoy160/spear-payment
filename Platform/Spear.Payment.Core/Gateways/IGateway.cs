using Spear.Payment.Core.Request;
using Spear.Payment.Core.Response;

namespace Spear.Payment.Core.Gateways
{
    public interface IGateway
    {
        TResponse Execute<TModel, TResponse>(Request<TModel, TResponse> request) where TResponse : IResponse;
    }
}

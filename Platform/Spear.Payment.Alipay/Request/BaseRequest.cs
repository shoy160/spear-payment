﻿using Spear.Payment.Core.Request;
using Spear.Payment.Core.Response;
using Spear.Payment.Core.Utils;

namespace Spear.Payment.Alipay.Request
{
    public class BaseRequest<TModel, TResponse> : Request<TModel, TResponse> where TResponse : IResponse
    {
        public BaseRequest(string method)
        {
            RequestUrl = "/gateway.do?charset=UTF-8";
            GatewayData.Add("method", method);
        }

        public override void AddGatewayData(TModel model)
        {
            base.AddGatewayData(model);

            GatewayData.Add("biz_content", Util.SerializeObject(model));
        }
    }
}

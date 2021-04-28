using Acb.Core.Extensions;
using System.Collections.Generic;

namespace Acb.Gateway.Payment.Payment
{
    internal static class PlatformHelper
    {
        private const string AuthorzieUrl = "https://open.weixin.qq.com/connect/oauth2/authorize";

        public static string WechatLogin(string appId, string state, string redirect, string scope = "snsapi_base")
        {
            var dict = new Dictionary<string, object>
            {
                {"appid", appId},
                {"redirect_uri", redirect},
                {"response_type", "code"},
                {"scope", scope},
                {"state", state}
            };
            var param = dict.ToUrl();
            return $"{AuthorzieUrl}?{param}";
        }
    }
}

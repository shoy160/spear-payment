using PaySharp.Core.Utils;
using System;

namespace PaySharp.Core.Notify
{
    /// <summary>
    /// 网关通知的处理类，通过对返回数据的分析识别网关类型
    /// </summary>
    internal static class NotifyProcess
    {
        /// <summary>
        /// 是否是Xml格式数据
        /// </summary>
        /// <returns></returns>
        private static bool IsXmlData => HttpUtil.ContentType.Contains("text/xml") ||
                                         HttpUtil.ContentType.Contains("application/xml");

        private static bool IsForm => HttpUtil.ContentType.Contains("application/x-www-form-urlencoded"); //== "application/x-www-form-urlencoded";
        private static bool IsJson => HttpUtil.ContentType.Contains("application/json");

        /// <summary> 是否是GET请求 </summary>
        /// <returns></returns>
        private static bool IsGetRequest => HttpUtil.RequestType == "GET";

        /// <summary>
        /// 获取网关
        /// </summary>
        /// <param name="gatewayFunc">网关列表</param>
        /// <returns></returns>
        public static BaseGateway GetGateway(Func<string, BaseGateway> gatewayFunc)
        {
            var gatewayData = ReadNotifyData();

            var appId = gatewayData.GetStringValue("appid");
            if (string.IsNullOrWhiteSpace(appId))
            {
                appId = gatewayData.GetStringValue("app_id");
            }
            var gateway = gatewayFunc.Invoke(appId) ?? new NullGateway();

            gateway.GatewayData = gatewayData;
            return gateway;
        }

        /// <summary>
        /// 网关参数数据项中是否存在指定的所有参数名
        /// </summary>
        /// <param name="parmaName">参数名数组</param>
        /// <param name="gatewayData">网关数据</param>
        public static bool ExistParameter(string[] parmaName, GatewayData gatewayData)
        {
            int compareCount = 0;
            foreach (var item in parmaName)
            {
                if (gatewayData.Exists(item))
                {
                    compareCount++;
                }
            }

            if (compareCount == parmaName.Length)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 读取网关发回的数据
        /// </summary>
        /// <returns></returns>
        public static GatewayData ReadNotifyData()
        {
            var gatewayData = new GatewayData();
            if (IsGetRequest)
            {
                gatewayData.FromQueryString(HttpUtil.QueryString);
            }
            else
            {
                gatewayData.Raw = HttpUtil.BodyContent;
                if (IsXmlData)
                {
                    gatewayData.FromXml(gatewayData.Raw);
                }
                else
                {
                    if (IsJson)
                    {
                        gatewayData.FromJson(gatewayData.Raw);
                    }
                    else
                    {
                        gatewayData.FromQueryString(gatewayData.Raw);
                    }
                }

                HttpUtil.Current.Items.Add("request_data", gatewayData.Raw);
            }

            return gatewayData;
        }
    }
}

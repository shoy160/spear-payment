namespace Spear.Sdk.Payment.Models
{
    public class PayInput
    {
        /// <summary> 子系统订单号 </summary>
        public string OrderNo { get; set; }

        /// <summary> 金额(分) </summary>
        public long Amount { get; set; }

        /// <summary> 支付标题 </summary>
        public string Title { get; set; }

        /// <summary> 支付描述 </summary>
        public string Body { get; set; }

        /// <summary> 扩展信息,通知时原样返回 </summary>
        public string Extend { get; set; }
        /// <summary> 跳转链接 </summary>
        public string RedirectUrl { get; set; }

        /// <summary> OpenId(公众号/小程序支付必填) </summary>
        public string OpenId { get; set; }

        /// <summary> 微信H5支付 (场景信息) </summary>
        public SceneInfo SceneInfo { get; set; }
    }

    public class SceneInfo
    {
        public H5Info h5_info { get; set; }

        public SceneInfo(string wapName, string wapUrl)
        {
            h5_info = new H5Info
            {
                wap_name = wapName,
                wap_url = wapUrl
            };
        }
    }

    public class H5Info
    {
        public string type { get; } = "Wap";
        public string wap_name { get; set; }
        public string wap_url { get; set; }
    }
}

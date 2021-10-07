namespace Spear.Payment.Wechat.Domain
{
    public class OAuthModel
    {
        /// <summary>
        /// 微信Code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 授权类型
        /// </summary>
        public string GrantType => "authorization_code";
    }
}

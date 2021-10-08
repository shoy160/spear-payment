#if NETSTANDARD2_0
using Microsoft.Extensions.Options;
#endif
using Spear.Payment.Core.Exceptions;
using Spear.Payment.Core.Request;
using Spear.Payment.Core.Utils;
using Spear.Payment.Wechat.Request;
using Spear.Payment.Wechat.Response;
using System.Threading.Tasks;
using Spear.Payment.Core.Gateways;
using static Spear.Payment.Wechat.Response.QueryResponse;

namespace Spear.Payment.Wechat
{
    /// <summary>
    /// 微信支付网关
    /// </summary>
    public sealed class WechatGateway : BaseGateway
    {
        #region 私有字段

        private readonly Merchant _merchant;

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化微信支付网关
        /// </summary>
        /// <param name="merchant">商户数据</param>
        public WechatGateway(Merchant merchant)
            : base(merchant)
        {
            _merchant = merchant;
        }

#if NETSTANDARD2_0

        /// <summary>
        /// 初始化微信支付网关
        /// </summary>
        /// <param name="merchant">商户数据</param>
        public WechatGateway(IOptions<Merchant> merchant)
            : this(merchant.Value)
        {
        }

#endif

        #endregion

        #region 属性
        private string _gatewayUrl;

        public override string GatewayUrl
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_gatewayUrl))
                    return _gatewayUrl;
                return Merchant.IsTest ? "https://api.mch.weixin.qq.com/sandboxnew" : "https://api.mch.weixin.qq.com";
            }
            set
            {
                _gatewayUrl = value;
            }
        }

        public new Merchant Merchant => _merchant;

        public new NotifyResponse NotifyResponse => (NotifyResponse)base.NotifyResponse;

        protected override bool IsPaySuccess => NotifyResponse.ResultCode == "SUCCESS" && !string.IsNullOrEmpty(NotifyResponse.TradeType);

        protected override bool IsRefundSuccess => NotifyResponse.RefundStatus == "SUCCESS";

        protected override bool IsCancelSuccess { get; }

        protected override string[] NotifyVerifyParameter => new string[]
        {
            "appid", "return_code", "mch_id", "nonce_str"
        };

        #endregion

        #region 方法

        protected override async Task<bool> ValidateNotifyAsync()
        {
            base.NotifyResponse = await GatewayData.ToObjectAsync<NotifyResponse>(StringCase.Snake);
            base.NotifyResponse.Raw = GatewayData.Raw;

            if (NotifyResponse.ReturnCode != "SUCCESS")
            {
                throw new GatewayException("不是成功的返回码");
            }

            if (string.IsNullOrEmpty(NotifyResponse.ReqInfo))
            {
                NotifyResponse.Coupons = ConvertUtil.ToList<CouponResponse, object>(GatewayData, -1);
                if (NotifyResponse.Sign != SubmitProcess.BuildSign(GatewayData, _merchant.Key))
                {
                    throw new GatewayException("签名不一致");
                }
            }
            else
            {
                var tempNotify = NotifyResponse;
                var key = EncryptUtil.Md5(_merchant.Key).ToLower();
                var data = EncryptUtil.AesDecrypt(NotifyResponse.ReqInfo, key);
                var gatewayData = new GatewayData();
                gatewayData.FromXml(data);
                base.NotifyResponse = await gatewayData.ToObjectAsync<NotifyResponse>(StringCase.Snake);
                GatewayData.Add(NotifyResponse, StringCase.Snake);

                NotifyResponse.AppId = tempNotify.AppId;
                NotifyResponse.MchId = tempNotify.MchId;
                NotifyResponse.NonceStr = tempNotify.NonceStr;
                NotifyResponse.ReqInfo = tempNotify.ReqInfo;
                NotifyResponse.ReturnCode = tempNotify.ReturnCode;
            }

            return true;
        }

        protected override void WriteSuccessFlag()
        {
            GatewayData.Clear();
            GatewayData.Add("return_code", "SUCCESS");
            HttpUtil.Write(GatewayData.ToXml());
        }

        protected override void WriteFailureFlag()
        {
            GatewayData.Clear();
            GatewayData.Add("return_code", "FAIL");
            HttpUtil.Write(GatewayData.ToXml());
        }

        public override TResponse Execute<TModel, TResponse>(Request<TModel, TResponse> request)
        {
            if (request is OAuthRequest)
            {
                return SubmitProcess.AuthExecute(_merchant, request, GatewayUrl);
            }

            return SubmitProcess.Execute(_merchant, request, GatewayUrl);
        }

        #endregion
    }
}

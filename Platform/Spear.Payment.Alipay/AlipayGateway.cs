
using Microsoft.Extensions.Options;
using Spear.Payment.Alipay.Request;
using Spear.Payment.Alipay.Response;
using Spear.Payment.Core.Exceptions;
using Spear.Payment.Core.Gateways;
using Spear.Payment.Core.Request;
using Spear.Payment.Core.Utils;
using System.Threading.Tasks;

#if NETSTANDARD2_0
#endif

namespace Spear.Payment.Alipay
{
    /// <summary>
    /// 支付宝网关
    /// </summary>
    public sealed class AlipayGateway : BaseGateway
    {
        #region 私有字段

        private readonly Merchant _merchant;

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化支付宝网关
        /// </summary>
        /// <param name="merchant">商户数据</param>
        public AlipayGateway(Merchant merchant)
            : base(merchant)
        {
            _merchant = merchant;
        }

#if NETSTANDARD2_0

        /// <summary>
        /// 初始化支付宝网关
        /// </summary>
        /// <param name="merchant">商户数据</param>
        public AlipayGateway(IOptions<Merchant> merchant)
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
                return Merchant.IsTest ? "https://openapi.alipaydev.com" : "https://openapi.alipay.com";
            }
            set
            {
                _gatewayUrl = value;
            }
        }

        public new NotifyResponse NotifyResponse => (NotifyResponse)base.NotifyResponse;

        protected override bool IsPaySuccess =>
            NotifyResponse.TradeStatus == "TRADE_SUCCESS" || NotifyResponse.TradeStatus == "TRADE_FINISHED";

        protected override bool IsRefundSuccess => NotifyResponse.RefundAmount > 0;

        protected override bool IsCancelSuccess { get; }

        protected override string[] NotifyVerifyParameter => new string[]
        {
            "app_id","version", "charset","trade_no", "sign","sign_type"
        };

        #endregion

        #region 公共方法

        protected override async Task<bool> ValidateNotifyAsync()
        {
            base.NotifyResponse = await GatewayData.ToObjectAsync<NotifyResponse>(StringCase.Snake);
            base.NotifyResponse.Raw = GatewayData.ToUrl(false);
            GatewayData.Remove("sign");
            GatewayData.Remove("sign_type");

            var result = EncryptUtil.RSAVerifyData(GatewayData.ToUrl(false),
                NotifyResponse.Sign, _merchant.AlipayPublicKey, _merchant.SignType);
            if (result)
            {
                return true;
            }
            throw new GatewayException("签名不一致");
        }

        public override TResponse Execute<TModel, TResponse>(Request<TModel, TResponse> request)
        {
            if (request is WapPayRequest || request is WebPayRequest || request is AppPayRequest)
            {
                return SubmitProcess.SdkExecute(_merchant, request, GatewayUrl);
            }

            return SubmitProcess.Execute(_merchant, request, GatewayUrl);
        }

        #endregion
    }
}
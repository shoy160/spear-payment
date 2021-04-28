using Acb.Core.Exceptions;
using Acb.Gateway.Payment.Payment;
using Acb.Gateway.Payment.ViewModels;
using Acb.Payment.Contracts;
using Acb.Payment.Contracts.Dtos;
using Acb.Payment.Contracts.Enums;
using PaySharp.Core;
using System.Threading.Tasks;

namespace Acb.Gateway.Payment.Controllers
{
    /// <summary> 支付基类 </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PaymentController<T> : DController where T : BaseGateway
    {
        /// <summary> 支付方式 </summary>
        protected readonly PaymentMode Mode;

        /// <summary> 交易契约 </summary>
        public ITradeContract TradeContract { protected get; set; }

        /// <summary> 构造函数 </summary>
        /// <param name="mode"></param>
        protected PaymentController(PaymentMode mode)
        {
            Mode = mode;
        }

        /// <summary> 支付网关 </summary>
        protected IGateway Gateway(PaymentType? type = null)
        {
            var channel = Channel(Mode, type);
            if (string.IsNullOrWhiteSpace(channel?.AppId))
                throw new BusiException("未开通支付方式");
            var gateway = Mode.CreateGateway(channel.Config);
            if (gateway == null)
                throw new BusiException("支付方式异常");
            return gateway;
        }

        /// <summary> 创建交易 </summary>
        /// <param name="type"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        protected Task<TradeDto> CreateTrade(PaymentType type, VPaymentInput input)
        {
            return base.CreateTrade(input, Mode, type);
        }

        /// <summary> 获取交易信息 </summary>
        /// <param name="orderNo"></param>
        /// <param name="projectCode"></param>
        /// <returns></returns>
        protected Task<TradeDto> GetTrade(string orderNo, string projectCode = null)
        {
            projectCode = string.IsNullOrWhiteSpace(projectCode) ? Current.Project()?.Code : projectCode;
            return TradeContract.DetailAsync(projectCode, orderNo);
        }
    }
}

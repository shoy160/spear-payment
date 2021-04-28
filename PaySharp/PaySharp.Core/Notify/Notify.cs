using Acb.Core;
using Acb.Core.Exceptions;
using Acb.Core.Logging;
using PaySharp.Core.Events;
using PaySharp.Core.Exceptions;
using PaySharp.Core.Utils;
using System;
using System.Threading.Tasks;

namespace PaySharp.Core.Notify
{
    /// <summary>
    /// 网关返回的支付通知数据的接受
    /// </summary>
    public class Notify
    {
        #region 私有字段

        private readonly Func<string, BaseGateway> _gatewayFunc;

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化支付通知
        /// </summary>
        /// <param name="gatewayFunc">用于验证支付网关返回数据的网关列表</param>
        public Notify(Func<string, BaseGateway> gatewayFunc)
        {
            _gatewayFunc = gatewayFunc;
        }

        #endregion

        #region 事件

        /// <summary>
        /// 网关异步返回的支付通知验证成功时触发
        /// </summary>
        public event Func<object, PaySucceedEventArgs, Task<bool>> PaySucceed;

        /// <summary>
        /// 网关异步返回的撤销通知验证成功时触发
        /// </summary>
        public event Func<object, CancelSucceedEventArgs, Task<bool>> CancelSucceed;

        /// <summary>
        /// 网关异步返回的退款通知验证成功时触发
        /// </summary>
        public event Func<object, RefundSucceedEventArgs, Task<bool>> RefundSucceed;

        /// <summary>
        /// 网关异步返回的未知通知时触发
        /// </summary>
        public event Func<object, UnKnownNotifyEventArgs, Task<bool>> UnknownNotify;

        /// <summary>
        /// 找不到网关时触发
        /// </summary>
        public event Action<object, UnknownGatewayEventArgs> UnknownGateway;

        #endregion

        #region 方法

        private Task<bool> OnPaySucceed(PaySucceedEventArgs e) => PaySucceed?.Invoke(this, e) ?? Task.FromResult(false);

        private Task<bool> OnCancelSucceed(CancelSucceedEventArgs e) => CancelSucceed?.Invoke(this, e) ?? Task.FromResult(false);

        private Task<bool> OnRefundSucceed(RefundSucceedEventArgs e) => RefundSucceed?.Invoke(this, e) ?? Task.FromResult(false);

        private Task<bool> OnUnknownNotify(UnKnownNotifyEventArgs e) => UnknownNotify?.Invoke(this, e) ?? Task.FromResult(false);

        private void OnUnknownGateway(UnknownGatewayEventArgs e) => UnknownGateway?.Invoke(this, e);

        /// <summary>
        /// 接收并验证网关的支付通知
        /// </summary>
        public async Task<DResult> ReceivedAsync()
        {
            var logger = LogManager.Logger<Notify>();
            var gateway = NotifyProcess.GetGateway(_gatewayFunc);
            var notifyData = string.IsNullOrWhiteSpace(gateway.GatewayData.Raw)
                ? gateway.GatewayData.ToJson()
                : gateway.GatewayData.Raw;
            logger.Debug($"notify:{notifyData}");
            if (gateway is NullGateway)
            {
                OnUnknownGateway(new UnknownGatewayEventArgs(gateway));
                return DResult.Error("未知的支付网关");
            }

            bool result;
            try
            {
                if (!await gateway.ValidateNotifyAsync())
                    throw new GatewayException("签名验证失败");

                if (string.Equals(HttpUtil.RequestType, "GET", StringComparison.CurrentCultureIgnoreCase))
                    result = await OnPaySucceed(new PaySucceedEventArgs(gateway));
                else
                {
                    if (gateway.IsPaySuccess)
                        result = await OnPaySucceed(new PaySucceedEventArgs(gateway));
                    else if (gateway.IsRefundSuccess)
                        result = await OnRefundSucceed(new RefundSucceedEventArgs(gateway));
                    else if (gateway.IsCancelSuccess)
                        result = await OnCancelSucceed(new CancelSucceedEventArgs(gateway));
                    else
                        result = await OnUnknownNotify(new UnKnownNotifyEventArgs(gateway));
                }
                if (result)
                    gateway.WriteSuccessFlag();
                else
                    gateway.WriteFailureFlag();
                return DResult.Success;
            }
            catch (Exception ex)
            {
                var d = DResult.Error(ex.Message);
                switch (ex)
                {
                    case BusiException busi:
                        logger.Warn($"支付回调业务异常：{busi.Code},{busi.Message}");
                        result = true;
                        d.Code = busi.Code;
                        break;
                    case GatewayException _:
                        result = await OnUnknownNotify(new UnKnownNotifyEventArgs(gateway)
                        {
                            Message = ex.Message
                        });
                        break;
                    default:
                        result = false;
                        logger.Error(ex.Message, ex);
                        break;
                }
                if (result)
                    gateway.WriteSuccessFlag();
                else
                    gateway.WriteFailureFlag();

                return d;
            }
        }

        #endregion
    }
}
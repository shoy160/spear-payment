using Acb.Core.Dependency;
using Acb.Core.EventBus;
using Acb.Gateway.Payment.Payment;
using Acb.Payment.Contracts;
using Acb.Payment.Contracts.Dtos;
using Acb.RabbitMq;
using System.Threading.Tasks;

namespace Acb.Gateway.Payment.EventHandler
{
    /// <inheritdoc />
    /// <summary> 支付回调通知 </summary>
    [Subscription("payment_notify")]
    public class PaymentNotifyEventHandler : IEventHandler<TradeDto>
    {
        /// <inheritdoc />
        /// <summary> 事件处理 </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public async Task Handle(TradeDto @event)
        {
            var projectContract = CurrentIocManager.Resolve<IProjectContract>();
            var project = await projectContract.DetailByIdAsync(@event.ProjectId);
            if (project == null || string.IsNullOrWhiteSpace(project.NotifyUrl))
                return;
            await @event.TradeNotify(project.NotifyUrl, project.Secret);
        }
    }
}

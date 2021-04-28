using Acb.Core.Dependency;
using Acb.Core.Monitor;
using Acb.Payment.Contracts;
using Acb.Payment.Contracts.Dtos;
using Acb.Payment.Contracts.Enums;
using System.Threading.Tasks;

namespace Acb.Gateway.Payment.Payment
{
    public class PaymentMonitor : IMonitor
    {
        public async Task RecordAsync(MonitorData data)
        {
            if (data.Service != "payment_notify")
                return;
            var contract = CurrentIocManager.Resolve<ITradeContract>();
            await contract.NotifyAsync(new TradeNotifyInputDto
            {
                Url = data.Url,
                TradeId = null,
                Type = NotifyType.Receive,
                Content = data.Data,
                Result = data.Result
            });
        }
    }
}

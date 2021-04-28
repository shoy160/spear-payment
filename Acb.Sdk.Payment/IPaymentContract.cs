using System.Threading.Tasks;
using Acb.Sdk.Payment.Enums;
using Acb.Sdk.Payment.Models;
using Acb.SdkCore;

namespace Acb.Sdk.Payment
{
    public interface IPaymentContract
    {
        Task<SdkResult<string>> CreateCashier(CashierInput payment);

        Task<SdkResult<string>> Pay(PaymentMode mode, PaymentType type, PayInput paymentParams);

        Task<NotifyModel> NotifyVerify(NotifyModel model = null);
    }
}

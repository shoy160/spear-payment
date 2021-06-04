using System.Threading.Tasks;
using Spear.Sdk.Payment.Enums;
using Spear.Sdk.Payment.Models;
using Spear.SdkCore;

namespace Spear.Sdk.Payment
{
    public interface IPaymentContract
    {
        Task<SdkResult<string>> CreateCashier(CashierInput payment);

        Task<SdkResult<string>> Pay(PaymentMode mode, PaymentType type, PayInput paymentParams);

        Task<NotifyModel> NotifyVerify(NotifyModel model = null);
    }
}

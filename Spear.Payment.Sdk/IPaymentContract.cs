using System.Threading.Tasks;
using Spear.Payment.Sdk.Enums;
using Spear.Payment.Sdk.Models;
using Spear.Sdk.Core.Dtos;

namespace Spear.Payment.Sdk
{
    public interface IPaymentContract
    {
        Task<SdkResult<string>> CreateCashier(CashierInput payment);

        Task<SdkResult<string>> Pay(PaymentMode mode, PaymentType type, PayInput paymentParams);

        Task<NotifyModel> NotifyVerify(NotifyModel model = null);
    }
}

#if NETSTANDARD2_0
using System;
using System.Threading.Tasks;
using Acb.Sdk.Payment.Services;
using Acb.SdkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Acb.Sdk.Payment
{
    public static class ServiceCollectionExtensions
    {
        /// <summary> 添加支付模块 </summary>
        /// <param name="services"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddPayment(this IServiceCollection services,
            Action<PaymentOptions> optionsAction = null, Func<SdkRequestData, Task> errorAction = null)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IPaymentContract>(provider =>
            {
                var accessor = provider.GetService<IHttpContextAccessor>();
                var options = new PaymentOptions();
                optionsAction?.Invoke(options);
                var service = new PaymentService(options, accessor);
                if (errorAction != null)
                    service.OnError += data => errorAction(data);
                return service;
            });
            return services;
        }
    }
}
#endif

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Spear.Payment.Core.Gateways;
using Spear.Payment.Core.Utils;
using System;

namespace Spear.Payment.Core
{
    public static class ServiceCollectionExtensions
    {
        /// <summary> 添加Payment </summary>
        /// <param name="services"></param>
        /// <param name="setupAction"></param>
        public static void AddPayment(this IServiceCollection services, Action<IGateways> setupAction)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<IGateways>(a =>
            {
                var gateways = new Gateways.Gateways();
                setupAction(gateways);

                return gateways;
            });
        }

        /// <summary> 使用Payment </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UsePayment(this IApplicationBuilder app)
        {
            var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            HttpUtil.Configure(httpContextAccessor);

            return app;
        }
    }
}
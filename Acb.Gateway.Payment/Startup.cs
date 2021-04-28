using Acb.Core;
using Acb.Core.Domain;
using Acb.Core.EventBus;
using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Gateway.Payment.Payment;
using Acb.RabbitMq;
using Acb.WebApi;
using Acb.WebApi.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using PaySharp.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Acb.Gateway.Payment
{
    public class Startup : DStartup
    {
        public Startup()
        {
        }

        protected override IDictionary<string, string> DocGroups()
        {
            return new Dictionary<string, string>
            {
                {"help", "支付网关接口文档"},
                {"manage", "支付网关接口文档（后台管理）"}
            };
        }

        protected override void MapServices(IServiceCollection services)
        {
            services.AddMonitor(monitorTypes: typeof(PaymentMonitor));
            services.AddRabbitMqEventBus();
            base.MapServices(services);
        }

        protected override void UseServices(IServiceProvider provider)
        {
            if (Consts.Mode != ProductMode.Dev)
                provider.SubscribeAt();
            base.UseServices(provider);
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            ExceptionHandler.OnException += e =>
            {
                if (e is ArgumentException argException)
                {
                    throw new BusiException(argException.Message, ErrorCodes.ParamaterError);
                }
            };

            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
                EnableDefaultFiles = true,
                DefaultFilesOptions = { DefaultFileNames = new[] { "index.html" } }
            });
            app.UseMiddleware<CoreVersionMiddleWare>();
            app.UsePaySharp();
            //app.UseStaticFiles();
            app.UseCors(builder => builder.SetIsOriginAllowed(t => true).AllowAnyHeader().AllowAnyMethod());
            base.Configure(app, env);
        }
    }

    public class CoreVersionMiddleWare
    {
        private readonly RequestDelegate _next;

        public CoreVersionMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            httpContext.Response.Headers.Add("core-version",
                RuntimeInformation.FrameworkDescription.Replace(".NET Core ", string.Empty));
            httpContext.Response.Headers.Add("fw-version", Consts.Version);
            await _next(httpContext);
        }
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using NSwag.AspNetCore;
using PaySharp.Core;
using Spear.Core;
using Spear.Core.Data;
using Spear.Core.EventBus;
using Spear.Core.Exceptions;
using Spear.Framework;
using Spear.RabbitMq;
using Spear.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Spear.Gateway.Payment
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

        protected override void SwaggerUiSetting(SwaggerUiSettings setting)
        {
            setting.CustomJavaScriptPath = "/swagger/payment/swagger.js";
            setting.CustomStylesheetPath = "/swagger/payment/swagger.css";
        }

        protected override void MapServices(IServiceCollection services)
        {
            //services.AddMonitor(monitorTypes: typeof(PaymentMonitor));
            DbConnectionManager.AddAdapter(new Spear.Dapper.Mysql.MySqlConnectionAdapter());
            services.AddRabbitMqEventBus();
            base.MapServices(services);
        }

        protected override void UseServices(IServiceProvider provider)
        {
            if (Constants.Mode != ProductMode.Dev)
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
            app.UseFileServer(new FileServerOptions
            {
                RequestPath = new PathString("/swagger/payment"),
                FileProvider = new EmbeddedFileProvider(typeof(Startup).GetTypeInfo().Assembly, "Spear.Payment.Content")
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
            httpContext.Response.Headers.Add("fw-version", "3.1.2");
            await _next(httpContext);
        }
    }
}

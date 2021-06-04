using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Spear.Core.Extensions;
using Spear.Sdk.Payment;
using Spear.Sdk.Payment.Enums;
using Spear.Sdk.Payment.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Spear.Payment.Tests
{
    [TestClass]
    public class SdkTest : DTest
    {
        protected override void MapServices(IServiceCollection services)
        {
            services.AddPayment(options =>
            {
                options.Gateway = "http://localhost:25859";
                options.Code = "hainan-local";
                options.Secret = "56f4568de9698878";
            }, data =>
            {
                if (data.Exception != null)
                    Print(data.Exception.Format());
                data.Exception = null;
                Print(data);
                return Task.CompletedTask;
            });
            base.MapServices(services);
        }

        [TestMethod]
        public async Task CreateCashierTest()
        {
            var helper = Resolve<IPaymentContract>();
            var result = await helper.CreateCashier(new CashierInput(1, "TO2020090300001")
            {
                Title = "收银台订单支付",
                RedirectUrl = "http://www.baidu.com",
                //                Scan = true
            });
            Print(result);
        }

        [TestMethod]
        public async Task PaymentTest()
        {
            var helper = Resolve<IPaymentContract>();
            var result = await helper.Pay(PaymentMode.Wechat, PaymentType.App, new PayInput
            {
                Amount = 1,
                OrderNo = "O1545721939126",
                Title = "中铁建-充值订单"
                //SceneInfo = new SceneInfo("凡车汇订单支付", "")
            });
            Print(result);
        }

        [TestMethod]
        public void VerifyTest()
        {
            var helper = Resolve<IPaymentContract>();
            var model = helper.NotifyVerify(new NotifyModel
            {
                OrderNo = "MT1542004598016",
                Amount = 1,
                Mode = PaymentMode.Wechat,
                Type = PaymentType.Scan,
                Status = TradeStatus.Paid,
                TradeNo = "4200000224201811122503359098",
                Sign = "1542074354fb0f7139894096e2130e1b64c51cc8b2"
            });
            Print(model);
        }

        [TestMethod]
        public void RawDataTest()
        {
            var path = "D:\\Docs\\海南环岛\\文档\\certificate\\apiclient_cert.p12";
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            var cert = new X509Certificate2(path, "1377779702");
            var buffer = File.ReadAllBytes(path);
            var base64 = Convert.ToBase64String(buffer);
            Print(base64);
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;
            return false;
        }
    }
}

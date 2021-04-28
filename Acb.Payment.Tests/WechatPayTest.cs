using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaySharp.Wechatpay;
using PaySharp.Wechatpay.Request;

namespace Acb.Payment.Tests
{
    [TestClass]
    public class WechatPayTest : DTest
    {
        private WechatpayGateway gateway;

        public WechatPayTest()
        {
            var merchant = new Merchant()
            {
                MchId = "1491231192",
                AppId = "wx615260566af104db",
                AppSecret = "PlbYvz59xfDBdEzkg5DLX5yDgI7oor6V",
                Key = "PlbYvz59xfDBdEzkg5DLX5yDgI7oor6V",
                NotifyUrl = "http://iapp.i-cbao.com/notify/payment"
            };
            gateway = new WechatpayGateway(merchant);
        }

        [TestMethod]
        public void SandboxTest()
        {
            SandboxKeyRequest request = new SandboxKeyRequest();
            var response = gateway.Execute(request);
            Print(response);
        }
    }
}

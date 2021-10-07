using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spear.Payment.Wechat;
using Spear.Payment.Wechat.Request;

namespace Spear.Payment.Tests
{
    [TestClass]
    public class WechatPayTest : DTest
    {
        private readonly WechatGateway _gateway;

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
            _gateway = new WechatGateway(merchant);
        }

        [TestMethod]
        public void SandboxTest()
        {
            var request = new SandboxKeyRequest();
            var response = _gateway.Execute(request);
            Print(response);
        }
    }
}

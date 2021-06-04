using System;
using System.Threading.Tasks;
using Spear.Payment.Contracts;
using Spear.Payment.Contracts.Dtos;
using Spear.Payment.Contracts.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Spear.Payment.Tests
{
    [TestClass]
    public class TradeServiceTest : DTest
    {
        private readonly ITradeContract _tradeContract;

        public TradeServiceTest()
        {
            _tradeContract = Resolve<ITradeContract>();
        }

        [TestMethod]
        public async Task CreateTest()
        {
            var result = await _tradeContract.CreateAsync(new TradeInputDto());
            Assert.AreNotEqual(result, null);
            Print(result);
        }

        [TestMethod]
        public async Task DetailTest()
        {
            var result = await _tradeContract.DetailAsync("4e37bae3-4b0c-c300-90f1-08d632a535ca");
            Assert.AreNotEqual(result, null);
            Print(result);
        }

        [TestMethod]
        public async Task QueryTest()
        {
            var result = await _tradeContract.QueryAsync(new TradeSearchInputDto
            {
                Page = 1,
                Size = 10,
                ChannelId = "882752fd-e41c-c675-bb9b-08d62dfc7ea2",
                ProjectId = "55f020eb-4bb1-c842-7219-08d62f809717",
                Begin = new DateTime(2018, 09, 01),
                End = new DateTime(2018, 11, 01),
                PaidTimeBegin = new DateTime(2018, 09, 01),
                PaidTimeEnd = new DateTime(2018, 11, 01),
                TradeNo = "TO2018101100005",
                OrderNo = "TO2018101100005",
                OutTradeNo = "4200000187201810174063809477",
                Mode = PaymentMode.Alipay,
                Status = TradeStatus.Paid
            });
            Print(result);
        }

        [TestMethod]
        public async Task NotifyListTest()
        {
            var result = await _tradeContract.NotifyListAsync(1, 10, NotifyType.Receive, "123456");
            Print(result);
        }

        [TestMethod]
        public async Task StatisticTest()
        {
            var dto = await _tradeContract.StatisticAsync();
            Print(dto);
        }

    }
}

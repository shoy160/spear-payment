using Acb.Core;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Core.Timing;
using Acb.Payment.Contracts;
using Acb.Payment.Contracts.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acb.Gateway.Payment.Areas.Manage.Controllers
{
    [Route("[area]/home")]
    public class HomeController : DController
    {
        private readonly ITradeContract _tradeContract;

        public HomeController(ITradeContract tradeContract)
        {
            _tradeContract = tradeContract;
        }

        private static List<StatisticPlatform> MockTrade(int days)
        {
            var list = new List<StatisticPlatform>();
            var rand = RandomHelper.Random();
            for (var i = days - 1; i >= 0; i--)
            {
                var date = Clock.Now.AddDays(-i);
                var item = new StatisticPlatform
                {
                    Date = date.ToString("yyyy-MM-dd"),
                    AlipayCount = rand.Next(200),
                    AlipayAmount = rand.Next(500000),
                    WechatCount = rand.Next(200),
                    WechatAmount = rand.Next(600000)
                };
                list.Add(item);
            }
            return list;
        }

        private static int NextNumber(int max, int min = 0)
        {
            return RandomHelper.Random().Next(min, max);
        }

        /// <summary> 交易统计 </summary>
        /// <returns></returns>
        [HttpGet("statistic")]
        public async Task<DResult<HomeStatisticDto>> Statistic()
        {
            if ("const:enableStatistic".Config(false))
            {
                var dto = await _tradeContract.StatisticAsync(Client.ProjectId);
                return Succ(dto);
            }
            //模拟数据
            var result = new HomeStatisticDto
            {
                Statistic = new TradeStatistic
                {
                    TodayCount = NextNumber(200),
                    TodayAmount = NextNumber(500000),
                    Count = NextNumber(5000, 200),
                    Amount = NextNumber(50000000, 500000)
                },
                Platforms = MockTrade(10)
            };
            return await Task.FromResult(DResult.Succ(result));
        }
    }
}

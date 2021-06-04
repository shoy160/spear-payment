using System.Collections.Generic;

namespace Spear.Payment.Contracts.Dtos
{
    public class HomeStatisticDto
    {
        public TradeStatistic Statistic { get; set; }
        public List<StatisticPlatform> Platforms { get; set; }
    }

    public class StatisticPlatform
    {
        public string Date { get; set; }
        public int AlipayCount { get; set; }
        public long AlipayAmount { get; set; }
        public int WechatCount { get; set; }
        public long WechatAmount { get; set; }
    }

    public class TradeStatistic
    {
        public int TodayCount { get; set; }
        public long TodayAmount { get; set; }
        public int Count { get; set; }
        public long Amount { get; set; }
    }
}

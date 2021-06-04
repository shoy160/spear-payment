using Spear.Core;
using Spear.Core.Data;
using Spear.Core.Extensions;
using Spear.Core.Timing;
using Spear.Dapper;
using Spear.Dapper.Domain;
using Spear.Payment.Business.Domain.Entities;
using Spear.Payment.Contracts.Dtos;
using Spear.Payment.Contracts.Enums;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeStatus = Spear.Payment.Contracts.Enums.TradeStatus;

namespace Spear.Payment.Business.Domain.Repositories
{
    public class TradeRepository : DapperRepository<TTrade>
    {
        public async Task<TTrade> QueryByOrderNo(string projectId, string orderNo)
        {
            var type = typeof(TTrade);
            var sql =
                $"select {type.Columns()} FROM [{type.PropName()}] WHERE [project_id]=@projectId AND [order_no]=@orderNo";
            sql = Connection.FormatSql(sql);
            return await Connection.QueryFirstOrDefaultAsync<TTrade>(sql, new { projectId, orderNo });
        }

        public async Task<int> UpdateTradeAsync(TTrade model, string[] fields = null)
        {
            // 金额 & title变更之后会提示商户订单号重复~
            //nameof(TTrade.Amount), nameof(TTrade.Title),
            return await TransConnection.UpdateAsync(model, fields, Trans);
        }

        public async Task<int> UpdatePlatformAsync(string id, string platformId)
        {
            return await Connection.UpdateAsync(new TTrade
            {
                Id = id,
                PlatformId = platformId
            }, new[] { nameof(TTrade.PlatformId) }, Trans);
        }

        /// <summary> 修改支付方式 </summary>
        /// <param name="id"></param>
        /// <param name="mode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<int> ChangeMode(string id, PaymentMode mode, string type)
        {
            return await Connection.UpdateAsync(new TTrade
            {
                Id = id,
                Mode = (byte)mode,
                Type = type
            }, new[] { nameof(TTrade.Mode), nameof(TTrade.Type) }, Trans);
        }

        public async Task<int> Paid(TTrade model)
        {
            return await Connection.UpdateAsync(model,
                new[]
                {
                    nameof(TTrade.Status), nameof(TTrade.PaidUser), nameof(TTrade.PaidAccount), nameof(TTrade.PaidTime),
                    nameof(TTrade.OutTradeNo), nameof(TTrade.Mode)
                }, Trans);
        }

        public async Task Notify(TTradeNotify model)
        {
            await Connection.InsertAsync(model);
        }

        public async Task<PagedList<TTrade>> QueryAsync(TradeSearchInputDto dto)
        {
            var mode = typeof(TTrade);
            SQL sql = $"SELECT {mode.Columns()} from [{mode.PropName()}] WHERE 1=1";
            if (dto.Mode.HasValue)
                sql += "AND [mode]=@Mode";
            if (dto.Status.HasValue)
                sql += "AND [status]=@Status";
            if (!string.IsNullOrWhiteSpace(dto.ChannelId))
                sql += "AND [channel_id]=@ChannelId";
            if (!string.IsNullOrWhiteSpace(dto.ProjectId))
                sql += "AND [project_id]=@ProjectId";
            if (!string.IsNullOrEmpty(dto.TradeNo))
                sql += "AND ([trade_no]=@TradeNo OR [order_no]=@TradeNo)";
            if (!string.IsNullOrEmpty(dto.OrderNo))
                sql += "AND [order_no]=@OrderNo";
            if (!string.IsNullOrEmpty(dto.OutTradeNo))
                sql += "AND [out_trade_no]=@OutTradeNo";
            if (dto.PaidTimeBegin.HasValue)
                sql += "AND [paid_time]>=@PaidTimeBegin";
            if (dto.PaidTimeEnd.HasValue)
                sql += "AND [paid_time]<@PaidTimeEnd";
            if (dto.Begin.HasValue)
                sql += "AND [create_time]>=@Begin";
            if (dto.End.HasValue)
                sql += "AND [create_time]<@End";
            sql += "ORDER BY [create_time] DESC";
            var fsql = Connection.FormatSql(sql.ToString());
            return await Connection.PagedListAsync<TTrade>(fsql, dto.Page, dto.Size, dto);
        }

        public async Task<PagedList<TradeNotifyDto>> NotifyListAsync(int page, int size,
            NotifyType? type, string tradeNo, string projectId)
        {
            var tradeType = typeof(TTradeNotify);
            SQL sql =
                $"SELECT {tradeType.Columns(tableAlias: "tn")},t.[trade_no] as [TradeNo] FROM [{tradeType.PropName()}] tn LEFT JOIN [t_trade] t ON t.[id]=tn.[trade_id] WHERE 1=1";
            if (type.HasValue)
            {
                sql += "AND tn.[type] = @type";
            }

            if (!string.IsNullOrWhiteSpace(tradeNo))
            {
                sql += "AND (t.[trade_no]=@tradeNo OR t.[order_no]=@tradeNo)";
            }

            if (!string.IsNullOrWhiteSpace(projectId))
            {
                sql += "AND t.[project_id]=@projectId";
            }

            sql += "ORDER BY tn.[create_time] DESC";

            return await sql.PagedListAsync<TradeNotifyDto>(Connection, page, size, new { type, tradeNo, projectId });
        }

        public async Task<HomeStatisticDto> StatisticAsync(string projectId, int days)
        {
            var today = Clock.Now.Date;
            var minDate = today.AddDays(1 - days);
            var condition = string.Empty;
            if (!string.IsNullOrWhiteSpace(projectId))
                condition = " AND [project_id]=@projectId";
            var sql =
                $"SELECT COALESCE(SUM([amount]),0) AS [amount],COALESCE(COUNT(1),0) AS [count] FROM [t_trade] WHERE [status]=@status{condition};" +
                $"SELECT COALESCE(SUM([amount]),0) AS [amount],COALESCE(COUNT(1),0) AS [count] FROM [t_trade] WHERE [status]=@status{condition} AND [create_time] > @today;" +
                "SELECT DATE_FORMAT(create_time, '%Y-%m-%d') AS [date],COALESCE(SUM(CASE [mode] WHEN 0 THEN 1 ELSE 0 END),0) AS [alipayCount]," +
                "COALESCE(SUM(CASE [mode] WHEN 0 THEN [amount] ELSE 0 END),0) AS [alipayAmount]," +
                "COALESCE(SUM(CASE [mode] WHEN 1 THEN 1 ELSE 0 END),0) AS [wechatCount]," +
                $"COALESCE(SUM(CASE [mode] WHEN 1 THEN [amount] ELSE 0 END),0) AS [wechatAmount] FROM [t_trade] WHERE STATUS = @status{condition} AND create_time > @minDate GROUP BY [date]; ";
            //var sql =
            //    $"SELECT COALESCE(SUM([amount]),0) AS [amount],COALESCE(COUNT(1),0) AS [count] FROM [t_trade] WHERE [status]=@status{condition};" +
            //    $"SELECT COALESCE(SUM([amount]),0) AS [amount],COALESCE(COUNT(1),0) AS [count] FROM [t_trade] WHERE [status]=@status{condition} AND [create_time] > @today;" +
            //    "SELECT TO_CHAR(create_time, 'YYYY-MM-DD') AS [date],COALESCE(SUM(CASE [mode] WHEN 0 THEN 1 ELSE 0 END),0) AS [alipayCount]," +
            //    "COALESCE(SUM(CASE [mode] WHEN 0 THEN [amount] ELSE 0 END),0) AS [alipayAmount]," +
            //    "COALESCE(SUM(CASE [mode] WHEN 1 THEN 1 ELSE 0 END),0) AS [wechatCount]," +
            //    $"COALESCE(SUM(CASE [mode] WHEN 1 THEN [amount] ELSE 0 END),0) AS [wechatAmount] FROM [t_trade] WHERE STATUS = @status{condition} AND create_time > @minDate GROUP BY [date]; ";
            var dto = new HomeStatisticDto
            {
                Statistic = new TradeStatistic(),
                Platforms = new List<StatisticPlatform>()
            };
            var fsql = Connection.FormatSql(sql);
            var muli = await Connection.QueryMultipleAsync(fsql, new
            {
                projectId,
                today,
                minDate,
                status = TradeStatus.Paid
            });
            using (muli)
            {
                var t = await muli.ReadFirstAsync();
                dto.Statistic.Count = (int)t.count;
                dto.Statistic.Amount = (long)t.amount;
                t = await muli.ReadFirstAsync();
                dto.Statistic.TodayCount = (int)t.count;
                dto.Statistic.TodayAmount = (long)t.amount;
                var flatforms = await muli.ReadAsync<StatisticPlatform>();
                dto.Platforms = (flatforms ?? new List<StatisticPlatform>()).ToList();
            }

            return dto;
        }
    }
}

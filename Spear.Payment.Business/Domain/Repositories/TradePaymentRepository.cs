using Spear.Core.Data;
using Spear.Core.Extensions;
using Spear.Dapper.Domain;
using Spear.Payment.Business.Domain.Entities;
using Spear.Payment.Contracts.Enums;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spear.Dapper;

namespace Spear.Payment.Business.Domain.Repositories
{
    public class TradePaymentRepository : DapperRepository<TTradePayment>
    {
        private readonly Type _modeType = typeof(TTradePayment);

        public Task<int> InsertAsync(TTradePayment mode)
        {
            return Transaction(async (conn, trans) =>
            {
                var count = await conn.DeleteWhereAsync<TTradePayment>(
                    "[trade_id]=@tradeId AND [mode]=@mode AND [type]=@type", mode, trans);
                count += await conn.InsertAsync(mode, trans: trans);
                return count;
            });
        }

        public async Task<string> QueryByModeAndTypeAsync(string tradeId, PaymentMode mode, string type)
        {
            var sql =
                $"SELECT [value] FROM [{_modeType.PropName()}] WHERE [trade_id]=@tradeId AND [mode]=@mode AND [type]=@type LIMIT 1";
            sql = Connection.FormatSql(sql);
            return await Connection.QueryFirstOrDefaultAsync<string>(sql,
                new { tradeId, mode, type });
        }

        public async Task<Dictionary<PaymentMode, string>> QueryByTypeAsync(string tradeId, string type)
        {
            var sql =
                $"SELECT [mode],[value] FROM [{_modeType.PropName()}] WHERE [trade_id]=@tradeId AND [type]=@type";
            sql = Connection.FormatSql(sql);
            var models = await Connection.QueryAsync<TTradePayment>(sql, new { tradeId, type });
            return models.GroupBy(t => t.Mode).ToDictionary(k => (PaymentMode)k.Key, v => v.First().Value);
        }
    }
}

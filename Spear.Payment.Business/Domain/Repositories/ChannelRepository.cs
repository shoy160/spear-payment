using Spear.Core.Data;
using Spear.Core.Extensions;
using Spear.Dapper;
using Spear.Dapper.Domain;
using Spear.Payment.Business.Domain.Entities;
using Spear.Payment.Contracts.Enums;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spear.Core;

namespace Spear.Payment.Business.Domain.Repositories
{
    public class ChannelRepository : DapperRepository<TChannel>
    {
        public Task<bool> ExistsAppId(PaymentMode mode, string appId)
        {
            return Connection.ExistsWhereAsync<TChannel>("[mode]=@mode AND [app_id]=@appId", new { mode, appId });
        }

        public Task<int> InsertAsync(TChannel model)
        {
            var modelType = typeof(TChannel);

            var sql = modelType.InsertSql();//.Replace("@Config", "@Config::json").Replace("@Types", "@Types::json");
            sql = Connection.FormatSql(sql);
            return Connection.ExecuteAsync(sql, model);
        }

        public Task<int> UpdateAsync(TChannel model)
        {
            var modelType = typeof(TChannel);
            SQL sql =
                $"UPDATE [{modelType.PropName()}] SET";
            var props = new List<string>();
            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                props.Add("[name]=@Name");
            }
            if (!string.IsNullOrWhiteSpace(model.Remark))
            {
                props.Add("[remark]=@Remark");
            }

            if (!string.IsNullOrWhiteSpace(model.Config))
            {
                //props.Add("[config] = @Config::json");
                props.Add("[config] = @Config");
            }
            if (!string.IsNullOrWhiteSpace(model.AppId))
            {
                props.Add("[app_id] = @AppId");
            }
            //props.Add("[types] = @Types::json");
            props.Add("[types] = @Types");

            sql += string.Join(",", props);
            sql += "WHERE[id] = @Id";
            var fsql = Connection.FormatSql(sql.ToString());
            return Connection.ExecuteAsync(fsql, model, Trans);
        }

        public async Task<int> UpdateIsDefaultAsync(string id, byte mode, bool isDefault)
        {
            var sql0 = "update [t_channel] set [is_default]=false where [mode]=@mode;";
            var sql1 = "update [t_channel] set [is_default]=@isDefault where [id]=@id;";
            var count = 0;
            if (isDefault)
                count += await Connection.ExecuteAsync(Connection.FormatSql(sql0), new { mode }, Trans);
            count += await Connection.ExecuteAsync(Connection.FormatSql(sql1), new { id, isDefault }, Trans);
            return count;
        }

        public async Task<int> UpdateStatusAsync(string id, ChannelStatus status)
        {
            return await Connection.UpdateAsync(new TChannel
            {
                Id = id,
                Status = (byte)status
            }, new[] { nameof(TChannel.Status) });
        }


        public Task<IEnumerable<TChannel>> QueryByIdsAsync(IEnumerable<string> ids)
        {
            var sql = Select("[id] IN @ids");
            sql = Connection.FormatSql(sql);
            return Connection.QueryAsync<TChannel>(sql, new { ids = ids.ToArray() });
        }

        public Task<PagedList<TChannel>> QueryAsync(string keyword, PaymentMode? mode, ChannelStatus? status, int page, int size)
        {
            SQL sql = Select("1=1");
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sql += "AND ([app_id]=@keyword OR [name] like @likeword)";
            }
            if (mode.HasValue)
            {
                sql += "AND [mode]=@mode";
            }
            if (status.HasValue)
            {
                sql += "AND [status]=@status";
            }
            else
            {
                sql += "AND [status]<>4";
            }
            sql += "ORDER BY [create_time] DESC";
            return sql.PagedListAsync<TChannel>(Connection, page, size, new
            {
                keyword,
                likeword = $"%{keyword}%",
                mode,
                status
            });
        }

        public Task<IEnumerable<TChannel>> QueryDefaultsAsync()
        {
            SQL sql = Select("[is_default]=true");
            var fsql = Connection.FormatSql(sql.ToString());
            return Connection.QueryAsync<TChannel>(fsql);
        }

        public Task<TChannel> QueryByAppIdAsync(string appId)
        {
            SQL sql =
                Select("([app_id]=@appId OR (config ->> '$.appletId') = @appId OR (config ->> '$.publicAppId')=@appId) AND [status]=@status");
            var fsql = Connection.FormatSql(sql.ToString());
            return Connection.QueryFirstOrDefaultAsync<TChannel>(fsql, new
            {
                appId,
                status = ChannelStatus.Enable
            });
        }
    }
}

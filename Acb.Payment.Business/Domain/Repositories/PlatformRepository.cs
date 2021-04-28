using Acb.Core.Data;
using Acb.Dapper;
using Acb.Dapper.Domain;
using Acb.Payment.Business.Domain.Entities;
using Acb.Payment.Contracts.Dtos;
using Dapper;
using System;
using System.Threading.Tasks;

namespace Acb.Payment.Business.Domain.Repositories
{
    public class PlatformRepository : DapperRepository<TPlatform>
    {
        public Task<string> ExistsAsync(string channelId, string openId)
        {
            const string sql = "SELECT [id] FROM [t_platform] WHERE [channel_id]=@channelId AND [open_id]=@openId";
            var fsql = Connection.FormatSql(sql);
            return Connection.QueryFirstOrDefaultAsync<string>(fsql, new { channelId, openId });
        }

        public Task<int> UpdateAsync(string id, PlatformDto dto)
        {
            return Connection.UpdateAsync(new TPlatform
            {
                Id = id,
                AccessToken = dto.AccessToken,
                RefreshToken = dto.RefreshToken,
                ExpireIn = dto.ExpireIn
            }, new[] { nameof(TPlatform.AccessToken), nameof(TPlatform.RefreshToken), nameof(TPlatform.ExpireIn) },
                Trans);
        }
    }
}

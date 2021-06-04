using Spear.Core.Data;
using Spear.Dapper;
using Spear.Dapper.Domain;
using Spear.Payment.Business.Domain.Entities;
using Spear.Payment.Contracts.Dtos;
using Dapper;
using System;
using System.Threading.Tasks;

namespace Spear.Payment.Business.Domain.Repositories
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

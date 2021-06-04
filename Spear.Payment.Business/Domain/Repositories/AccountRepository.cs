using Spear.Core;
using Spear.Core.Extensions;
using Spear.Core.Timing;
using Spear.Dapper;
using Spear.Dapper.Domain;
using Spear.Payment.Business.Domain.Entities;
using Spear.Payment.Contracts.Enums;
using System;
using System.Threading.Tasks;

namespace Spear.Payment.Business.Domain.Repositories
{
    public class AccountRepository : DapperRepository<TAccount>
    {
        private readonly Type _modelType = typeof(TAccount);
        /// <summary> 查询账号 </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public Task<TAccount> QueryByAccountAsync(string account)
        {
            return Connection.QueryByIdAsync<TAccount>(account, "account");
        }

        /// <summary> 更新密码 </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public Task<int> UpdatePasswordAsync(string id, string password, string salt)
        {
            return Connection.UpdateAsync(new TAccount
            {
                Id = id,
                Password = password,
                PasswordSalt = salt
            }, new[] { nameof(TAccount.Password), nameof(TAccount.PasswordSalt) });
        }

        /// <summary> 更新登陆时间 </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<int> UpdateLoginTimeAsync(string id)
        {
            return Connection.UpdateAsync(new TAccount
            {
                Id = id,
                LastLoginTime = Clock.Now
            }, new[] { nameof(TAccount.LastLoginTime) });
        }

        public Task<PagedList<TAccount>> QueryByStatusAsync(CommonStatus? status, int page, int size)
        {
            var columns = _modelType.Columns(excepts: new[] { nameof(TAccount.Password), nameof(TAccount.PasswordSalt) });
            SQL sql = $"SELECT {columns} FROM [{_modelType.PropName()}] WHERE 1=1";
            if (status.HasValue)
            {
                sql += "AND [status]=@status";
            }

            sql += "ORDER BY [create_time] DESC";
            return sql.PagedListAsync<TAccount>(Connection, page, size, new { status });
        }
    }
}

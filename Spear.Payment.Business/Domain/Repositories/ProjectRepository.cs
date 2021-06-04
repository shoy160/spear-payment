using Spear.AutoMapper;
using Spear.Core;
using Spear.Core.Data;
using Spear.Core.Extensions;
using Spear.Core.Serialize;
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

namespace Spear.Payment.Business.Domain.Repositories
{
    public class ProjectRepository : DapperRepository<TProject>
    {
        private readonly Type _modelType = typeof(TProject);

        public async Task<int> InsertAsync(TProject model)
        {
            var sql = _modelType.InsertSql();
            //sql = sql.Replace("@Channels", "@Channels::json");
            sql = Connection.FormatSql(sql);
            return await Connection.ExecuteAsync(sql, model);
        }

        public async Task<int> UpdateAsync(TProject model)
        {
            var modelType = typeof(TProject);
            SQL sql =
                $"UPDATE [{modelType.PropName()}] SET";
            var props = new List<string>();
            //if (!string.IsNullOrWhiteSpace(model.Secret))
            //{
            //    props.Add("[secret]=@Secret");
            //}
            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                props.Add("[name]=@Name");
            }
            if (!string.IsNullOrWhiteSpace(model.NotifyUrl))
            {
                props.Add("[notify_url] = @NotifyUrl");
            }
            if (!string.IsNullOrWhiteSpace(model.RedirectUrl))
            {
                props.Add("[redirect_url] = @RedirectUrl");
            }
            if (!string.IsNullOrWhiteSpace(model.QueueName))
            {
                props.Add("[queue_name] = @QueueName");
            }
            if (!string.IsNullOrWhiteSpace(model.Channels))
            {
                //props.Add("[channels] = @Channels::json");
                props.Add("[channels] = @Channels");
            }
            sql += string.Join(",", props);
            sql += "WHERE [id] = @Id";
            var fsql = Connection.FormatSql(sql.ToString());
            return await Connection.ExecuteAsync(fsql, model, Trans);
        }

        public Task<int> UpdateSecretAsync(string id, string secret)
        {
            return Connection.UpdateAsync(new TProject
            {
                Id = id,
                Secret = secret
            }, new[] { nameof(TProject.Secret) });
        }

        public Task<IEnumerable<TProject>> QueryByIdsAsync(IEnumerable<string> ids)
        {
            var modelType = typeof(TProject);
            var sql = $"SELECT {modelType.Columns()} FROM [{modelType.PropName()}] WHERE [id] IN @ids";
            sql = Connection.FormatSql(sql);
            return Connection.QueryAsync<TProject>(sql, new { ids = ids.ToArray() });
        }

        public Task<PagedList<TProject>> QueryAsync(string keyword, CommonStatus? status, int page, int size)
        {
            var mode = typeof(TProject);
            SQL sql = $"SELECT {mode.Columns()} from [{mode.PropName()}] WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sql += "AND ([code]=@keyword OR [name] like @likeword)";
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
            return sql.PagedListAsync<TProject>(Connection, page, size, new
            {
                keyword,
                likeword = $"%{keyword}%",
                status
            });
        }

        /// <summary> 根据项目编号获取项目信息 </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<ProjectDto> QueryByCode(string code)
        {
            var model = await QueryByIdAsync(code, "code");
            var dto = model.MapTo<ProjectDto>();
            dto.Channels = JsonHelper.JsonList<string>(model.Channels);
            return dto;
        }

        public Task<bool> ExistsAsync(string code)
        {
            return Connection.ExistsAsync<TProject>("code", code);
        }
    }
}

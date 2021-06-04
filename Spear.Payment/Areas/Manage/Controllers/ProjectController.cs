using Spear.AutoMapper;
using Spear.Core;
using Spear.Core.Extensions;
using Spear.Gateway.Payment.Areas.Manage.Models;
using Spear.Payment.Contracts;
using Spear.Payment.Contracts.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spear.Gateway.Payment.Areas.Manage.Filters;
using Spear.Payment.Contracts.Enums;

namespace Spear.Gateway.Payment.Areas.Manage.Controllers
{
    /// <summary>
    /// 支付项目
    /// </summary>
    [Route("[area]/project")]
    public class ProjectController : DController
    {
        private readonly IProjectContract _contract;
        private readonly IChannelContract _channelContract;

        public ProjectController(IProjectContract contract, IChannelContract channelContract)
        {
            _contract = contract;
            _channelContract = channelContract;
        }

        /// <summary> 添加项目 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost, DRole(AccountRole.Admin)]
        public async Task<DResult> Post([FromBody] VProjectInput input)
        {
            var dto = input.MapTo<ProjectInputDto>();
            return FormatResult(await _contract.CreateAsync(dto));
        }

        /// <summary>
        /// 编辑项目
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<DResult> Put(string id, [FromBody] VProjectInput input)
        {
            var dto = input.MapTo<ProjectDto>();
            return FormatResult(await _contract.SetAsync(id, dto));
        }

        /// <summary> 更新项目密钥 </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("secret/{id}")]
        public async Task<DResult> SetSecret(string id)
        {
            return FormatResult(await _contract.SetSecretAsync(id));
        }

        /// <summary>
        /// 删除项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}"), DRole(AccountRole.Admin)]
        public async Task<DResult> Delete(string id)
        {
            return FormatResult(await _contract.RemoveAsync(id));
        }

        /// <summary> 项目列表 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DResults<VProject>> List(VProjectListInput input)
        {
            if (!string.IsNullOrWhiteSpace(Client.ProjectId))
            {
                var dto = await _contract.DetailByIdAsync(Client.ProjectId);
                var project = dto.MapTo<VProject>();
                project.ChannelModels = (await _channelContract.DetailsAsync(dto.Channels)).MapTo<VChannel>();
                return Succ(new List<VProject>
                {
                    project
                }, 1);
            }
            var dtos = await _contract.QueryAsync(input.Keyword, input.Status, input.Page, input.Size);
            var data = dtos.MapPagedList<VProject, ProjectDto>();
            var channelIds = data.List.SelectMany(i => i.Channels).ToList();
            if (!channelIds.Any())
                return DResult.Succ(data);

            var channels = await _channelContract.DetailsAsync(channelIds);
            data.List.Foreach(item => item.ChannelModels = channels.Where(i => item.Channels.Contains(i.Id)).MapTo<VChannel>());

            return DResult.Succ(data);
        }

    }
}

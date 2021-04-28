using Acb.AutoMapper;
using Acb.Core;
using Acb.Gateway.Payment.Areas.Manage.Models;
using Acb.Payment.Contracts;
using Acb.Payment.Contracts.Dtos;
using Acb.Payment.Contracts.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Acb.Gateway.Payment.Areas.Manage.Filters;

namespace Acb.Gateway.Payment.Areas.Manage.Controllers
{
    /// <summary>
    /// 支付渠道
    /// </summary>
    [Route("[area]/channel"), DRole(AccountRole.Admin)]
    public class ChannelController : DController
    {
        private readonly IChannelContract _contract;

        public ChannelController(IChannelContract contract)
        {
            _contract = contract;
        }

        /// <summary>
        /// 新增支付渠道
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DResult> Post([FromBody] VChannelInput input)
        {
            var dto = input.MapTo<ChannelInputDto>();
            dto.IsDefault = false;
            return FormatResult(await _contract.CreateAsync(dto));
        }

        /// <summary> 编辑支付渠道 </summary>
        /// <param name="id">渠道ID</param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<DResult> Put(string id, [FromBody] VChannelUpdateInput input)
        {
            var dto = input.MapTo<ChannelInputDto>();
            return FormatResult(await _contract.SetAsync(id, dto));
        }

        /// <summary> 更新状态 </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPut("status/{id}")]
        public async Task<DResult> SetStatus(string id, ChannelStatus status)
        {
            return FormatResult(await _contract.SetStatusAsync(id, status));
        }

        /// <summary>
        /// 设置默认支付渠道
        /// </summary>
        /// <param name="id">渠道ID</param>
        /// <param name="isDefault">是否默认</param>
        /// <returns></returns>
        [HttpPut("default/{id}")]
        public async Task<DResult> SetDefault(string id, bool isDefault)
        {
            return FormatResult(await _contract.SetDefaultAsync(id, isDefault));
        }

        /// <summary> 支付渠道列表 </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DResults<VChannel>> List(VChannelListInput inputDto)
        {
            var list = await _contract.ListAsync(inputDto?.MapTo<ChannelListInputDto>() ?? new ChannelListInputDto());
            return DResult.Succ(list.MapPagedList<VChannel, ChannelDto>());
        }

        /// <summary>
        /// 支付渠道明细
        /// </summary>
        /// <param name="id">渠道ID</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<DResult<VChannel>> Detail(string id)
        {
            var dto = await _contract.DetailAsync(id);
            return dto == null
                ? DResult.Error<VChannel>("支付渠道不存在")
                : DResult.Succ(dto.MapTo<VChannel>());
        }
    }
}

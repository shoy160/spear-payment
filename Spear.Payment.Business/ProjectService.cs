using Spear.AutoMapper;
using Spear.Core;
using Spear.Core.Domain;
using Spear.Core.Exceptions;
using Spear.Core.Extensions;
using Spear.Core.Helper;
using Spear.Core.Serialize;
using Spear.Core.Timing;
using Spear.Payment.Business.Domain.Entities;
using Spear.Payment.Business.Domain.Repositories;
using Spear.Payment.Contracts;
using Spear.Payment.Contracts.Dtos;
using Spear.Payment.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spear.Payment.Business
{
    public class ProjectService : DService, IProjectContract
    {
        private readonly ProjectRepository _projectRepository;
        private readonly IAccountContract _accountContract;
        private readonly IChannelContract _channelContract;


        public ProjectService(ProjectRepository repository, IAccountContract accountContract, IChannelContract channelContract)
        {
            _projectRepository = repository;
            _accountContract = accountContract;
            _channelContract = channelContract;
        }

        public async Task<int> CreateAsync(ProjectInputDto dto)
        {
            if (await _projectRepository.ExistsAsync(dto.Code))
                throw new BusiException("项目编码已存在");
            var model = dto.MapTo<TProject>();
            model.Id = IdentityHelper.Guid32;
            model.Status = (byte)CommonStatus.Normal;
            model.CreateTime = Clock.Now;
            model.Secret = IdentityHelper.Guid16;
            model.Channels = JsonHelper.ToJson(dto.Channels);
            if (dto.Password.IsNotNullOrEmpty())
            {
                await _accountContract.RegistProjectAsync(model.Id, model.Code, dto.Password);
            }
            return await _projectRepository.InsertAsync(model);
        }

        public async Task<ProjectDto> DetailByCodeAsync(string projectCode)
        {
            return await _projectRepository.QueryByCode(projectCode);
        }

        public async Task<ProjectDto> DetailByIdAsync(string id)
        {
            var model = await _projectRepository.QueryByIdAsync(id);
            var dto = model.MapTo<ProjectDto>();
            dto.Channels = JsonHelper.JsonList<string>(model.Channels);
            return dto;
        }

        public async Task<IEnumerable<ProjectDto>> QueryByIdsAsync(IEnumerable<string> ids)
        {
            var models = await _projectRepository.QueryByIdsAsync(ids);
            var list = new List<ProjectDto>();
            models.Foreach(model =>
            {
                var dto = model.MapTo<ProjectDto>();
                dto.Channels = JsonHelper.JsonList<string>(model.Channels);
                list.Add(dto);
            });
            return list;
        }

        public async Task<PagedList<ProjectDto>> QueryAsync(string keyword, CommonStatus? status, int page, int size)
        {
            var models = await _projectRepository.QueryAsync(keyword, status, page, size);
            var list = new List<ProjectDto>();
            models.List.Foreach(model =>
            {
                var dto = model.MapTo<ProjectDto>();
                dto.Channels = JsonHelper.JsonList<string>(model.Channels);
                list.Add(dto);
            });
            return new PagedList<ProjectDto>(list, page, size, models.Total);
        }

        public async Task<int> SetAsync(string id, ProjectDto dto)
        {
            var model = dto.MapTo<TProject>();
            model.Id = id;
            model.Channels = JsonHelper.ToJson(dto.Channels);
            if (dto.Password.IsNotNullOrEmpty())
            {
                await _accountContract.RegistProjectAsync(id, dto.Code, dto.Password);
            }
            return await _projectRepository.UpdateAsync(model);
        }

        public Task<int> SetSecretAsync(string id)
        {
            var secret = IdentityHelper.Guid16;
            return _projectRepository.UpdateSecretAsync(id, secret);
        }

        public Task<int> RemoveAsync(string id)
        {
            return _projectRepository.DeleteAsync(id);
        }

        public async Task<IDictionary<PaymentMode, List<ChannelDto>>> ChannelsAsync(string id)
        {
            var project = await _projectRepository.QueryByIdAsync(id);
            if (project == null)
                return new Dictionary<PaymentMode, List<ChannelDto>>();
            var channelIds = JsonHelper.JsonList<string>(project.Channels ?? "[]").ToArray();

            IEnumerable<ChannelDto> channels;
            if (channelIds.Any())
                channels = await _channelContract.DetailsAsync(channelIds);
            else
                channels = await _channelContract.DefaultsAsync();
            return channels.Where(t => t.Status == ChannelStatus.Enable).GroupBy(t => t.Mode)
                .ToDictionary(k => k.Key, v => v.ToList());
        }

        public async Task<ChannelDto> ChannelAsync(string id, PaymentMode mode, PaymentType? type = null)
        {
            var dict = await ChannelsAsync(id);
            if (!dict.TryGetValue(mode, out var channels))
                return null;
            ChannelDto channel = null;
            if (type.HasValue)
            {
                //支付类型过滤
                channel = channels.FirstOrDefault(t => t.Types.Contains(type.Value));
            }

            if (channel == null)
            {
                //优先所有支付类型的渠道
                channel = channels.FirstOrDefault(t => t.Types == null || t.Types.Length == 0);
            }

            return channel ?? channels.FirstOrDefault();
        }
    }
}

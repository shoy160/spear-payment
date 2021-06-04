using Spear.AutoMapper;
using Spear.Core;
using Spear.Core.Domain;
using Spear.Core.Exceptions;
using Spear.Core.Helper;
using Spear.Core.Serialize;
using Spear.Core.Timing;
using Spear.Payment.Business.Domain.Entities;
using Spear.Payment.Business.Domain.Repositories;
using Spear.Payment.Contracts;
using Spear.Payment.Contracts.Dtos;
using Spear.Payment.Contracts.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spear.Payment.Business
{
    public class ChannelService : DService, IChannelContract
    {
        private readonly ChannelRepository _channelRepository;

        public ChannelService(ChannelRepository channelRepository)
        {
            _channelRepository = channelRepository;
        }

        private static ChannelDto ConvertDto(TChannel model)
        {
            if (model == null) return null;
            var dto = model.MapTo<ChannelDto>();
            dto.Config = JsonHelper.Json<object>(model.Config);
            if (!string.IsNullOrWhiteSpace(model.Types))
                dto.Types = JsonHelper.Json<PaymentType[]>(model.Types);
            return dto;
        }

        public async Task<int> CreateAsync(ChannelInputDto inputDto)
        {
            if (await _channelRepository.ExistsAppId(inputDto.Mode, inputDto.AppId))
                throw new BusiException("支付渠道已存在");
            var model = inputDto.MapTo<TChannel>();
            model.Id = IdentityHelper.Guid32;
            model.CreateTime = Clock.Now;
            model.Status = (byte)ChannelStatus.Verify;
            if (inputDto.Config != null)
            {
                if (inputDto.Config is string config)
                    model.Config = config;
                else
                    model.Config = JsonHelper.ToJson(inputDto.Config);
            }
            //model.Config = JsonHelper.ToJson(inputDto.Config);
            if (inputDto.Types != null && inputDto.Types.Length > 0)
                model.Types = JsonHelper.ToJson(inputDto.Types);
            return await _channelRepository.InsertAsync(model);
        }

        public async Task<ChannelDto> DetailAsync(string id)
        {
            var model = await _channelRepository.QueryByIdAsync(id);
            return ConvertDto(model);
        }

        public async Task<IEnumerable<ChannelDto>> DetailsAsync(IEnumerable<string> ids)
        {
            var models = await _channelRepository.QueryByIdsAsync(ids);
            return models.Select(ConvertDto).ToList();
        }

        public Task<int> SetAsync(string id, ChannelInputDto inputDto)
        {
            var model = inputDto.MapTo<TChannel>();
            model.Id = id;
            if (inputDto.Config != null)
            {
                if (inputDto.Config is string config)
                    model.Config = config;
                else
                    model.Config = JsonHelper.ToJson(inputDto.Config);
            }

            model.Types = JsonHelper.ToJson(inputDto.Types ?? new PaymentType[] { });
            return _channelRepository.UpdateAsync(model);
        }

        public async Task<int> SetDefaultAsync(string id, bool isDefault)
        {
            var model = await _channelRepository.QueryByIdAsync(id);
            if (model == null)
                throw new BusiException("渠道不存在");
            if (isDefault && model.Status != (byte)ChannelStatus.Enable)
                throw new BusiException("已开启的渠道才能设置为默认渠道");
            return await _channelRepository.UpdateIsDefaultAsync(id, model.Mode, isDefault);
        }

        public async Task<int> SetStatusAsync(string id, ChannelStatus status)
        {
            var model = await _channelRepository.QueryByIdAsync(id);
            if (model == null)
                throw new BusiException("渠道不存在");
            if (model.IsDefault)
                throw new BusiException("默认渠道不能更新状态");
            return await _channelRepository.UpdateStatusAsync(id, status);
        }

        public async Task<PagedList<ChannelDto>> ListAsync(ChannelListInputDto inputDto)
        {
            var models = await _channelRepository.QueryAsync(inputDto.Keyword, inputDto.Mode, inputDto.Status,
                inputDto.Page, inputDto.Size);
            var pList = new PagedList<ChannelDto>
            {
                Total = models.Total,
                Index = models.Index,
                Size = models.Size
            };
            var list = models.List.Select(ConvertDto).ToList();
            pList.List = list;
            return pList;
        }

        public async Task<IEnumerable<ChannelDto>> DefaultsAsync()
        {
            var models = await _channelRepository.QueryDefaultsAsync();
            return models.Select(ConvertDto).ToList();
        }

        public async Task<ChannelDto> DetailByAppIdAsync(string appId)
        {
            var model = await _channelRepository.QueryByAppIdAsync(appId);
            return ConvertDto(model);
        }
    }
}

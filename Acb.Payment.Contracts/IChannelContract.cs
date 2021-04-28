using Acb.Core.Dependency;
using Acb.Payment.Contracts.Dtos;
using Acb.Payment.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acb.Core;

namespace Acb.Payment.Contracts
{
    /// <summary> 支付渠道相关契约 </summary>
    public interface IChannelContract : IDependency
    {
        /// <summary> 创建渠道 </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        Task<int> CreateAsync(ChannelInputDto inputDto);

        /// <summary> 渠道详细 </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ChannelDto> DetailAsync(string id);

        /// <summary> 批量渠道详情 </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<IEnumerable<ChannelDto>> DetailsAsync(IEnumerable<string> ids);

        /// <summary> 更新渠道 </summary>
        /// <param name="id"></param>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        Task<int> SetAsync(string id, ChannelInputDto inputDto);

        /// <summary> 更新默认状态 </summary>
        /// <param name="id"></param>
        /// <param name="isDefault"></param>
        /// <returns></returns>
        Task<int> SetDefaultAsync(string id, bool isDefault);

        /// <summary> 更新状态 </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<int> SetStatusAsync(string id, ChannelStatus status);

        /// <summary> 渠道列表 </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        Task<PagedList<ChannelDto>> ListAsync(ChannelListInputDto inputDto);

        /// <summary> 默认渠道列表 </summary>
        /// <returns></returns>
        Task<IEnumerable<ChannelDto>> DefaultsAsync();

        /// <summary> 根据商户号获取渠道 </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        Task<ChannelDto> DetailByAppIdAsync(string appId);
    }
}

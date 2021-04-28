using Acb.Core;
using Acb.Core.Dependency;
using Acb.Payment.Contracts.Dtos;
using Acb.Payment.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acb.Payment.Contracts
{
    public interface IProjectContract : IDependency
    {
        /// <summary> 添加项目 </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<int> CreateAsync(ProjectInputDto dto);

        /// <summary> 获取项目信息 </summary>
        /// <param name="projectCode"></param>
        /// <returns></returns>
        Task<ProjectDto> DetailByCodeAsync(string projectCode);

        /// <summary> 项目信息 </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ProjectDto> DetailByIdAsync(string id);

        /// <summary> 项目列表 </summary>
        /// <param name="ids">项目ID集合</param>
        /// <returns></returns>
        Task<IEnumerable<ProjectDto>> QueryByIdsAsync(IEnumerable<string> ids);

        /// <summary> 项目列表 </summary>
        /// <param name="keyword"></param>
        /// <param name="status"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        Task<PagedList<ProjectDto>> QueryAsync(string keyword, CommonStatus? status, int page, int size);

        /// <summary> 更新项目 </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<int> SetAsync(string id, ProjectDto dto);

        /// <summary> 更新项目密钥 </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> SetSecretAsync(string id);

        /// <summary> 移除项目 </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> RemoveAsync(string id);

        /// <summary> 项目支持支付渠道列表 </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IDictionary<PaymentMode, List<ChannelDto>>> ChannelsAsync(string id);

        /// <summary> 项目支持支付渠道列表 </summary>
        /// <param name="id"></param>
        /// <param name="mode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<ChannelDto> ChannelAsync(string id, PaymentMode mode, PaymentType? type = null);
    }
}

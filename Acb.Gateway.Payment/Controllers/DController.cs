using Acb.Core.Dependency;
using Acb.Core.Extensions;
using Acb.Core.Serialize;
using Acb.Gateway.Payment.ViewModels;
using Acb.Payment.Contracts;
using Acb.Payment.Contracts.Dtos;
using Acb.Payment.Contracts.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acb.Gateway.Payment.Controllers
{
    /// <summary> 支付基础控制器 </summary>
    [ApiExplorerSettings(GroupName = "help")]
    public class DController : WebApi.DController
    {
        private IDictionary<PaymentMode, List<ChannelDto>> _projectChannels;

        /// <summary> 项目渠道 </summary>
        protected IDictionary<PaymentMode, List<ChannelDto>> ProjectChannels
        {
            get
            {
                var project = Current.Project();
                if (project == null)
                    return _projectChannels = new Dictionary<PaymentMode, List<ChannelDto>>();
                if (_projectChannels != null)
                    return _projectChannels;
                var channelContract = CurrentIocManager.Resolve<IProjectContract>();
                return _projectChannels = channelContract.ChannelsAsync(project.Id).GetAwaiter().GetResult();
            }
        }

        /// <summary> 获取支付渠道 </summary>
        /// <param name="mode"></param>
        /// <param name="type"></param>
        /// <param name="dict"></param>
        /// <returns></returns>
        protected ChannelDto Channel(PaymentMode mode, PaymentType? type = null, IDictionary<PaymentMode, List<ChannelDto>> dict = null)
        {
            dict = dict ?? ProjectChannels;
            if (!dict.TryGetValue(mode, out var channels))
                return null;
            if (!type.HasValue)
                return channels.FirstOrDefault();
            var channel = channels.FirstOrDefault(t => t.Types.Contains(type.Value));
            return channel ?? channels.FirstOrDefault(t => t.Types == null || t.Types.Length == 0);
        }

        /// <summary> 创建交易 </summary>
        /// <param name="type"></param>
        /// <param name="input"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        protected virtual async Task<TradeDto> CreateTrade(VPaymentInput input, PaymentMode? mode = null, PaymentType? type = null)
        {
            var contract = CurrentIocManager.Resolve<ITradeContract>();
            var dto = new TradeInputDto
            {
                OrderNo = input.OrderNo,
                Amount = input.Amount,
                Title = input.Title,
                Body = input.Body,
                Extend = input.Extend,
                RawParams = JsonHelper.ToJson(input),
                Type = type?.ToString(),
                RedirectUrl = input.RedirectUrl
            };
            var project = Current.Project();
            if (project != null)
            {
                dto.ProjectId = project.Id;
            }

            if (mode.HasValue)
            {
                dto.Mode = mode.Value;
                var channel = Channel(dto.Mode, type);
                if (channel != null)
                    dto.ChannelId = channel.Id;
            }

            if (type.HasValue)
            {
                dto.Type = type.Value.ToString().ToLower();
            }
            return await contract.CreateAsync(dto);
        }
    }
}

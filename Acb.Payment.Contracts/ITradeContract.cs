using Acb.Core;
using Acb.Core.Dependency;
using Acb.Payment.Contracts.Dtos;
using Acb.Payment.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acb.Payment.Contracts
{
    /// <summary> 交易相关契约 </summary>
    public interface ITradeContract : IDependency
    {
        /// <summary> 创建交易 </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        Task<TradeDto> CreateAsync(TradeInputDto inputDto);

        /// <summary> 交易详情 </summary>
        /// <param name="id">交易ID</param>
        /// <returns></returns>
        Task<TradeDto> DetailAsync(string id);

        /// <summary> 交易详情 </summary>
        /// <param name="tradeNo">交易ID</param>
        /// <returns></returns>
        Task<TradeDto> DetailByNoAsync(string tradeNo);

        /// <summary> 交易详情 </summary>
        /// <param name="projectCode">项目编码</param>
        /// <param name="orderNo">订单号</param>
        /// <returns></returns>
        Task<TradeDto> DetailAsync(string projectCode, string orderNo);

        /// <summary> 修改支付方式 </summary>
        /// <param name="id"></param>
        /// <param name="mode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<int> ChangeModeAsync(string id, PaymentMode mode, string type);

        /// <summary> 交易支付 </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        Task<TradeDto> TradePaidAsync(TradePaidInputDto inputDto);

        /// <summary> 通知记录 </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        Task NotifyAsync(TradeNotifyInputDto inputDto);

        /// <summary> 交易记录列表 </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<PagedList<TradeDto>> QueryAsync(TradeSearchInputDto dto);

        /// <summary> 交易通知列表 </summary>
        /// <param name="page">分页页码</param>
        /// <param name="size">单页数量</param>
        /// <param name="type">通知类型</param>
        /// <param name="tradeNo">交易单号</param>
        /// <param name="projectId">项目ID</param>
        /// <returns></returns>
        Task<PagedList<TradeNotifyDto>> NotifyListAsync(int page, int size, NotifyType? type = null,
            string tradeNo = null, string projectId = null);

        Task<HomeStatisticDto> StatisticAsync(string projectId = null, int days = 10);

        /// <summary> 保存交易用户平台信息 </summary>
        /// <param name="id"></param>
        /// <param name="platformDto"></param>
        /// <returns></returns>
        Task<int> SavePlatform(string id, PlatformDto platformDto);

        /// <summary> 获取交易用户平台信息 </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<PlatformDto> GetPlatform(string id);

        /// <summary> 获取支付参数 </summary>
        /// <param name="tradeId"></param>
        /// <param name="mode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<string> GetPaymentAsync(string tradeId, PaymentMode mode, PaymentType type);

        /// <summary> 保存支付信息 </summary>
        /// <param name="tradeId"></param>
        /// <param name="mode"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<int> SavePaymentAsync(string tradeId, PaymentMode mode, PaymentType type, string value);

        /// <summary> 获取该支付类型下的所有支付参数 </summary>
        /// <param name="tradeId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<Dictionary<PaymentMode, string>> GetPaymentsAsync(string tradeId, PaymentType type);

        /// <summary>
        /// 申请退款
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        Task<int> RefundAsync(TradeRefundInputDto inputDto);

        /// <summary>
        /// 退款成功
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        Task<int> RefundSuccessAsync(TradeRefundInputDto inputDto);
    }
}

using Acb.AutoMapper;
using Acb.Core;
using Acb.Core.Domain;
using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Core.Timing;
using Acb.Payment.Business.Domain.Entities;
using Acb.Payment.Business.Domain.Repositories;
using Acb.Payment.Contracts;
using Acb.Payment.Contracts.Dtos;
using Acb.Payment.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acb.Payment.Business
{
    public class TradeService : DService, ITradeContract
    {
        private readonly ProjectRepository _projectRepository;
        private readonly TradeRepository _tradeRepository;
        private readonly PlatformRepository _platformRepository;
        private readonly TradePaymentRepository _paymentRepository;

        /// <summary> 支付默认标题 </summary>
        private const string PaymentTitle = "i车保在线支付";

        public TradeService(TradeRepository repository, ProjectRepository projectRepository,
            PlatformRepository platformRepository, TradePaymentRepository paymentRepository)
        {
            _tradeRepository = repository;
            _projectRepository = projectRepository;
            _platformRepository = platformRepository;
            _paymentRepository = paymentRepository;
        }

        private static string CreateTradeNo()
        {
            //毫秒级+随机5位
            var date = Clock.Now - new DateTime(2018, 10, 1);
            var tick = ((long)date.TotalMilliseconds).ToString().PadRight(13, '0');
            return $"T{tick}{RandomHelper.RandomNums(5)}";
        }

        public async Task<TradeDto> CreateAsync(TradeInputDto inputDto)
        {
            var model = await _tradeRepository.QueryByOrderNo(inputDto.ProjectId, inputDto.OrderNo);
            if (model != null)
            {
                //已有支付订单
                if (model.Status != (byte)TradeStatus.WaitPay)
                    throw new BusiException($"支付状态异常，当前状态:{model.Status.GetEnumText<TradeStatus, byte>()}");
                var update = inputDto.MapTo<TTrade>();
                update.Id = model.Id;
                if (string.IsNullOrWhiteSpace(update.Title))
                    update.Title = model.Title;
                await _tradeRepository.UpdateTradeAsync(update, new[]
                {
                    nameof(TTrade.Mode), nameof(TTrade.Type),
                    nameof(TTrade.Body), nameof(TTrade.Extend), nameof(TTrade.RawParams), nameof(TTrade.ChannelId)
                });
                return model.MapTo<TradeDto>();
            }

            model = inputDto.MapTo<TTrade>();
            model.Id = IdentityHelper.Guid32;
            if (string.IsNullOrWhiteSpace(model.Title))
                model.Title = PaymentTitle;
            model.TradeNo = CreateTradeNo();
            model.Status = (byte)TradeStatus.WaitPay;
            model.CreateTime = Clock.Now;
            var result = await _tradeRepository.InsertAsync(model);
            return result > 0 ? model.MapTo<TradeDto>() : throw new BusiException("创建交易失败");
        }

        public async Task<TradeDto> DetailAsync(string id)
        {
            var model = await _tradeRepository.QueryByIdAsync(id);
            if (model == null)
                throw new BusiException("交易不存在");
            if (string.IsNullOrWhiteSpace(model.RedirectUrl))
            {
                var project = await _projectRepository.QueryByIdAsync(model.ProjectId);
                model.RedirectUrl = project?.RedirectUrl;
            }
            return model.MapTo<TradeDto>();
        }

        public async Task<TradeDto> DetailByNoAsync(string tradeNo)
        {
            var model = await _tradeRepository.QueryByIdAsync(tradeNo, "trade_no");
            if (model == null)
                throw new BusiException("交易不存在");
            if (string.IsNullOrWhiteSpace(model.RedirectUrl))
            {
                var project = await _projectRepository.QueryByIdAsync(model.ProjectId);
                model.RedirectUrl = project?.RedirectUrl;
            }
            return model.MapTo<TradeDto>();
        }

        public async Task<TradeDto> DetailAsync(string projectCode, string orderNo)
        {
            var project = await _projectRepository.QueryByCode(projectCode);
            if (project == null || project.Status == (byte)CommonStatus.Delete)
                throw new BusiException("项目编码不存在");
            var model = await _tradeRepository.QueryByOrderNo(project.Id, orderNo);
            if (model == null)
                throw new BusiException("商户订单交易不存在");
            return model.MapTo<TradeDto>();
        }

        public async Task<int> ChangeModeAsync(string id, PaymentMode mode, string type)
        {
            return await _tradeRepository.ChangeMode(id, mode, type);
        }

        public async Task<TradeDto> TradePaidAsync(TradePaidInputDto inputDto)
        {
            var model = await _tradeRepository.QueryByIdAsync(inputDto.TradeNo, "trade_no");
            if (model == null)
                throw new BusiException($"支付交易号[{inputDto.TradeNo}]不存在", 20001);
            if (model.Status != (byte)TradeStatus.WaitPay)
            {
                //重复支付,退款
                throw new BusiException("重复的支付", 20001);
            }

            if (model.Amount != inputDto.Amount)
            {
                //支付金额异常,退款
                throw new BusiException("支付金额异常");
            }
            model.OutTradeNo = inputDto.OutTradeNo;
            model.PaidUser = inputDto.User;
            model.PaidAccount = inputDto.Account;
            model.PaidTime = inputDto.PaidTime;
            model.Mode = (byte)inputDto.Mode;
            model.Status = (byte)TradeStatus.Paid;
            var result = await _tradeRepository.Paid(model);
            if (result <= 0)
                //支付异常, 退款
                throw new BusiException("支付异常");
            return model.MapTo<TradeDto>();
        }

        private static bool IsSuccess(string result)
        {
            if (string.IsNullOrWhiteSpace(result))
                return false;
            var successResults = new[] { "接收成功", "通知成功" };
            if (successResults.Contains(result))
                return true;
            return "SUCCESS".Equals(result.ToUpper());
        }

        public Task NotifyAsync(TradeNotifyInputDto inputDto)
        {
            var model = inputDto.MapTo<TTradeNotify>();
            model.Id = IdentityHelper.Guid32;
            model.CreateTime = Clock.Now;
            if (IsSuccess(model.Result))
                model.Status = (byte)NotifyStatus.Success;
            else
                model.Status = (byte)NotifyStatus.Fail;
            return _tradeRepository.Notify(model);
        }

        public async Task<PagedList<TradeDto>> QueryAsync(TradeSearchInputDto dto)
        {
            var models = await _tradeRepository.QueryAsync(dto);
            return models.MapPagedList<TradeDto, TTrade>();
        }

        public async Task<PagedList<TradeNotifyDto>> NotifyListAsync(int page, int size, NotifyType? type = null,
            string tradeNo = null, string projectId = null)
        {
            return await _tradeRepository.NotifyListAsync(page, size, type, tradeNo, projectId);
        }

        public async Task<HomeStatisticDto> StatisticAsync(string projectId = null, int days = 10)
        {
            var dto = await _tradeRepository.StatisticAsync(projectId, days);
            for (var i = days - 1; i >= 0; i--)
            {
                var date = Clock.Now.AddDays(-i);
                var key = date.ToString("yyyy-MM-dd");
                if (dto.Platforms.Any(t => t.Date == key))
                    continue;
                dto.Platforms.Add(new StatisticPlatform
                {
                    Date = key
                });
            }

            dto.Platforms = dto.Platforms.OrderBy(t => t.Date).ToList();
            return dto;
        }

        public async Task<int> SavePlatform(string id, PlatformDto platformDto)
        {
            var platformId = await _platformRepository.ExistsAsync(platformDto.ChannelId, platformDto.OpenId);
            if (!string.IsNullOrWhiteSpace(platformId))
            {
                return await DTransaction.Use(async () =>
                {
                    var count = await _platformRepository.UpdateAsync(platformId, platformDto);
                    count += await _tradeRepository.UpdatePlatformAsync(id, platformId);
                    return count;
                });
            }
            var model = platformDto.MapTo<TPlatform>();
            model.Id = IdentityHelper.Guid32;
            model.CreateTime = Clock.Now;
            return await DTransaction.Use(async () =>
            {
                var count = await _platformRepository.InsertAsync(model);
                count += await _tradeRepository.UpdatePlatformAsync(id, model.Id);
                return count;
            });
        }

        public async Task<PlatformDto> GetPlatform(string id)
        {
            var detail = await _tradeRepository.QueryByIdAsync(id);
            if (detail == null || string.IsNullOrWhiteSpace(detail.PlatformId))
                return null;
            var model = _platformRepository.QueryByIdAsync(detail.PlatformId);
            return model.MapTo<PlatformDto>();
        }

        public Task<string> GetPaymentAsync(string tradeId, PaymentMode mode, PaymentType type)
        {
            return _paymentRepository.QueryByModeAndTypeAsync(tradeId, mode, type.ToString().ToLower());
        }

        public Task<int> SavePaymentAsync(string tradeId, PaymentMode mode, PaymentType type, string value)
        {
            var model = new TTradePayment
            {
                Id = IdentityHelper.Guid32,
                TradeId = tradeId,
                Mode = (byte)mode,
                Type = type.ToString().ToLower(),
                Value = value,
                CreateTime = Clock.Now
            };
            return _paymentRepository.InsertAsync(model);
        }

        public Task<Dictionary<PaymentMode, string>> GetPaymentsAsync(string tradeId, PaymentType type)
        {
            return _paymentRepository.QueryByTypeAsync(tradeId, type.ToString().ToLower());
        }

        public async Task<int> RefundAsync(TradeRefundInputDto inputDto)
        {
            var model = await _tradeRepository.QueryByIdAsync(inputDto.TradeId);
            if (model == null)
                throw new BusiException("交易不存在");
            if (model.Status != (byte)TradeStatus.Paid)
                throw new BusiException("交易状态异常");
            model.Status = (byte)TradeStatus.Refunding;
            model.RefundNo = inputDto.RefundNo;
            model.OutRefundNo = inputDto.OutRefundNo;
            model.RefundAmount = inputDto.Amount ?? model.Amount;
            model.RefundReason = inputDto.Reason;
            model.RefundTime = Clock.Now;
            return await _tradeRepository.UpdateTradeAsync(model, new[]
            {
                nameof(TTrade.Status), nameof(TTrade.RefundNo), nameof(TTrade.OutRefundNo),
                nameof(TTrade.RefundAmount), nameof(TTrade.RefundReason), nameof(TTrade.RefundTime)
            });
        }

        public async Task<int> RefundSuccessAsync(TradeRefundInputDto inputDto)
        {
            var model = await _tradeRepository.QueryByIdAsync(inputDto.TradeId);
            if (model == null)
                throw new BusiException("交易不存在");
            if (model.Status != (byte)TradeStatus.Refunding)
                throw new BusiException("交易状态异常");
            model.Status = (byte)TradeStatus.Refund;
            model.OutRefundNo = inputDto.OutRefundNo;
            model.RefundAmount = inputDto.Amount ?? model.Amount;
            model.RefundTime = inputDto.RefundTime ?? Clock.Now;
            return await _tradeRepository.UpdateTradeAsync(model, new[]
            {
                nameof(TTrade.Status), nameof(TTrade.OutRefundNo),
                nameof(TTrade.RefundAmount), nameof(TTrade.RefundTime)
            });
        }
    }
}

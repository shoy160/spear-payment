using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaySharp.Core;
using PaySharp.Core.Response;
using PaySharp.Wechatpay;
using PaySharp.Wechatpay.Domain;
using PaySharp.Wechatpay.Request;
using Spear.Core;
using Spear.Core.Dependency;
using Spear.Core.Extensions;
using Spear.Core.Serialize;
using Spear.Core.Timing;
using Spear.Gateway.Payment.Filters;
using Spear.Gateway.Payment.Payment;
using Spear.Gateway.Payment.ViewModels;
using Spear.Payment.Contracts;
using Spear.Payment.Contracts.Dtos;
using Spear.Payment.Contracts.Enums;
using System;
using System.Threading.Tasks;

namespace Spear.Gateway.Payment.Controllers
{
    /// <summary> 微信支付 </summary>
    [Route("wechat"), Project]
    public class WechatController : PaymentController<WechatpayGateway>
    {
        private readonly ILogger _logger;

        public WechatController() : base(PaymentMode.Wechat)
        {
            _logger = CurrentIocManager.CreateLogger<WechatController>();
        }

        private T ParsePayModel<T>(TradeDto dto, VPaymentInput input)
            where T : BasePayModel, new()
        {
            var model = new T
            {
                OutTradeNo = dto.TradeNo,
                TotalAmount = input.Amount,
                Body = dto.Title
            };
            if (!string.IsNullOrWhiteSpace(dto.Extend))
            {
                model.Attach = dto.Extend;
            }
            if (input.Timeout.HasValue && input.Timeout > 0)
            {
                model.TimeExpire = Clock.Now.AddSeconds(input.Timeout.Value).ToString("yyyyMMddHHmmss");
            }
            return model;
        }

        /// <summary> 公众号支付 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("public")]
        public async Task<DResult<string>> PublicPay(VUserTagPaymentByInput input)
        {
            var dto = await CreateTrade(PaymentType.Public, input);
            var request = new PublicPayRequest();
            var model = ParsePayModel<PublicPayModel>(dto, input);
            model.OpenId = input.OpenId;
            request.AddGatewayData(model);
            request.ReturnUrl = input.RedirectUrl;
            var response = Gateway(PaymentType.Public).Execute(request);
            _logger.LogDebug(JsonHelper.ToJson(response));
            if (response.ReturnCode == "FAIL")
                return Error<string>(response.ReturnMsg);
            if (response.ResultCode == "FAIL")
                return Error<string>(response.ErrCodeDes);
            return Succ(response.OrderInfo);
        }

        /// <summary> APP支付 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("app")]
        public async Task<DResult<string>> AppPay(VPaymentInput input)
        {
            var dto = await CreateTrade(PaymentType.App, input);
            var request = new AppPayRequest();
            IGateway gateway = Gateway(PaymentType.App);
            if (gateway is WechatpayGateway wc && wc.Merchant.IsTest)
            {
                // 验收Case 必须为2.01
                input.Amount = 201;
            }
            var payModel = ParsePayModel<AppPayModel>(dto, input);
            request.AddGatewayData(payModel);
            var response = gateway.Execute(request);
            _logger.LogDebug(JsonHelper.ToJson(response));
            if (response.ReturnCode == "FAIL")
                return Error<string>(response.ReturnMsg);
            if (response.ResultCode == "FAIL")
                return Error<string>(response.ErrCodeDes);
            return Succ(response.OrderInfo);
        }

        /// <summary> 小程序支付 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("applet")]
        public async Task<DResult<string>> AppletPay(VUserTagPaymentByInput input)
        {
            var dto = await CreateTrade(PaymentType.Applet, input);
            var request = new AppletPayRequest();

            var model = ParsePayModel<AppletPayModel>(dto, input);
            model.OpenId = input.OpenId;
            request.AddGatewayData(model);

            var response = Gateway(PaymentType.Applet).Execute(request);
            _logger.LogDebug(JsonHelper.ToJson(response));
            if (response.ReturnCode == "FAIL")
                return Error<string>(response.ReturnMsg);
            if (response.ResultCode == "FAIL")
                return Error<string>(response.ErrCodeDes);
            return Succ(response.OrderInfo);
        }

        /// <summary> H5支付 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("h5")]
        public async Task<DResult<string>> WapPay(VSceneInfoPaymentInput input)
        {
            var dto = await CreateTrade(PaymentType.H5, input);
            var request = new WapPayRequest();
            var clientIp = Current.RemoteIp();
            _logger.LogDebug($"[h5]client-ip:{clientIp}");
            var model = ParsePayModel<WapPayModel>(dto, input);
            model.SceneInfo = input.SceneInfo;
            model.SpbillCreateIp = clientIp;
            request.AddGatewayData(model);
            request.ReturnUrl = input.RedirectUrl;
            var response = Gateway(PaymentType.H5).Execute(request);
            if (response.ReturnCode == "FAIL")
                return Error<string>(response.ReturnMsg);
            if (response.ResultCode == "FAIL")
                return Error<string>(response.ErrCodeDes);
            var url = response.MwebUrl;
            var redirect = dto.RedirectUrl;
            if (string.IsNullOrWhiteSpace(redirect))
            {
                var project = Current.Project();
                redirect = project?.RedirectUrl;
            }
            if (!string.IsNullOrWhiteSpace(redirect))
                url = Current.SetQuery("redirect_url", redirect, url);
            return Succ(url);
        }

        /// <summary> 扫码支付 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("scan")]
        public async Task<DResult<string>> ScanPay(VPaymentInput input)
        {
            const PaymentType type = PaymentType.Scan;
            var dto = await CreateTrade(type, input);
            var request = new ScanPayRequest();
            var model = ParsePayModel<ScanPayModel>(dto, input);
            request.AddGatewayData(model);
            var response = Gateway(PaymentType.Scan).Execute(request);
            if (response.ReturnCode == "FAIL")
                return Error<string>(response.ReturnMsg);
            if (response.ResultCode == "FAIL")
                return Error<string>(response.ErrCodeDes);
            return Succ(response.CodeUrl);
        }

        /// <summary> 条码支付 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("barcode"), ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> BarcodePay(VBarcodePaymentInput input)
        {
            var dto = await CreateTrade(PaymentType.Barcode, input);
            var request = new BarcodePayRequest();
            var model = ParsePayModel<BarcodePayModel>(dto, input);
            model.AuthCode = input.AuthCode;
            request.AddGatewayData(model);
            request.PaySucceed += BarcodePay_PaySucceed;
            request.PayFailed += BarcodePay_PayFaild;

            var response = Gateway(PaymentType.Barcode).Execute(request);

            return Json(response);
        }

        /// <summary> 支付成功事件 </summary>
        /// <param name="response">返回结果</param>
        /// <param name="message">提示信息</param>
        private void BarcodePay_PaySucceed(IResponse response, string message)
        {
        }

        /// <summary>
        /// 支付失败事件
        /// </summary>
        /// <param name="response">返回结果,可能是BarcodePayResponse/QueryResponse</param>
        /// <param name="message">提示信息</param>
        private void BarcodePay_PayFaild(IResponse response, string message)
        {
        }

        /// <summary> 支付查询 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("query")]
        public async Task<ActionResult> Query(VQueryInput input)
        {
            var trade = await GetTrade(input.OrderNo, input.ProjectCode);
            var request = new QueryRequest();
            request.AddGatewayData(new QueryModel
            {
                TradeNo = trade.OutTradeNo,
                OutTradeNo = trade.TradeNo
            });
            var response = Gateway().Execute(request);
            return Json(response);
        }

        /// <summary> 退款 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("refund")]
        public async Task<ActionResult> Refund(VRefundInput input)
        {
            var trade = await GetTrade(input.OrderNo, input.ProjectCode);
            var request = new RefundRequest();
            request.AddGatewayData(new RefundModel
            {
                TradeNo = trade.OutTradeNo,
                RefundAmount = (int)input.RefundAmount,
                RefundDesc = input.RefundDesc,
                OutRefundNo = input.RefundOrderNo,
                TotalAmount = (int)input.Amount,
                OutTradeNo = trade.TradeNo
            });

            var response = Gateway().Execute(request);
            return Json(response);
        }

        /// <summary> 退款查询 </summary>
        /// <param name="orderNo"></param>
        /// <param name="tradeNo"></param>
        /// <param name="outRefundNo"></param>
        /// <param name="refundNo"></param>
        /// <returns></returns>
        [HttpGet("refund/query"), ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult RefundQuery(string orderNo, string tradeNo, string outRefundNo, string refundNo)
        {
            var request = new RefundQueryRequest();
            request.AddGatewayData(new RefundQueryModel
            {
                TradeNo = tradeNo,
                OutTradeNo = orderNo,
                OutRefundNo = outRefundNo,
                RefundNo = refundNo
            });

            var response = Gateway().Execute(request);
            return Json(response);
        }

        /// <summary> 关闭支付 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("close")]
        public async Task<ActionResult> Close(VQueryInput input)
        {
            var trade = await GetTrade(input.OrderNo, input.ProjectCode);
            var request = new CloseRequest();
            request.AddGatewayData(new CloseModel
            {
                OutTradeNo = trade.TradeNo
            });

            var response = Gateway().Execute(request);
            return Json(response);
        }

        /// <summary> 取消支付 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("cancel")]
        public async Task<ActionResult> Cancel(VQueryInput input)
        {
            var trade = await GetTrade(input.OrderNo, input.ProjectCode);
            var request = new CancelRequest();
            request.AddGatewayData(new CancelModel
            {
                OutTradeNo = trade.TradeNo
            });

            var response = Gateway().Execute(request);
            return Json(response);
        }

        /// <summary> 转账 </summary>
        /// <param name="orderNo">订单号</param>
        /// <param name="amount">金额(分)</param>
        /// <param name="openid"></param>
        /// <param name="checkName"></param>
        /// <param name="trueName"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        [HttpGet("transfer"), ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult Transfer(string orderNo, int amount, string openid, string checkName, string trueName, string desc)
        {
            var request = new TransferRequest();
            request.AddGatewayData(new TransferModel()
            {
                OutTradeNo = orderNo,
                OpenId = openid,
                Amount = amount,
                Desc = desc,
                CheckName = checkName,
                TrueName = trueName
            });

            var response = Gateway().Execute(request);
            return Json(response);
        }

        /// <summary> 转账查询 </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet("transfer/query"), ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult TransferQuery(string orderNo)
        {
            var request = new TransferQueryRequest();
            request.AddGatewayData(new TransferQueryModel
            {
                OutTradeNo = orderNo
            });

            var response = Gateway().Execute(request);
            return Json(response);
        }

        /// <summary> 公钥查询 </summary>
        /// <returns></returns>
        [HttpGet("public/key"), ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult PublicKey()
        {
            var request = new PublicKeyRequest();

            var response = Gateway().Execute(request);
            return Json(response);
        }

        /// <summary> 银行转账 </summary>
        /// <param name="orderNo"></param>
        /// <param name="bankNo"></param>
        /// <param name="trueName"></param>
        /// <param name="bankCode"></param>
        /// <param name="amount"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        [HttpGet("transfer/bank"), ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult TransferToBank(string orderNo, string bankNo, string trueName, string bankCode, int amount, string desc)
        {
            var request = new TransferToBankRequest();
            request.AddGatewayData(new TransferToBankModel
            {
                OutTradeNo = orderNo,
                BankNo = bankNo,
                Amount = amount,
                Desc = desc,
                BankCode = bankCode,
                TrueName = trueName
            });

            var response = Gateway().Execute(request);
            return Json(response);
        }

        /// <summary> 银行转账查询 </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet("transfer/bank/query"), ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult TransferToBankQuery(string orderNo)
        {
            var request = new TransferToBankQueryRequest();
            request.AddGatewayData(new TransferToBankQueryModel()
            {
                OutTradeNo = orderNo
            });

            var response = Gateway().Execute(request);
            return Json(response);
        }

        /// <summary> 账单下载 </summary>
        /// <param name="billDate">下载对账单的日期，格式"yyyyMMdd"</param>
        /// <param name="billType">
        /// 账单类型
        /// ALL，返回当日所有订单信息，默认值
        /// SUCCESS，返回当日成功支付的订单
        /// REFUND，返回当日退款订单
        /// RECHARGE_REFUND，返回当日充值退款订单</param>
        /// <returns></returns>
        [HttpGet("bill/download"), ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult BillDownload(string billDate, string billType)
        {
            var request = new BillDownloadRequest();
            request.AddGatewayData(new BillDownloadModel()
            {
                BillDate = billDate,
                BillType = billType
            });

            var response = Gateway().Execute(request);
            return File(response.GetBillFile(), "text/csv", $"{DateTime.Now:yyyyMMddHHmmss}.csv");
        }

        /// <summary> 资金流水下载 </summary>
        /// <param name="billDate"></param>
        /// <param name="accountType"></param>
        /// <returns></returns>
        [HttpGet("flow/download"), ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult FundFlowDownload(string billDate, string accountType)
        {
            var request = new FundFlowDownloadRequest();
            request.AddGatewayData(new FundFlowDownloadModel()
            {
                BillDate = billDate,
                AccountType = accountType
            });

            var response = Gateway().Execute(request);
            return File(response.GetBillFile(), "text/csv", $"{DateTime.Now:yyyyMMddHHmmss}.csv");
        }

        /// <summary> OAuth认证 </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet("oauth"), ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult OAuth(string code)
        {
            var request = new OAuthRequest();
            request.AddGatewayData(new OAuthModel
            {
                Code = code
            });

            var response = Gateway().Execute(request);
            return Json(response);
        }
    }
}
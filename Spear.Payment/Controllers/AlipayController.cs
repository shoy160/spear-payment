﻿using Microsoft.AspNetCore.Mvc;
using Spear.Core;
using Spear.Core.Extensions;
using Spear.Core.Timing;
using Spear.Gateway.Payment.ViewModels;
using Spear.Payment.Alipay;
using Spear.Payment.Alipay.Domain;
using Spear.Payment.Alipay.Request;
using Spear.Payment.Contracts.Dtos;
using Spear.Payment.Contracts.Enums;
using Spear.Payment.Core.Response;
using System.Threading.Tasks;

namespace Spear.Payment.Controllers
{
    /// <summary> 支付宝支付 </summary>
    [Route("alipay")]
    public class AlipayController : PaymentController<AlipayGateway>
    {
        public AlipayController() : base(PaymentMode.Alipay)
        {
        }

        private T ParsePayModel<T>(TradeDto dto, VPaymentInput input)
            where T : BasePayModel, new()
        {
            var model = new T
            {
                OutTradeNo = dto.TradeNo,
                TotalAmount = dto.Amount / 100D,
                Subject = dto.Title,
                Body = dto.Body
            };
            if (!string.IsNullOrWhiteSpace(dto.Extend))
            {
                model.PassbackParams = dto.Extend.UrlEncode();
            }
            if (input.Timeout.HasValue && input.Timeout > 0)
            {
                //过期时间
                model.TimeExpire = Clock.Now.AddSeconds(input.Timeout.Value).ToString("yyyy-MM-dd HH:mm");
            }

            return model;
        }

        /// <summary> 网页支付(跳转) </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("web")]
        public async Task<DResult<string>> WebPay(VPaymentInput input)
        {
            var dto = await CreateTrade(PaymentType.Web, input);

            var request = new WebPayRequest();
            var payModel = ParsePayModel<WebPayModel>(dto, input);
            request.AddGatewayData(payModel);
            request.ReturnUrl = input.RedirectUrl;
            var response = Gateway(PaymentType.Web).Execute(request);
            return Succ(response.Html);
            //return Content(response.Html, "text/html", Encoding.UTF8);
        }

        /// <summary> H5支付 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("h5")]
        public async Task<DResult<string>> WapPay(VPaymentInput input)
        {
            var dto = await CreateTrade(PaymentType.H5, input);
            var request = new WapPayRequest();
            var payModel = ParsePayModel<WapPayModel>(dto, input);
            payModel.QuitUrl = input.RedirectUrl;
            request.AddGatewayData(payModel);
            request.ReturnUrl = input.RedirectUrl;
            var response = Gateway(PaymentType.H5).Execute(request);
            return Succ(response.Url);
        }

        /// <summary> App支付 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("app")]
        public async Task<DResult<string>> AppPay(VPaymentInput input)
        {
            var dto = await CreateTrade(PaymentType.App, input);
            var request = new AppPayRequest();
            var payModel = ParsePayModel<AppPayModel>(dto, input);
            request.AddGatewayData(payModel);
            request.ReturnUrl = input.RedirectUrl;
            var response = Gateway(PaymentType.App).Execute(request);
            return Succ(response.OrderInfo);
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
            request.AddGatewayData(new ScanPayModel
            {
                OutTradeNo = dto.TradeNo,
                TotalAmount = dto.Amount / 100D,
                Subject = dto.Title,
                Body = dto.Body
            });

            var response = Gateway(PaymentType.Scan).Execute(request);
            if (response.Code == "10000")
            {
                return Succ(response.QrCode);
            }
            return Error<string>($"{response.SubCode}:{response.SubMessage}");
        }

        /// <summary> 条码支付 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("barcode"), ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> BarcodePay(VBarcodePaymentInput input)
        {
            var dto = await CreateTrade(PaymentType.Barcode, input);
            var request = new BarcodePayRequest();
            request.AddGatewayData(new BarcodePayModel
            {
                OutTradeNo = dto.TradeNo,
                TotalAmount = dto.Amount / 100D,
                Subject = dto.Title,
                Body = dto.Body,
                AuthCode = input.AuthCode
            });
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
            request.AddGatewayData(new RefundModel()
            {
                TradeNo = trade.OutTradeNo,
                OutTradeNo = trade.TradeNo,
                RefundAmount = input.RefundAmount / 100D,
                RefundReason = input.RefundDesc,
                OutRefundNo = input.RefundOrderNo
            });

            var response = Gateway().Execute(request);
            return Json(response);
        }

        /// <summary> 退款查询 </summary>
        /// <param name="orderNo"></param>
        /// <param name="tradeNo"></param>
        /// <param name="outRefundNo"></param>
        /// <returns></returns>
        [HttpGet("refund/query"), ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult RefundQuery(string orderNo, string tradeNo, string outRefundNo)
        {
            var request = new RefundQueryRequest();
            request.AddGatewayData(new RefundQueryModel
            {
                TradeNo = tradeNo,
                OutTradeNo = orderNo,
                OutRefundNo = outRefundNo
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
                TradeNo = trade.OutTradeNo,
                OutTradeNo = trade.TradeNo
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
                TradeNo = trade.OutTradeNo,
                OutTradeNo = trade.TradeNo
            });

            var response = Gateway().Execute(request);
            return Json(response);
        }

        /// <summary> 转账 </summary>
        /// <param name="orderNo"></param>
        /// <param name="payeeAccount"></param>
        /// <param name="payeeType"></param>
        /// <param name="amount">转账金额(分)</param>
        /// <param name="remark"></param>
        /// <returns></returns>
        [HttpGet("transfer"), ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult Transfer(string orderNo, string payeeAccount, string payeeType, int amount, string remark)
        {
            var request = new TransferRequest();
            request.AddGatewayData(new TransferModel
            {
                OutTradeNo = orderNo,
                PayeeAccount = payeeAccount,
                Amount = amount / 100D,
                Remark = remark,
                PayeeType = payeeType
            });

            var response = Gateway().Execute(request);
            return Json(response);
        }

        /// <summary> 转账查询 </summary>
        /// <param name="orderNo"></param>
        /// <param name="tradeNo"></param>
        /// <returns></returns>
        [HttpGet("transfer/query"), ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult TransferQuery(string orderNo, string tradeNo)
        {
            var request = new TransferQueryRequest();
            request.AddGatewayData(new TransferQueryModel()
            {
                TradeNo = tradeNo,
                OutTradeNo = orderNo
            });

            var response = Gateway().Execute(request);
            return Json(response);
        }

        /// <summary> 对账单下载 </summary>
        /// <param name="billDate"></param>
        /// <param name="billType"></param>
        /// <returns></returns>
        [HttpGet("bill/download"), ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> BillDownload(string billDate, string billType)
        {
            var request = new BillDownloadRequest();
            var model = new BillDownloadModel
            {
                BillDate = billDate,
                BillType = billType
            };
            request.AddGatewayData(model);

            var response = Gateway().Execute(request);
            return File(await response.GetBillFileAsync(), "application/zip");
        }
    }
}

﻿using PaySharp.Core.Attributes;
using PaySharp.Core.Request;

namespace PaySharp.Wechatpay.Response
{
    public class SandboxKeyResponse : BaseResponse
    {
        /// <summary>
        /// 沙箱环境商户号
        /// </summary>
        [ReName(Name = "mch_id")]
        public string SandboxMchId { get; set; }

        /// <summary>
        /// 沙箱环境秘钥
        /// </summary>
        [ReName(Name = "sandbox_signkey")]
        public string SandboxKey { get; set; }

        internal override void Execute<TModel, TResponse>(Merchant merchant, Request<TModel, TResponse> request)
        {
        }
    }
}

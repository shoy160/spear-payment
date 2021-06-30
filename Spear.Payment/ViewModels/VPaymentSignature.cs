using Microsoft.Extensions.Logging;
using Spear.Core;
using Spear.Core.Dependency;
using Spear.Core.Domain;
using Spear.Core.Extensions;
using Spear.Core.Serialize;
using Spear.Core.Timing;
using Spear.Gateway.Payment.Payment;
using Spear.Payment.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Spear.Gateway.Payment.ViewModels
{
    /// <summary> 支付参数签名 </summary>
    public class VPaymentSignature : IValidatableObject
    {
        /// <summary> 项目编码 </summary>
        [Required(ErrorMessage = "项目编号不能为空")]
        [StringLength(16, MinimumLength = 3, ErrorMessage = "项目编号无效")]
        public string ProjectCode { get; set; }

        /// <summary>
        /// 签名
        /// 规则=>时间戳(毫秒) + Md532(除Sign外所有不为空的参数的url拼接(如：a=1&amp;b=2,不编码,参数名ASCII升序,首字母大写) + key + 时间戳(毫秒)).ToLower()
        /// </summary>
        [Required(ErrorMessage = "接口签名无效")]
        [StringLength(45, ErrorMessage = "接口签名无效")]
        public string Sign { get; set; }

        /// <inheritdoc />
        /// <summary> 参数验证 </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            var contract = CurrentIocManager.Resolve<IProjectContract>();
            var project = contract.DetailByCodeAsync(ProjectCode).GetAwaiter().GetResult();
            if (project == null)
            {
                results.Add(new ValidationResult("项目编号不存在"));
                return results;
            }

            CurrentIocManager.Context.SetProject(project);
            var mode = PaymentExtensions.Mode;

            if (string.IsNullOrWhiteSpace(project.Secret) || mode == Core.ProductMode.Dev)
                return results;
            if (Sign.Length != 45)
            {
                results.Add(new ValidationResult("参数签名格式异常"));
                return results;
            }

            var timestamp = Sign.Substring(0, 13).CastTo(0L);
            ////有效期验证 5分钟有效期
            if (DateTime.Now > timestamp.FromMillisecondTimestamp().AddMinutes(5))
            {
                results.Add(new ValidationResult("请求已失效"));
                return results;
            }

            //签名验证
            //获取除Sign外所有参数并进行排序、url拼接
            var props = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var dict = props.Where(prop => prop.Name != nameof(Sign))
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(validationContext.ObjectInstance));
            var logger = CurrentIocManager.CreateLogger<VPaymentSignature>();
            logger.LogDebug(JsonHelper.ToJson(dict));
            var sign = dict.Sign(project.Secret, timestamp);
            //签名验证
            if (!string.Equals(sign, Sign, StringComparison.CurrentCultureIgnoreCase))
            {
                var msg = $"参数签名验证失败,{Sign}=>{sign}";
                logger.LogWarning(msg);
                results.Add(new ValidationResult(msg));
            }

            return results;
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spear.Core.Dependency;
using Spear.Core.Extensions;
using Spear.Payment.Contracts;
using Spear.Payment.Contracts.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spear.Gateway.Payment.ViewModels
{
    internal static class Constants
    {
        private const string ProjectCacheKey = "__current_project";
        private const string ProjectCodeKey = "ProjectCode";

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        private static string Md5(string data)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            var dataByte = md5.ComputeHash(Encoding.UTF8.GetBytes(data));
            var sb = new StringBuilder();
            foreach (var x in dataByte)
            {
                sb.Append(x.ToString("X2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 参数签名
        /// 规则:时间戳(毫秒) + Md532(除Sign外所有参数的url拼接(如：a=1&amp;b=2,不编码) + key + 时间戳(毫秒)).ToLower()
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="secret"></param>
        /// <param name="timestamp"></param>
        /// <param name="filterEmpty">过滤空参数</param>
        /// <returns></returns>
        public static string Sign(this IDictionary<string, object> dict, string secret, long timestamp,
            bool filterEmpty = true)
        {
            if (filterEmpty)
                dict = dict.Where(t => t.Value != null && !string.IsNullOrWhiteSpace(t.Value.ToString()))
                    .ToDictionary(k => k.Key, v => v.Value);
            var sortDict = new SortedDictionary<string, object>(dict);
            var array = sortDict.Select(
                t => $"{t.Key}={t.Value.UnEscape()}");
            var unSigned = string.Concat(string.Join("&", array), secret, timestamp);
            CurrentIocManager.CreateLogger(typeof(Constants)).LogInformation($"un_sign:{unSigned}");
            return timestamp + Md5(unSigned).ToLower();
        }

        public static void SetProject(this HttpContext context, ProjectDto dto)
        {
            context.Items.TryAdd(ProjectCacheKey, dto);
        }

        public static void SetProjectCode(this HttpContext context, string code)
        {
            context.Items.TryAdd(ProjectCodeKey, code);
        }

        public static ProjectDto Project(this HttpContext context)
        {
            ProjectDto project = null;
            if (context.Items.TryGetValue(ProjectCacheKey, out var value))
                project = value as ProjectDto;
            if (project == null)
            {
                string code;
                if (!context.Items.TryGetValue(ProjectCodeKey, out var tCode) || tCode == null)
                {
                    code = context.QueryOrForm(ProjectCodeKey, string.Empty);
                }
                else
                {
                    code = tCode.ToString();
                }
                if (!string.IsNullOrWhiteSpace(code))
                {
                    project = context.RequestServices.GetService<IProjectContract>().DetailByCodeAsync(code).Result;
                    if (project != null)
                        context.SetProject(project);
                }
            }
            return project;
        }
    }
}

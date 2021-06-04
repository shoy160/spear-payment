using Spear.Payment.Contracts.Enums;
using Spear.WebApi.ViewModels;

namespace Spear.Gateway.Payment.Areas.Manage.Models
{
    /// <summary> 项目列表参数 </summary>
    public class VProjectListInput : VPageInput
    {
        /// <summary> 项目编码/项目名称 </summary>
        public string Keyword { get; set; }
        /// <summary> 项目状态 </summary>
        public CommonStatus? Status { get; set; }
    }
}

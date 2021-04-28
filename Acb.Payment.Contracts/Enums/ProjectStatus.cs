using System.ComponentModel;

namespace Acb.Payment.Contracts.Enums
{
    /// <summary> 项目状态 </summary>
    public enum ProjectStatus : byte
    {
        /// <summary> 正常 </summary>
        [Description("正常")]
        Normal = 0,
        /// <summary> 删除 </summary>
        [Description("删除")]
        Delete = 4
    }
}

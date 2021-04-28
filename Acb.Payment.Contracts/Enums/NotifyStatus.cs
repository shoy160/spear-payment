using System.ComponentModel;

namespace Acb.Payment.Contracts.Enums
{
    /// <summary> 通知状态 </summary>
    public enum NotifyStatus : byte
    {
        /// <summary> 默认状态 </summary>
        [Description("失败")] Fail = 0,
        /// <summary> 默认状态 </summary>
        [Description("成功")] Success = 1,
    }
}

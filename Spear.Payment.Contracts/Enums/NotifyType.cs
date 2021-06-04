using System.ComponentModel;

namespace Spear.Payment.Contracts.Enums
{
    /// <summary> 通知类型 </summary>
    public enum NotifyType : byte
    {
        /// <summary> 接收 </summary>
        [Description("接收")]
        Receive = 0,
        /// <summary> 发送 </summary>
        [Description("发送")]
        Send = 1
    }
}

using System.ComponentModel;

namespace Spear.Payment.Contracts.Enums
{
    public enum ChannelStatus : byte
    {
        /// <summary> 已创建 </summary>
        [Description("已创建")]
        Create = 0,
        /// <summary> 已验证 </summary>
        [Description("已验证")]
        Verify = 1,
        /// <summary> 已启用 </summary>
        [Description("已启用")]
        Enable = 2,
        /// <summary> 已删除 </summary>
        [Description("已删除")]
        Delete = 4
    }
}

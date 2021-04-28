namespace Acb.Payment.Contracts.Enums
{
    public enum AccountRole : byte
    {
        /// <summary> 项目账号 </summary>
        Project = 1 << 0,
        /// <summary> 管理员账号 </summary>
        Admin = 1 << 6
    }
}

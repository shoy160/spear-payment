namespace Acb.Gateway.Payment.Areas.Manage.Models
{
    public class VChangePwdInput
    {
        /// <summary>
        /// 原密码
        /// </summary>
        public string OldPwd { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPwd { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Acb.Gateway.Payment.Areas.Manage.Models
{
    /// <summary> 登录 </summary>
    public class VLoginInput : VPasswordInput
    {
        /// <summary> 账号 </summary>
        [Required(ErrorMessage = "请输入登录账号")]
        public string Account { get; set; }
    }

    public class VPasswordInput
    {
        /// <summary> 密码 </summary>
        [Required(ErrorMessage = "请输入登录密码")]
        public string Password { get; set; }
    }
}

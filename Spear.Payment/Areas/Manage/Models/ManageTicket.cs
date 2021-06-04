using Spear.WebApi;
using System;

namespace Spear.Gateway.Payment.Areas.Manage.Models
{
    public class ManageTicket : DClientTicket<string>
    {
        /// <summary> 项目ID </summary>
        public string ProjectId { get; set; }
        /// <summary> 头像 </summary>
        public string Avatar { get; set; }
    }
}

using Acb.WebApi;
using System;

namespace Acb.Gateway.Payment.Areas.Manage.Models
{
    public class ManageTicket : DClientTicket<string>
    {
        /// <summary> 项目ID </summary>
        public string ProjectId { get; set; }
        /// <summary> 头像 </summary>
        public string Avatar { get; set; }
    }
}

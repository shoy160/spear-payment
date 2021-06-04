using Spear.Core;
using Spear.Gateway.Payment.Areas.Manage.Models;
using Spear.WebApi;
using Spear.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Spear.Gateway.Payment.Areas.Manage.Controllers
{
    [DAuthorize]
    [Area("manage"), Route("[area]/[controller]")]
    [ApiExplorerSettings(GroupName = "manage")]
    public class DController : DAuthorController<ManageTicket>
    {

        private const string DefaultErrorMessage = "操作失败，请刷新重试";
        protected DResult FormatResult(int result, string message = null)
        {
            return result > 0
                ? DResult.Success
                : DResult.Error(string.IsNullOrWhiteSpace(message) ? DefaultErrorMessage : message);
        }
    }
}

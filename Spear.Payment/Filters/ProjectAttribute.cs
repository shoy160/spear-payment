using Spear.Core.Exceptions;
using Spear.Gateway.Payment.ViewModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace Spear.Gateway.Payment.Filters
{
    public class ProjectAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
            {
                if (descriptor.MethodInfo.CustomAttributes.Any(t => t.AttributeType == typeof(AllowAnonymousAttribute)))
                {
                    await base.OnActionExecutionAsync(context, next);
                }
            }
            else if (context.ActionDescriptor.FilterDescriptors.Any(t => t.Filter.GetType() == typeof(AllowAnonymousFilter)))
            {
                await base.OnActionExecutionAsync(context, next);
            }
            var project = context.HttpContext.Project();
            if (project == null)
            {
                throw new BusiException("项目不存在");
            }
            await base.OnActionExecutionAsync(context, next);
        }
    }
}

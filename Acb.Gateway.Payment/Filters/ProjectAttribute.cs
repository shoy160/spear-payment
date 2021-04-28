using Acb.Core.Exceptions;
using Acb.Gateway.Payment.ViewModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace Acb.Gateway.Payment.Filters
{
    public class ProjectAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionDescriptor.FilterDescriptors.Any(t => t.Filter.GetType() == typeof(AllowAnonymousFilter)))
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

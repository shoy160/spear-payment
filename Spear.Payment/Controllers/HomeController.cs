using Spear.Payment.Contracts.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
namespace Spear.Gateway.Payment.Controllers
{
    [Route("home"), ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        /// <summary> 平台登录 </summary>
        /// <param name="id"></param>
        /// <param name="mode"></param>
        /// <param name="redirect"></param>
        /// <returns></returns>
        [HttpGet("platform/login")]
        public IActionResult PlatformLogin(string id, PaymentMode mode, string redirect)
        {
            return Redirect("");
        }
    }
}

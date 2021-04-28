using Acb.AutoMapper;
using Acb.Core;
using Acb.Gateway.Payment.Areas.Manage.Models;
using Acb.Payment.Contracts;
using Acb.Payment.Contracts.Dtos;
using Acb.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Acb.Gateway.Payment.Areas.Manage.Controllers
{
    [Route("[area]/account")]
    public class AccountController : DController
    {
        private readonly IAccountContract _accountContract;

        public AccountController(IAccountContract accountContract)
        {
            _accountContract = accountContract;
        }

        /// <summary> 登录 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("login"), AllowAnonymous]
        public async Task<DResult<string>> Login([FromBody] VLoginInput input)
        {
            var dto = await _accountContract.LoginAsync(input.Account, input.Password);
            var client = new ManageTicket
            {
                Id = dto.Id,
                Name = dto.Account,
                Role = dto.Role.ToString(),
                ProjectId = dto.ProjectId,
                Avatar = dto.Avatar
            };
            return Succ(client.Ticket());
        }

        /// <summary> 检查登录密码 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("check")]
        public async Task<DResult> CheckPwd([FromBody] VPasswordInput input)
        {
            await _accountContract.CheckPasswordAsync(Client.Id, input.Password);
            return Success;
        }

        /// <summary>
        /// 更新密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("pwd")]
        public async Task<DResult> ChangePwd([FromBody] VChangePwdInput input)
        {
            var result = await _accountContract.ChangePasswordAsync(Client.Id, input.OldPwd, input.NewPwd);
            return result > 0 ? Success : Error("更新密码失败");
        }

        /// <summary> 获取登陆账号信息 </summary>
        /// <returns></returns>
        [HttpGet]
        public DResult<AccountDto> Detail()
        {
            var dto = Client.MapTo<AccountDto>();
            return Succ(dto);
        }
    }
}

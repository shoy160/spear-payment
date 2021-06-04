using Spear.Core;
using Spear.Core.Dependency;
using Spear.Payment.Contracts.Dtos;
using Spear.Payment.Contracts.Enums;
using System.Threading.Tasks;

namespace Spear.Payment.Contracts
{
    public interface IAccountContract : IDependency
    {
        /// <summary> 帐号登录 </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<AccountDto> LoginAsync(string account, string password);

        /// <summary> 帐号注册 </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        Task<AccountDto> RegistAsync(AccountInputDto inputDto);

        /// <summary> 设置登录密码 </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<int> SetPasswordAsync(string id, string password);

        /// <summary>
        /// 更改密码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        Task<int> ChangePasswordAsync(string id, string oldPassword, string newPassword);

        /// <summary> 更新登录密码 </summary>
        /// <param name="id"></param>
        /// <param name="code"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<int> RegistProjectAsync(string id, string code, string password);

        /// <summary> 获取帐号列表 </summary>
        /// <param name="status"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        Task<PagedList<AccountDto>> ListAsync(CommonStatus? status = null, int page = 1, int size = 10);

        /// <summary> 检查登录密码 </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task CheckPasswordAsync(string id, string password);
    }
}

using Acb.AutoMapper;
using Acb.Core;
using Acb.Core.Domain;
using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Core.Timing;
using Acb.Payment.Business.Domain.Entities;
using Acb.Payment.Business.Domain.Repositories;
using Acb.Payment.Contracts;
using Acb.Payment.Contracts.Dtos;
using Acb.Payment.Contracts.Enums;
using System;
using System.Threading.Tasks;

namespace Acb.Payment.Business
{
    public class AccountService : DService, IAccountContract
    {
        private readonly AccountRepository _accountRepository;

        public AccountService(AccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        private static string EncryptPassword(string password, string salt)
        {
            return $"{password},{salt}".Md5();
        }

        public async Task<AccountDto> LoginAsync(string account, string password)
        {
            var model = await _accountRepository.QueryByAccountAsync(account);
            if (model == null)
                throw new BusiException("登录帐号不存在");
            var pwd = EncryptPassword(password, model.PasswordSalt);
            if (!string.Equals(pwd, model.Password, StringComparison.CurrentCultureIgnoreCase))
                throw new BusiException("登录密码不正确");
            await _accountRepository.UpdateLoginTimeAsync(model.Id);
            return model.MapTo<AccountDto>();
        }

        public async Task<AccountDto> RegistAsync(AccountInputDto inputDto)
        {
            var mode = await _accountRepository.QueryByAccountAsync(inputDto.Account);
            if (mode != null)
                throw new BusiException("登陆账号已存在");
            var model = inputDto.MapTo<TAccount>();
            model.Id = inputDto.Id ?? IdentityHelper.Guid32;
            model.PasswordSalt = IdentityHelper.Guid16;
            model.Password = EncryptPassword(inputDto.Password, model.PasswordSalt);
            model.Status = (byte)CommonStatus.Normal;
            model.CreateTime = Clock.Now;
            var result = await _accountRepository.InsertAsync(model);
            if (result > 0)
                return model.MapTo<AccountDto>();
            throw new BusiException("注册失败");
        }

        public Task<int> SetPasswordAsync(string id, string password)
        {
            var salt = IdentityHelper.Guid16;
            password = EncryptPassword(password, salt);
            return _accountRepository.UpdatePasswordAsync(id, password, salt);
        }

        public async Task<int> RegistProjectAsync(string id, string code, string password)
        {
            var mode = await _accountRepository.QueryByAccountAsync(code);
            if (mode != null)
                return await SetPasswordAsync(id, password);
            await RegistAsync(new AccountInputDto
            {
                Id = id,
                Account = code,
                Password = password,
                ProjectId = id,
                Role = AccountRole.Project
            });
            return 1;

        }

        public async Task<PagedList<AccountDto>> ListAsync(CommonStatus? status = null, int page = 1, int size = 10)
        {
            var models = await _accountRepository.QueryByStatusAsync(status, page, size);
            return models.MapPagedList<AccountDto, TAccount>();
        }

        public async Task CheckPasswordAsync(string id, string password)
        {
            var model = await _accountRepository.QueryByIdAsync(id);
            if (model == null)
                throw new BusiException("帐号不存在");
            var pwd = EncryptPassword(password, model.PasswordSalt);
            if (!string.Equals(pwd, model.Password, StringComparison.CurrentCultureIgnoreCase))
                throw new BusiException("登录密码不正确");
        }

        public async Task<int> ChangePasswordAsync(string id, string oldPassword, string newPassword)
        {
            await CheckPasswordAsync(id, oldPassword);
            return await SetPasswordAsync(id, newPassword);
        }
    }
}

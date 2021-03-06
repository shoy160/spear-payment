using Spear.Payment.Contracts;
using Spear.Payment.Contracts.Dtos;
using Spear.Payment.Contracts.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Spear.Payment.Tests
{
    [TestClass]
    public class AccountServiceTest : DTest
    {
        private readonly IAccountContract _accountContract;

        public AccountServiceTest()
        {
            _accountContract = Resolve<IAccountContract>();
        }

        [TestMethod]
        public async Task RegistTest()
        {
            var dto = await _accountContract.RegistAsync(new AccountInputDto
            {
                Account = "shay",
                Password = "123456",
                Role = AccountRole.Admin,
                //ProjectId = "55f020eb-4bb1-c842-7219-08d62f809717"
            });
            Print(dto);
        }
    }
}

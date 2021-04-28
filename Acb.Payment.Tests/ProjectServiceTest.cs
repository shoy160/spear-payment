using Acb.Core.Dependency;
using Acb.Payment.Contracts;
using Acb.Payment.Contracts.Dtos;
using Acb.Payment.Contracts.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acb.Core.Serialize;

namespace Acb.Payment.Tests
{
    [TestClass]
    public class ProjectServiceTest : DTest
    {
        private readonly IProjectContract _projectContract;

        public ProjectServiceTest()
        {
            _projectContract = CurrentIocManager.Resolve<IProjectContract>();
        }

        [TestMethod]
        public async Task CreateTest()
        {
            var result = await _projectContract.CreateAsync(new ProjectInputDto
            {
                Code = "wanglaidan",
                Channels = new List<string>
                {
                    "882752fd-e41c-c675-bb9b-08d62dfc7ea2",
                    "5a5a2f8d-bbe2-c9d9-cf1b-08d62f6ef870"
                },
                Name = "网来单",
                NotifyUrl = string.Empty,
                RedirectUrl = string.Empty,
                QueueName = string.Empty
            });
            Print(result);
        }

        [TestMethod]
        public async Task DetailByCodeTest()
        {
            var detail = await _projectContract.DetailByCodeAsync("icbhs");
            Print(detail);
        }

        [TestMethod]
        public async Task DetailByIdTest()
        {
            var detail = await _projectContract.DetailByIdAsync("091480df-51f4-c3f3-8555-08d6128aaaaa");
            Print(detail);
        }
    }
}

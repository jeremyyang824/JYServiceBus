using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wind.iSeller.Data.Core.Commands.Meta;
using Wind.iSeller.Data.Core.Commands.SysUser;
using Wind.iSeller.Data.Core.Dtos.Meta;
using Wind.iSeller.Data.Core.Services.Meta;
using Wind.iSeller.Data.Test.Common;

namespace Wind.iSeller.Data.Test.ServiceUnitTests
{
    [TestClass]
    public class SysUserServiceTest : TestBase<DataTestModule>
    {
        private readonly SysUserService sysUserService;

        public SysUserServiceTest()
        {
            this.sysUserService = Resolve<SysUserService>();
        }

        [TestMethod]
        public virtual void GetSysUserByWftIdCommandTest()
        {
            var command = new GetSysUserByWftIdCommand
            {
                wftids = new string[] { "W0813710" }
            };

            var result = sysUserService.HandlerCommand(command);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var userBean = result.FirstOrDefault();
            Assert.AreEqual("W0813710", userBean.wftid);
        }

        [TestMethod]
        public virtual void GetSysUserByWftIdSingleCommandTest()
        {
            var command = new GetSysUserByWftIdSingleCommand
            {
                wftId = "W0813710"
            };

            var userBean = sysUserService.HandlerCommand(command);
            Assert.IsNotNull(userBean);
            Assert.AreEqual("W0813710", userBean.wftid);
        }

        [TestMethod]
        public virtual void UpdateSysUserCommandTest()
        {
            //GetUser
            var userDto = sysUserService.HandlerCommand(
                new GetSysUserByWftIdCommand
                {
                    wftids = new string[] { "W0813710" }
                })
                .FirstOrDefault();
            Assert.IsNotNull(userDto);

            //Update User
            userDto.username = "吕时正";
            var result = sysUserService.HandlerCommand(
                new UpdateSysUserCommand
                {
                    user = userDto
                });
            Assert.IsNotNull(result);
            Assert.AreEqual("吕时正", result.username);
            Assert.AreEqual("Wind资讯", result.institudename);
        }

        [TestMethod]
        public virtual void UpdateSysUsersBatchCommandTest()
        {
            var userDto1 = sysUserService.HandlerCommand(
                new GetSysUserByWftIdCommand
                {
                    wftids = new string[] { "W0813710" }
                })
                .FirstOrDefault();
            Assert.IsNotNull(userDto1);

            var userDto2 = sysUserService.HandlerCommand(
                new GetSysUserByWftIdCommand
                {
                    wftids = new string[] { "W0818564" }
                })
                .FirstOrDefault();
            Assert.IsNotNull(userDto2);

            //Update User Batch
            userDto1.username = "吕时正";
            userDto2.username = "丁旺荣";

            var result = sysUserService.HandlerCommand(
                new UpdateSysUsersBatchCommand
                {
                    users = new List<SysUserDto> { userDto1, userDto2 }
                });
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 2);

            var resultDto1 = result[0];
            Assert.AreEqual("吕时正", resultDto1.username);
            Assert.AreEqual("Wind资讯", resultDto1.institudename);

            var resultDto2 = result[1];
            Assert.AreEqual("丁旺荣", resultDto2.username);
            Assert.AreEqual("Wind资讯", resultDto2.institudename);
        }

        [TestMethod]
        public virtual void InsertSysUserCommandResultTest()
        {
            var addResult1 = this.sysUserService.HandlerCommand(new InsertSysUserCommand
            {
                user = new SysUserDto
                {
                    userid = Guid.NewGuid().ToString(),
                    username = "测试1027",
                    wftid = "WFT1027"
                }
            });
            Assert.IsNotNull(addResult1);

            //删除
            addResult1.isdel = 1;
            this.sysUserService.HandlerCommand(new UpdateSysUserCommand
            {
                user = addResult1
            });

            //尝试再次插入
            var addResult2 = this.sysUserService.HandlerCommand(new InsertSysUserCommand
            {
                user = new SysUserDto
                {
                    userid = Guid.NewGuid().ToString(),
                    username = "测试1027A",
                    wftid = "WFT1027"
                }
            });
            Assert.IsNotNull(addResult2);
        }
    }
}

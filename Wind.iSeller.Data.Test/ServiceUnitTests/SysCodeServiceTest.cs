using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wind.iSeller.Data.Core.Commands.Meta;
using Wind.iSeller.Data.Core.Services.Meta;
using Wind.iSeller.Data.Test.Common;

namespace Wind.iSeller.Data.Test.ServiceUnitTests
{
    [TestClass]
    public class SysCodeServiceTest : TestBase<DataTestModule>
    {
        private readonly SysCodeService sysCodeService;

        public SysCodeServiceTest()
        {
            this.sysCodeService = Resolve<SysCodeService>();
        }

        [TestMethod]
        public virtual void TestGetSysCodeByConkeyCommand()
        {
            var command = new GetSysCodeByConkeyCommand
            {
                conkey = "one_egg_factor"
            };

            var result = sysCodeService.HandlerCommand(command);
            Assert.IsNotNull(result);
            Assert.AreEqual("中奖概率调整系数", result.conkeydes);
        }
    }
}

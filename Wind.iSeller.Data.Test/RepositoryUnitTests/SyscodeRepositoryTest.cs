using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wind.iSeller.Framework.Core.Domain.Repositories;
using Wind.iSeller.Data.Core.Entities.Meta;
using Wind.iSeller.Data.Test.Common;

namespace Wind.iSeller.Data.Test.RepositoryUnitTests
{
    [TestClass]
    public class SyscodeRepositoryTest : TestBase<DataTestModule>
    {
        private readonly IRepository<SysCode, string> syscodeRepository;

        public SyscodeRepositoryTest()
        {
            this.syscodeRepository = Resolve<IRepository<SysCode, string>>();
        }

        [TestMethod]
        public virtual void Getsyscode()
        {
            var bean = this.syscodeRepository.Get("b71fc7ee-33d3-4dc5-a957-366ae24009b0");
            Assert.IsNotNull(bean);
            Assert.AreEqual("中奖概率调整系数", bean.conkeydes);
            Assert.AreEqual("one_egg_factor", bean.conkey);
        }
    }
}
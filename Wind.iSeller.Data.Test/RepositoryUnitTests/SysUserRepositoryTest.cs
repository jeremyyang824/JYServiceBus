using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wind.iSeller.Data.Core.Entities.Meta;
using Wind.iSeller.Data.Test.Common;
using Wind.iSeller.Framework.Core.Domain.Repositories;

namespace Wind.iSeller.Data.Test.RepositoryUnitTests
{
    [TestClass]
    public class SysUserRepositoryTest : TestBase<DataTestModule>
    {
        private readonly IRepository<SysUser, string> sysUserRepository;

        public SysUserRepositoryTest()
        {
            this.sysUserRepository = Resolve<IRepository<SysUser, string>>();
        }

        [TestMethod]
        public virtual void GetUser()
        {
            var bean = this.sysUserRepository.Get("10000092@wind.com.cn");
            Assert.IsNotNull(bean);
            Assert.AreEqual("童伟", bean.username);
        }
    }
}

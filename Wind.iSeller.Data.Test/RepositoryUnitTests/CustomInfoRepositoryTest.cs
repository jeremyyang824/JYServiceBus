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
    public class CustomInfoRepositoryTest : TestBase<DataTestModule>
    {
        private readonly IRepository<CustomInfo, string> cusinfoRepository;

        public CustomInfoRepositoryTest()
        {
            this.cusinfoRepository = Resolve<IRepository<CustomInfo, string>>();
        }

        [TestMethod]
        public virtual void GetCustomInfo()
        {
            var bean = this.cusinfoRepository.Get("74167ffe-5d85-42d1-a903-4a9b582baaf0");
            Assert.IsNotNull(bean);
            Assert.AreEqual("郑超", bean.cusname);
        }
    }
}

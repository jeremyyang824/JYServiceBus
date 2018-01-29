using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wind.iSeller.Data.Core.Entities.Point;
using Wind.iSeller.Data.Test.Common;
using Wind.iSeller.Framework.Core.Domain.Repositories;

namespace Wind.iSeller.Data.Test.RepositoryUnitTests
{
    [TestClass]
    public class PointLogRepositoryTest : TestBase<DataTestModule>
    {
        private readonly IRepository<PointLog, string> pointlogRepository;

        public PointLogRepositoryTest()
        {
            this.pointlogRepository = Resolve<IRepository<PointLog, string>>();
        }

        [TestMethod]
        public virtual void GetPointLog()
        {
            var bean = this.pointlogRepository.Get("a3fa510b-18c5-4f52-b0be-1b07800738c4");
            Assert.IsNotNull(bean);
            Assert.AreEqual(2, bean.logtype);
            Assert.AreEqual("W0816921", bean.wftid);
        }
    }
}

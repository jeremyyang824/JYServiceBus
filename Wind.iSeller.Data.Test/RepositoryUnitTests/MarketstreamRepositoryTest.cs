using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wind.iSeller.Data.Core.Entities.Order;
using Wind.iSeller.Data.Test.Common;
using Wind.iSeller.Framework.Core.Domain.Repositories;

namespace Wind.iSeller.Data.Test.RepositoryUnitTests
{
    [TestClass]
    public class MarketstreamRepositoryTest : TestBase<DataTestModule>
    {
        private readonly IRepository<Marketstream, string> marketstreamRepository;

        public MarketstreamRepositoryTest()
        {
            this.marketstreamRepository = Resolve<IRepository<Marketstream, string>>();
        }

        [TestMethod]
        public virtual void Getmarketstream()
        {
            var bean = this.marketstreamRepository.Get("2BF7B129318F4894B7186F18A65C06A8");
            Assert.IsNotNull(bean);
            Assert.AreEqual("17河南债10", bean.productname);
        }
    }
}

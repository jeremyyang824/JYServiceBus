using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wind.iSeller.Framework.Core.Domain.Repositories;
using Wind.iSeller.Data.Core.Entities.Order;
using Wind.iSeller.Data.Test.Common;

namespace Wind.iSeller.Data.Test.UnitTests
{
    [TestClass]
    public class OrderListRepositoryTest : TestBase<DataTestModule>
    {
        private readonly IRepository<OrderList, string> OrderlistRepository;

        public OrderListRepositoryTest()
        {
            this.OrderlistRepository = Resolve<IRepository<OrderList, string>>();
        }

        [TestMethod]
        public virtual void GetOrderlist()
        {
            var bean = this.OrderlistRepository.Get("eda9cafc-23b5-4120-a9e5-c4494062abf2");
            Assert.IsNotNull(bean);
            Assert.AreEqual("W0812467", bean.buyercrmid);
            Assert.AreEqual("20160914100317708", bean.orderno);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wind.iSeller.Framework.Core.Domain.Repositories;
using Wind.iSeller.Data.Core.Entities.Order;
using Wind.iSeller.Data.Test.Common;
using Wind.iSeller.Data.Core.Repositories.Order;

namespace Wind.iSeller.Data.Test.UnitTests
{
    [TestClass]
    public class OrderDetailRepositoryTest : TestBase<DataTestModule>
    {
        private readonly OrderDetailRepository OrderdetailRepository;

        public OrderDetailRepositoryTest()
        {
            this.OrderdetailRepository = Resolve<OrderDetailRepository>();
        }

        [TestMethod]
        public virtual void GetOrderdetail()
        {
            var bean = this.OrderdetailRepository.Get("3f01d3b7-cd44-4d56-817d-d6cec863a19e");
            Assert.IsNotNull(bean);
            Assert.AreEqual("16牧原股份MTN001", bean.productname);
            Assert.AreEqual(5, bean.amount);
            Assert.AreEqual(OrderDetailStatus.UnConfirmed, bean.status);

            Assert.IsNotNull(bean.Order);
            Assert.AreEqual("10000098@wind.com.cn", bean.Order.buyeriwindid);
            Assert.AreEqual("ed9c5cfb-377e-4251-9dff-055e26879511", bean.Order.buyercontactaccid);
        }

        [TestMethod]
        public virtual void QueryOrderDetail()
        {
            int totalCount = 0;
            var list = this.OrderdetailRepository.QueryOrderDetail("17aa1815-b4cc-46c4-a31d-82e1b9460cf6",
                od => (od.status == OrderDetailStatus.Bidded || od.status == OrderDetailStatus.UnBidded),
                null, 20, 10, out totalCount);

            Assert.IsTrue(list.Count > 0);
            Assert.IsTrue(totalCount > 0);
        }
    }
}
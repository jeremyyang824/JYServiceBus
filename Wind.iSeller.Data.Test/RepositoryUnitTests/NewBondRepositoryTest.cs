using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wind.iSeller.Framework.Core.Domain.Repositories;
using Wind.iSeller.Data.Core.Entities.Product;
using Wind.iSeller.Data.Core.Repositories.Product;
using Wind.iSeller.Data.Test.Common;

namespace Wind.iSeller.Data.Test.RepositoryUnitTests
{
    [TestClass]
    public class NewbondRepositoryTest : TestBase<DataTestModule>
    {
        private readonly NewBondRepository NewbondRepository;
        private readonly IRepository<NewBondLink, string> NewbondLinkRepository;

        public NewbondRepositoryTest()
        {
            this.NewbondRepository = Resolve<NewBondRepository>();
            this.NewbondLinkRepository = Resolve<IRepository<NewBondLink, string>>();
        }

        [TestMethod]
        public virtual void GetNewbond()
        {
            var bean = this.NewbondRepository.Get("S6983704");
            Assert.IsNotNull(bean);
            Assert.AreEqual("011698782.IB", bean.windcode);
            Assert.AreEqual("16海沧投资SCP003", bean.productname);
        }

        [TestMethod]
        public virtual void QueryBond()
        {
            var totalCount = 0;
            var list = this.NewbondRepository.QueryNewBondWithMallLink("eff9a529-fb31-4d39-adbd-e298a05b6c94",
                b => b.Id == "S5205526",
                null, 0, 10, out totalCount);

            Assert.IsTrue(list.Count > 0);
            Assert.IsTrue(totalCount != 0);

            var bondBean = list.First();
            Assert.IsTrue(bondBean.BondLinkList.Count == 1 && bondBean.BondLinkList[0].mallid == "eff9a529-fb31-4d39-adbd-e298a05b6c94");
        }

        [TestMethod]
        public virtual void QueryBondPager()
        {
            int page = 3, pageSize = 10;
            var query = this.NewbondRepository.GetAll()
                //条件
                .Where(b => b.bondtype == "短期融资券" && b.windcode.EndsWith(".IB") && b.islisted == "是");

            //总记录数
            var totalCount = query.Count();

            //分页
            var pagerList = query
                .OrderByDescending(b => b.createtime).ThenBy(b => b.windcode)
                .Skip((page - 1) * pageSize).Take(pageSize).ToList();

            Assert.IsTrue(totalCount > 0);
            Assert.IsTrue(pagerList.Count > 0 && pagerList.Count <= 10);

            if (pagerList.Count > 0)
            {
                var item = pagerList.First();
                Assert.AreEqual("是", item.islisted);
                Assert.AreEqual("短期融资券", item.bondtype);
                Assert.IsTrue(item.windcode.EndsWith(".IB"));
            }
        }
    }
}

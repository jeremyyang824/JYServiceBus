using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wind.iSeller.Framework.Core.Application.Services.Dto;
using Wind.iSeller.Framework.Core.Domain.Repositories;
using Wind.iSeller.Framework.Core.Domain.Uow;
using Wind.iSeller.Data.Core.Entities.Product;
using Wind.iSeller.Data.Test.Common;

namespace Wind.iSeller.Data.Test.RepositoryUnitTests
{
    [Obsolete("Use NewBondProduct entity")]
    [TestClass]
    public class BondProductRepositoryTest : TestBase<DataTestModule>
    {
        private readonly IRepository<BondProduct, string> bondProductRepository;

        public BondProductRepositoryTest()
        {
            this.bondProductRepository = Resolve<IRepository<BondProduct, string>>();
        }

        [TestMethod]
        public virtual void GetBond()
        {
            var bondProduct = this.bondProductRepository.Get("5fce5976-40a3-49ae-b29d-ad2009ba8a2c");
            Assert.IsNotNull(bondProduct);
            Assert.AreEqual("中合中小企业融资担保股份有限公司2016年公司债券(第一期)", bondProduct.FullName);
        }

        [TestMethod]
        public virtual void QueryBondPager()
        {
            int page = 3, pageSize = 10;
            var query = this.bondProductRepository.GetAll()
                //条件
                .Where(b => b.BondType == "短期融资券" && b.ProductCode.EndsWith(".IB") && b.IsDeleted == false);

            //总记录数
            var totalCount = query.Count();

            //分页
            var pagerList = query
                .OrderByDescending(b => b.CreateTime).ThenBy(b => b.ProductCode)
                .Skip((page - 1) * pageSize).Take(pageSize).ToList();

            Assert.IsTrue(totalCount > 0);
            Assert.IsTrue(pagerList.Count > 0 && pagerList.Count <= 10);

            if (pagerList.Count > 0)
            {
                var item = pagerList.First();
                Assert.IsFalse(item.IsDeleted);
                Assert.AreEqual("短期融资券", item.BondType);
                Assert.IsTrue(item.ProductCode.EndsWith(".IB"));

                //多对一关联（延迟加载测试）
                Assert.IsNotNull(item.Mall);
                Assert.IsNotNull(item.MktType);
            }
        }

        [TestMethod]
        public virtual void QueryBondReference()
        {
            var query = this.bondProductRepository
                .GetAllIncluding(b => b.Mall, b => b.MktType)
                .Where(b => b.IsDeleted == false)
                .Take(10).ToList();

            Assert.IsTrue(query.Count > 0);

            var bondBean = query.First();
            Assert.IsNotNull(bondBean.Mall);
            Assert.IsNotNull(bondBean.MktType);
        }
    }
}

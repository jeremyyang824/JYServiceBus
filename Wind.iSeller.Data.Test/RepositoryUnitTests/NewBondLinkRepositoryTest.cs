using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wind.iSeller.Framework.Core.Domain.Repositories;
using Wind.iSeller.Data.Core.Entities.Product;
using Wind.iSeller.Data.Test.Common;

namespace Wind.iSeller.Data.Test.RepositoryUnitTests
{
    [TestClass]
    public class NewBondLinkRepositoryTest : TestBase<DataTestModule>
    {
        private readonly IRepository<NewBondLink, string> NewbondlinkRepository;

        public NewBondLinkRepositoryTest()
        {
            this.NewbondlinkRepository = Resolve<IRepository<NewBondLink, string>>();
        }

        [TestMethod]
        public virtual void GetNewbondlink()
        {
            var bean = this.NewbondlinkRepository
                .GetAllIncluding(bl => bl.BondProduct, bl => bl.Mall)
                .FirstOrDefault(bl => bl.Id == "cf321cc1-2f5c-418f-8194-4fbe4491893d");
                //.Get("66357a95-12fa-4129-a284-6b57fa489944");
            Assert.IsNotNull(bean);
            Assert.AreEqual("S8016570", bean.bondproductid);

            Assert.IsNotNull(bean.BondProduct);
            Assert.AreEqual("17臻元2B", bean.BondProduct.productname);

            Assert.IsNotNull(bean.Mall);
            Assert.AreEqual("招商银行旗舰店", bean.Mall.mallname);
        }
    }
}
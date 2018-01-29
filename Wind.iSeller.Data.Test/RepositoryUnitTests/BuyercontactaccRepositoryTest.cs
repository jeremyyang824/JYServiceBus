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
    public class BuyercontactaccRepositoryTest : TestBase<DataTestModule>
    {
        private readonly IRepository<BuyerContactAcc, string> buyercontactaccRepository;

        public BuyercontactaccRepositoryTest()
        {
            this.buyercontactaccRepository = Resolve<IRepository<BuyerContactAcc, string>>();
        }

        [TestMethod]
        public virtual void Getbuyercontactacc()
        {
            var bean = this.buyercontactaccRepository.Get("ea5a6893-7e12-4666-855e-a917bd07c54e");
            Assert.IsNotNull(bean);
            Assert.AreEqual("天堂向左", bean.buyerorgname);
        }
    }
}

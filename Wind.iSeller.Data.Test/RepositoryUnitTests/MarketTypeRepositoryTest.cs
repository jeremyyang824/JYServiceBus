using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wind.iSeller.Framework.Core.Domain.Repositories;
using Wind.iSeller.Data.Core.Entities.Meta;
using Wind.iSeller.Data.Test.Common;

namespace Wind.iSeller.Data.Test.RepositoryUnitTests
{
    [TestClass]
    public class MarketTypeRepositoryTest : TestBase<DataTestModule>
    {
        private readonly IRepository<MarketType, string> marketTypeRepository;

        public MarketTypeRepositoryTest()
        {
            this.marketTypeRepository = Resolve<IRepository<MarketType, string>>();
        }

        [TestMethod]
        public virtual void GetAllMarketType()
        {
            var marketTypeList = this.marketTypeRepository.GetAll()
                .Where(mt => mt.IsDeleted == false)
                .ToList();

            Assert.IsTrue(marketTypeList.Count > 0);
        }
    }
}

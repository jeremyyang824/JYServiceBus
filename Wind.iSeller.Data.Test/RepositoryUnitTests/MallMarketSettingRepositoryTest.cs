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
    public class MallMarketSettingRepositoryTest : TestBase<DataTestModule>
    {
        private readonly IRepository<MallMarketSetting, string> mallMarketSettingRepository;

        public MallMarketSettingRepositoryTest()
        {
            this.mallMarketSettingRepository = Resolve<IRepository<MallMarketSetting, string>>();
        }

        [TestMethod]
        public virtual void GetMallMarketSetting()
        {
            var mallMktSettings = this.mallMarketSettingRepository.GetAllIncluding(ms => ms.MktType)
                .Where(m => m.Mall.Organization.orgname == "招商银行" && m.IsDeleted == false)
                .OrderBy(m => m.position)
                .ToList();

            Assert.IsTrue(mallMktSettings.Count > 0);
        }
    }
}

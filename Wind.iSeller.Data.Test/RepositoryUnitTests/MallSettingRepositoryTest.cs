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
    public class MallSettingRepositoryTest : TestBase<DataTestModule>
    {
        private readonly IRepository<MallSetting, string> mallSettingRepository;

        public MallSettingRepositoryTest()
        {
            this.mallSettingRepository = Resolve<IRepository<MallSetting, string>>();
        }

        [TestMethod]
        public virtual void GetMallSetting()
        {
            var mall = this.mallSettingRepository.Get("0b7a1602-e51b-4733-94bb-1e6227bf6490");
            Assert.IsNotNull(mall);
            Assert.AreEqual("招商银行旗舰店", mall.mallname);
            Assert.AreEqual(MallStatus.Open, mall.status);
            Assert.AreEqual("招商银行", mall.Organization.orgname);
        }

        [TestMethod]
        public virtual void QueryMall()
        {
            var mall = this.mallSettingRepository.GetAllIncluding(m => m.Organization)
                .Where(m => m.IsDeleted == false && m.status == MallStatus.Open
                    && m.Organization != null
                    && m.Organization.type == OrganizationType.Bank
                    && m.Organization.des.Contains("银行"))
                .ToList();

            Assert.IsTrue(mall.Count > 0);

            var mallBean = mall.First();
            Assert.IsFalse(mallBean.IsDeleted);
            Assert.AreEqual(MallStatus.Open, mallBean.status);
            Assert.AreEqual(OrganizationType.Bank, mallBean.Organization.type);
            Assert.IsTrue(mallBean.Organization.des.Contains("银行"));
        }
    }
}

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
    public class OrganizationRepositoryTest : TestBase<DataTestModule>
    {
        private readonly IRepository<Organization, string> organizationRepository;

        public OrganizationRepositoryTest()
        {
            this.organizationRepository = Resolve<IRepository<Organization, string>>();
        }

        [TestMethod]
        public virtual void GetOrganization()
        {
            var organization = this.organizationRepository.Get("0ed9eaa0-8c6f-4dd5-99d7-0228c922e783");
            Assert.IsNotNull(organization);
            Assert.AreEqual("招商银行", organization.orgname);
        }

        [TestMethod]
        public virtual void QueryOrganization()
        {
            var organization = this.organizationRepository.GetAll()
                .Where(org => org.type != null && org.type == OrganizationType.Bank)
                .ToList();

            Assert.IsTrue(organization.Count > 0);

            var orgBean = organization.First();
            Assert.AreEqual(OrganizationType.Bank, orgBean.type);
        }
    }
}

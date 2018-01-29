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
    public class MallEmployeeRepositoryTest : TestBase<DataTestModule>
    {
        private readonly IRepository<MallEmployee, string> mallemployeeRepository;

        public MallEmployeeRepositoryTest()
        {
            this.mallemployeeRepository = Resolve<IRepository<MallEmployee, string>>();
        }

        [TestMethod]
        public virtual void GetMallEmployee()
        {
            var bean = this.mallemployeeRepository.Get("a972896a-8c5f-480a-8418-1438253348b2");
            Assert.IsNotNull(bean);
            Assert.AreEqual("杜宾宾", bean.name);
        }
    }
}

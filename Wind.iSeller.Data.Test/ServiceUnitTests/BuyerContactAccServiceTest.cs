using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wind.iSeller.Data.Core.Commands.Customer;
using Wind.iSeller.Data.Core.Services.Meta;
using Wind.iSeller.Data.Test.Common;

namespace Wind.iSeller.Data.Test.ServiceUnitTests
{
    [TestClass]
    public class BuyerContactAccServiceTest : TestBase<DataTestModule>
    {
        private readonly BuyerContactAccService buyerContactAccService;

        public BuyerContactAccServiceTest()
        {
            this.buyerContactAccService = Resolve<BuyerContactAccService>();
        }

        [TestMethod]
        public virtual void GetBuyerContactAccByIdCommandTest()
        {
            var result = this.buyerContactAccService.HandlerCommand(new GetBuyerContactAccByIdCommand
            {
                buyerContactAccId = "c38a96dd-803a-426b-8a37-5c174c8c4f3e"
            });
            Assert.IsNotNull(result);
            Assert.AreEqual("万得资讯", result.buyerorgname);
        }

        [TestMethod]
        public virtual void InsertBuyerContactAccCommandTest()
        {
            var result = this.buyerContactAccService.HandlerCommand(new InsertBuyerContactAccCommand
            {
                buyerContactAcc = new Core.Dtos.Meta.BuyerContactAccDto
                {
                    buyercontactaccid = Guid.NewGuid().ToString(),
                    organizationcode = "ccc",
                    buyerorgname = "测试ccc"
                }
            });
            Assert.IsNotNull(result);
        }
    }
}

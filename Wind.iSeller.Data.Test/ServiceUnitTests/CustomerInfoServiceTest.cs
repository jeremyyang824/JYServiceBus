using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wind.iSeller.Data.Core.Commands.Customer;
using Wind.iSeller.Data.Core.Commands.Meta;
using Wind.iSeller.Data.Core.Services.Meta;
using Wind.iSeller.Data.Test.Common;

namespace Wind.iSeller.Data.Test.ServiceUnitTests
{
    [TestClass]
    public class CustomerInfoServiceTest : TestBase<DataTestModule>
    {
        private readonly CustomerInfoService customerInfoService;

        public CustomerInfoServiceTest()
        {
            this.customerInfoService = Resolve<CustomerInfoService>();
        }

        [TestMethod]
        public virtual void GetCustomerInfoByBuyerContactAccIdTest()
        {
            var result = this.customerInfoService.HandlerCommand(new GetCustomerInfoByAccIdCommand
            {
                buyerContactAccId = "6edcea2e-f9cf-4433-97f1-00bcf1855c31"
            });
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
            Assert.AreEqual("W0812467", result.First().wftid);
        }

        [TestMethod]
        public virtual void GetCustomerInfoByCusNameCommandTest()
        {
            var result = this.customerInfoService.HandlerCommand(new GetCustomerInfoByCusNameCommand
            {
                cusName = "郑超",
                cusorg = "天堂向左",
            });
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public virtual void InsertCustomerInfoCommandTest()
        {
            var result = this.customerInfoService.HandlerCommand(new InsertCustomerInfoCommand
            {
                customerInfo = new Core.Dtos.Meta.CustomInfoDto
                {
                    cusinfoid = Guid.NewGuid().ToString(),
                    mallid = "93d6ea84-b0c0-496b-ae54-8bef2ac28269",
                    cusname = "此次测试",
                }
            });
            Assert.IsNotNull(result);
        }
    }
}

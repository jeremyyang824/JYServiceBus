using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wind.iSeller.Data.Core.Commands.Order;
using Wind.iSeller.Data.Core.Dtos.Order;
using Wind.iSeller.Data.Core.Services.Order;
using Wind.iSeller.Data.Test.Common;

namespace Wind.iSeller.Data.Test.ServiceUnitTests
{
    [TestClass]
    public class MarketStreamServiceTest : TestBase<DataTestModule>
    {
        private readonly MarketStreamService marketStreamService;

        public MarketStreamServiceTest()
        {
            this.marketStreamService = Resolve<MarketStreamService>();
        }

        [TestMethod]
        public virtual void MarketStreamSearchAggregateCommandTest()
        {
            var result = this.marketStreamService.HandlerCommand(new MarketStreamSearchAggregateCommand
            {
                beginTime = new DateTime(2017, 10, 2),
                mktcode= "BOND_ANNOUNCED",
                mallId = "0b7a1602-e51b-4733-94bb-1e6227bf6490",
            });

            Assert.IsTrue(result != null && result.Count > 0);
        }

        [TestMethod]
        public virtual void CreateMarketStreamCommandTest()
        {
            var cmd1 = new CreateMarketStreamCommand
            {
                productId = "S7914602",
                mallId = "0b7a1602-e51b-4733-94bb-1e6227bf6490",
                orderDetailId = "441e872d-53dd-4e57-90b6-961301246f5d",
                modifyType = Core.Entities.Order.ModifyType.Add,
                operatorID = "W0816921"
            };
            var result1 = this.marketStreamService.HandlerCommand(cmd1);
            Assert.IsNotNull(result1);

            var cmd2 = new CreateMarketStreamCommand
            {
                productId = "S7914602",
                mallId = "0b7a1602-e51b-4733-94bb-1e6227bf6490",
                orderDetailId = "441e872d-53dd-4e57-90b6-961301246f5d",
                modifyType = Core.Entities.Order.ModifyType.Update,
                operatorID = "W0816921"
            };
            var result2 = this.marketStreamService.HandlerCommand(cmd2);
            Assert.IsNotNull(result2);

            var cmd3 = new CreateMarketStreamCommand
            {
                productId = "S7914602",
                mallId = "0b7a1602-e51b-4733-94bb-1e6227bf6490",
                orderDetailId = "441e872d-53dd-4e57-90b6-961301246f5d",
                modifyType = Core.Entities.Order.ModifyType.Update,
                operatorID = "W0816921"
            };
            var result3 = this.marketStreamService.HandlerCommand(cmd3);
            Assert.IsNotNull(result3);
        }
    }
}

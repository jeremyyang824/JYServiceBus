using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wind.iSeller.Data.Core.Commands.Product;
using Wind.iSeller.Data.Core.Dtos.Product;
using Wind.iSeller.Data.Core.Services.Product;
using Wind.iSeller.Data.Test.Common;

namespace Wind.iSeller.Data.Test.ServiceUnitTests
{
    [TestClass]
    public class BondServiceTest : TestBase<DataTestModule>
    {
        private readonly BondService bondService;

        public BondServiceTest()
        {
            this.bondService = Resolve<BondService>();
        }

        [TestMethod]
        public virtual void BondComprehensiveSearchCommandTest()
        {
            var command = new BondComprehensiveSearchCommand
            {
                BondType = Core.Domain.BondType.CreditBond,
                MallId = "0b7a1602-e51b-4733-94bb-1e6227bf6490",
                ProductNameKeyword = "",
                PublishDateRangeBegin = null,   /*20170615*/
                PublishDateRangeEnd = null,
                IsKeyDate = null,
                IsPutaway = false,
                HasOrders = false,
                IsInGroup = true,
                //BondCategorys = new string[] { "短融", "中票", "其他" },
                //EnterpriseTypes = new string[] { "民营企业" },
                //BondPeriodTypes = new string[] { "1年以内", "3-5年" },
                IssuerLevelTypes = new string[] { "AAA", "AA+" },
                IssuerTypes = null,
                SkipCount = 0,
                MaxResultCount = 1000,
                Sorting = null,
            };

            var result = bondService.HandlerCommand(command);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.BondLists.TotalCount > 0);
            Assert.IsTrue(result.BondLists.Items.Count > 0);
        }

        [TestMethod]
        public virtual void GetMallBondByIdCommandTest()
        {
            var result = this.bondService.HandlerCommand(
                new GetMallBondByIdCommand
                {
                    id = "1e89465f-c704-4250-b259-cd75f25fc8f5"
                });

            Assert.IsNotNull(result);
            Assert.AreEqual("S8014165", result.bondproductid);
        }

        [TestMethod]
        public virtual void GetMallBondByBaseProductIdCommandTest()
        {
            var result = this.bondService.HandlerCommand(
                new GetMallBondByBaseProductIdCommand
                {
                    mallId = "c8b7952a-367b-4d22-bd6b-8126208e5b46",
                    baseBondIds = new List<string> { "S8022305", "S8022480" }
                });

            Assert.IsTrue(result.Count() == 2);
        }

        [TestMethod]
        public virtual void GetMallBondByBaseProductIdSingleCommand()
        {
            var result = this.bondService.HandlerCommand(
                new GetMallBondByBaseProductIdSingleCommand
                {
                    mallid = "c8b7952a-367b-4d22-bd6b-8126208e5b46",
                    bondproductid = "S8022305",
                });

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public virtual void InsertMallBondCommandTest()
        {
            var newBondLinkId = Guid.NewGuid().ToString();
            var mallBond = new NewBondLinkDto
            {
                productid = newBondLinkId,
                bondproductid = "S6983704",
                mallid = "c8b7952a-367b-4d22-bd6b-8126208e5b46",
                limitmultiple = 0,
                fullmultiple = 0,
                predictintervalmin = 0,
                predictintervalmax = 0,
                ismaster = 0,
                sellbegintime = 201710112038168,
                sellendtime = 201712012038168,
                status = 1,
                operation = 0,
                putaway = 1,
                recommend = 0,
                isingroup = 0,
                price = 0,
                bidintervalmin = 0,
                bidintervalmax = 0
            };

            //insert DB
            var insertedResult = this.bondService.HandlerCommand(
                new InsertMallBondCommand
                {
                    mallBond = mallBond
                });
            Assert.IsNotNull(insertedResult);

            //删除数据
            this.bondService.DeleteMallBond(newBondLinkId);
        }

        [TestMethod]
        public virtual void InsertMallBondBatchCommandTest()
        {
            var newBondLinkId = Guid.NewGuid().ToString();
            var mallBond = new NewBondLinkDto
            {
                productid = newBondLinkId,
                bondproductid = "S6983704",
                mallid = "c8b7952a-367b-4d22-bd6b-8126208e5b46",
                limitmultiple = 0,
                fullmultiple = 0,
                predictintervalmin = 0,
                predictintervalmax = 0,
                ismaster = 0,
                sellbegintime = 201710112038168,
                sellendtime = 201712012038168,
                status = 1,
                operation = 0,
                putaway = 1,
                recommend = 0,
                isingroup = 0,
                price = 0,
                bidintervalmin = 0,
                bidintervalmax = 0
            };

            var insertedResult = this.bondService.HandlerCommand(
                new InsertMallBondBatchCommand
                {
                    mallBonds = new List<NewBondLinkDto> { mallBond }
                });
            Assert.IsNotNull(insertedResult);
            Assert.IsTrue(insertedResult.Count == 1);

            //删除数据
            this.bondService.DeleteMallBond(newBondLinkId);
        }

        [TestMethod]
        public virtual void UpdateMallBondCommandTest()
        {
            var newbondLinkDto = this.bondService.HandlerCommand(
                new GetMallBondByIdCommand
                {
                    id = "1e89465f-c704-4250-b259-cd75f25fc8f5"
                });

            Assert.IsNotNull(newbondLinkDto);

            //Update DB
            newbondLinkDto.isingroup = 1;
            var updateResult = this.bondService.HandlerCommand(
                new UpdateMallBondCommand
                {
                    mallBond = newbondLinkDto,
                });

            Assert.IsNotNull(updateResult);
            Assert.AreEqual(1, updateResult.isingroup);
        }

        [TestMethod]
        public virtual void GetMallBondForCloseTenderCommandTest()
        {
            var result = this.bondService.HandlerCommand(
                new GetMallBondForCloseTenderCommand
                {
                    endtime = 201710131700
                });
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public virtual void GetMallBondForCloseCommandTest()
        {
            //test data
            var mallBondList = this.bondService.HandlerCommand(
                new GetMallBondForCloseTenderCommand
                {
                    endtime = 201710121700
                });
            Assert.IsNotNull(mallBondList);
            Assert.IsTrue(mallBondList.Count > 0);

            var requestKeys = mallBondList.Select(mb => new MallBondKeyDto
            {
                mallid = mb.mallid,
                bondproductid = mb.bondproductid
            }).ToList();

            var result = this.bondService.HandlerCommand(
                new GetMallBondForCloseCommand
                {
                    mallBonds = requestKeys
                });
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public virtual void GetMallBondFullInfoByProductIdCommandTest()
        {
            var result1 = this.bondService.HandlerCommand(new GetMallBondFullInfoByProductIdCommand
            {
                productId = "S7928455",
                mallId = "0b7a1602-e51b-4733-94bb-1e6227bf6490"
            });
            Assert.IsNotNull(result1);
            Assert.IsNotNull(result1.bond);
            Assert.IsNotNull(result1.mallbond);

            var result2 = this.bondService.HandlerCommand(new GetMallBondFullInfoByProductIdCommand
            {
                productId = "S4837555",
                mallId = "0b7a1602-e51b-4733-94bb-1e6227bf6490"
            });
            Assert.IsNotNull(result2);
            Assert.IsNotNull(result2.bond);
            Assert.IsNull(result2.mallbond);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wind.iSeller.Data.Core.Commands.Point;
using Wind.iSeller.Data.Core.Dtos.Point;
using Wind.iSeller.Data.Core.Services.Point;
using Wind.iSeller.Data.Test.Common;

namespace Wind.iSeller.Data.Test.ServiceUnitTests
{
    [TestClass]
    public class PointLogServiceTest : TestBase<DataTestModule>
    {
        private readonly PointLogService pointLogService;

        public PointLogServiceTest()
        {
            this.pointLogService = Resolve<PointLogService>();
        }


        [TestMethod]
        public virtual void GetPointLogsByMallBondIdCommandTest()
        {
            var result = this.pointLogService.HandlerCommand(
                new GetPointLogsByMallBondIdCommand
                {
                    mallBondId = "4e910b37-d84c-4fab-83c2-3894e2ef2a07"
                });

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 1);

            var pointLogBean = result.FirstOrDefault();
            Assert.AreEqual("4e910b37-d84c-4fab-83c2-3894e2ef2a07", pointLogBean.ext1);
        }

        [TestMethod]
        public virtual void GetPointLogsByOrderDetailIdCommandTest()
        {
            var result = this.pointLogService.HandlerCommand(
                new GetPointLogsByOrderDetailIdCommand
                {
                    orderDetailId = "8c04097f-b595-446c-bf69-bbe51fe5bab6"
                });

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var pointLogBean = result.FirstOrDefault();
            Assert.AreEqual("8c04097f-b595-446c-bf69-bbe51fe5bab6", pointLogBean.ext2);
        }

        [TestMethod]
        public virtual void UpdatePointLogCommandTest()
        {
            var pointLogDto = this.pointLogService.HandlerCommand(
                new GetPointLogsByOrderDetailIdCommand
                {
                    orderDetailId = "8c04097f-b595-446c-bf69-bbe51fe5bab6"
                })
                .FirstOrDefault();

            Assert.IsNotNull(pointLogDto);

            //Update PointLog
            pointLogDto.iwindid = "W0816921";
            var result = this.pointLogService.HandlerCommand(
                new UpdatePointLogCommand
                {
                    pointLog = pointLogDto
                });
            Assert.IsNotNull(result);
            Assert.AreEqual("W0816921", result.iwindid);
            Assert.AreEqual("W0816921", result.wftid);
        }

        [TestMethod]
        public virtual void UpdatePointLogsBatchCommandTest()
        {
            var pointLogDtoList = this.pointLogService.HandlerCommand(
                new GetPointLogsByMallBondIdCommand
                {
                    mallBondId = "4e910b37-d84c-4fab-83c2-3894e2ef2a07"
                });

            Assert.IsNotNull(pointLogDtoList);
            Assert.IsTrue(pointLogDtoList.Count > 1);

            //Update PointLog
            foreach (var pointLogDto in pointLogDtoList)
            {
                pointLogDto.iwindid = pointLogDto.wftid;
            }

            var result = pointLogService.HandlerCommand(
                new UpdatePointLogsBatchCommand
                {
                    pointLogs = pointLogDtoList
                });

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 1);
        }

        [TestMethod]
        public virtual void InsertPointLogsBatchCommandTest()
        {
            PointLogDto newPointLog = new PointLogDto
            {
                logid = Guid.NewGuid().ToString(),
                logtype = 2,
                point = 12,
                isdel = 0,
                wftid = "W002233"
            };

            var result = pointLogService.HandlerCommand(
                new InsertPointLogsBatchCommand
                {
                    pointLogs = new List<PointLogDto> { newPointLog }
                });

            Assert.IsNotNull(result);
        }
    }
}

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wind.iSeller.Data.Core.Commands.Order;
using Wind.iSeller.Data.Core.Dtos.Order;
using Wind.iSeller.Data.Core.Services.Order;
using Wind.iSeller.Data.Test.Common;

namespace Wind.iSeller.Data.Test.ServiceUnitTests
{
    [TestClass]
    public class OrderServiceTest : TestBase<DataTestModule>
    {
        private readonly OrderService orderService;

        public OrderServiceTest()
        {
            this.orderService = Resolve<OrderService>();
        }

        [TestMethod]
        public virtual void GetOrderListByIdCommandTest()
        {
            var noneResult = this.orderService.HandlerCommand(
                new GetOrderListByIdCommand
                {
                    id = "112233"
                });
            Assert.IsNull(noneResult);

            var result = this.orderService.HandlerCommand(
                new GetOrderListByIdCommand
                {
                    id = "85f6a5e9-9527-4baa-a03d-7b193767ea15"
                });

            Assert.IsNotNull(result);
            Assert.AreEqual("8740ae06-39d2-4a9e-a02d-e227e6cc7365", result.mallid);
            Assert.AreEqual("W0817657", result.buyercrmid);
        }


        [TestMethod]
        public virtual void InsertAndDeleteOrderListCommandTest()
        {
            string newOrderId = Guid.NewGuid().ToString();

            var newOrderList = new OrderListDto
            {
                orderid = newOrderId,
                mallid = "8740ae06-39d2-4a9e-a02d-e227e6cc7365",
                buyercrmid = "W0812468",
                status = 0,
                buyercontactaccid = "c40930c6-b913-4afc-b03f-5df39241084f",
                iscommission = 0,
                buyerwmid = 0,
                orderno = "20171010132019169"
            };
            var addResult = this.orderService.HandlerCommand(
                new InsertOrderListCommand
                {
                    orderList = newOrderList
                });

            Assert.IsNotNull(addResult);
            Assert.AreEqual(0, addResult.isdel);

            var delResult = this.orderService.HandlerCommand(
                new DeleteOrderByIdCommand
                {
                    id = newOrderId
                });

            Assert.IsNotNull(delResult);
            Assert.AreEqual(1, delResult.isdel);
        }

        [TestMethod]
        public virtual void UpdateOrderListCommandTest()
        {
            var orderListDto = this.orderService.HandlerCommand(
                new GetOrderListByIdCommand
                {
                    id = "85f6a5e9-9527-4baa-a03d-7b193767ea15"
                });

            orderListDto.memo = "测试";

            var resultDto = this.orderService.HandlerCommand(
                new UpdateOrderListCommand
                {
                    orderList = orderListDto
                });

            Assert.IsNotNull(resultDto);
            Assert.AreEqual("测试", resultDto.memo);
        }

        [TestMethod]
        public virtual void InsertAndDeleteOrderDetailCommandTest()
        {
            var newdetailId = Guid.NewGuid().ToString();
            var orderDetail = new OrderDetailDto
            {
                orderdetailid = newdetailId,
                orderid = "ca6f5b70-b790-4cc0-93b6-e6c977d4fa4d",
                mallid = "ea7c84d9-299c-4ae3-8b88-eb0caf1ada8f",
                productid = "b6ec8e4c-60c7-4784-a3c1-5f9c5bac9355",
                productname = "16山东公用MTN002",
                price = 1.11M,
                amount = 1000M,
                dealamount = 1000M,
                dealprice = 0M,
                status = 0,
                recommenddealer = "测试",
                bidtype = 0,
                discount = 0M,
                netprice = 0M,
                dealamount2 = 1000M,
                deliverytype = 0M
            };

            //Insert
            var result = this.orderService.HandlerCommand(
                new InsertOrderDetailCommand
                {
                    orderDetail = orderDetail
                });
            Assert.IsNotNull(result);
            Assert.AreEqual(1.11M, result.price);

            //Delete
            var delResult = this.orderService.HandlerCommand(
                new DeleteOrderDetailByIdCommand
                {
                    orderId = null,
                    orderDetailId = newdetailId
                });

            Assert.IsNotNull(delResult);
            Assert.AreEqual(1, delResult.isdel);
        }

        [TestMethod]
        public virtual void UpdateOrderDetailCommandTest()
        {
            var orderDetail = this.orderService.HandlerCommand(
                new GetOrderDetailByOrderIdCommand
                {
                    id = "ca6f5b70-b790-4cc0-93b6-e6c977d4fa4d"
                }).FirstOrDefault();

            //Update
            orderDetail.recommenddealer = "测试员1";
            var result = this.orderService.HandlerCommand(
                new UpdateOrderDetailCommand
                {
                    orderDetail = orderDetail
                });

            Assert.IsNotNull(result);
            Assert.AreEqual("测试员1", result.recommenddealer);
        }

        [TestMethod]
        public virtual void UpdateOrderDetailsBatchCommandTest()
        {
            var orderDetail = this.orderService.HandlerCommand(
                new GetOrderDetailByOrderIdCommand
                {
                    id = "ca6f5b70-b790-4cc0-93b6-e6c977d4fa4d"
                }).FirstOrDefault();

            //Update
            var result = orderDetail.recommenddealer = "测试员1";
            this.orderService.HandlerCommand(
                new UpdateOrderDetailsBatchCommand
                {
                    orderDetails = new System.Collections.Generic.List<OrderDetailDto> { orderDetail }
                });

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public virtual void GetOrderDetailByOrderIdCommandTest()
        {
            var result = this.orderService.HandlerCommand(
                new GetOrderDetailByOrderIdCommand
                {
                    id = "ca6f5b70-b790-4cc0-93b6-e6c977d4fa4d"
                });

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public virtual void GetOrderDetailByMallBondIdCommandTest()
        {
            var result = this.orderService.HandlerCommand(
                new GetOrderDetailByMallBondIdCommand
                {
                    mallBondId = "0404880d-1297-4611-8b83-30fa78f6108d"
                });

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public virtual void GetConfirmedOrderDetailByMallBondIdCommandTest()
        {
            var result = this.orderService.HandlerCommand(
                new GetConfirmedOrderDetailByMallBondIdCommand
                {
                    mallBondId = "0404880d-1297-4611-8b83-30fa78f6108d"
                });

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public virtual void GetOrderDetailByMallProductCommandTest()
        {
            var result = this.orderService.HandlerCommand(
                new GetOrderDetailByMallProductCommand
                {
                    mallid = "0b7a1602-e51b-4733-94bb-1e6227bf6490",
                    bondproductid = "S8021109"
                });

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public virtual void GetOrderDetailByIdCommandTest()
        {
            var result = this.orderService.HandlerCommand(
                new GetOrderDetailByIdCommand
                {
                    orderId = "",
                    orderDetailId = "d2129497-6ab0-4b42-b7db-0d01af631a58"
                });

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public virtual void FilterOrderDetailForBondStatusCommandTest()
        {
            //订单行
            var orderDetails = this.orderService.HandlerCommand(
                new GetOrderDetailByOrderIdCommand
                {
                    id = "c6db495e-337f-4f5c-90de-3229928582dc"
                });

            Assert.IsNotNull(orderDetails);
            Assert.IsTrue(orderDetails.Count > 0);

            //过滤状态
            var result = this.orderService.HandlerCommand(
                new FilterOrderDetailForBondStatusCommand
                {
                    status = 4,
                    orderDetails = orderDetails
                });

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == orderDetails.Count);
        }
    }
}

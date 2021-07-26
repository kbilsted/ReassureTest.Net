using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace ReassureTest.Tests
{
    class DocumentationExamples
    {
        class Order
        {
            public Guid Id { get; set; }
            public DateTime OrderDate { get; set; }
            public DateTime LatestDeliveryDate { get; set; }
            public Decimal TotalPrice { get; set; }
            public List<OrderLine> OrderLines { get; set; }
        }

        class OrderLine
        {
            public Guid Id { get; set; }
            public int Count { get; set; }
            public Guid OrderId { get; set; }
            public string Name { get; set; }
            public string SKU { get; set; }
            public Decimal Amount { get; set; }
        }

        [Test]
        public void Order_and_oderlines_example()
        {
            var orderId = Guid.NewGuid();

            var order = new Order()
            {
                Id = orderId,
                OrderDate = DateTime.Now,
                LatestDeliveryDate = new DateTime(2020, 02, 01),
                TotalPrice = 26.98M,
                OrderLines = new List<OrderLine>()
                {
                    new OrderLine()
                    {
                        Id = Guid.NewGuid(),
                        Count = 3,
                        OrderId = orderId,
                        Name = "Rubber duck special",
                        SKU = "RD17930827",
                        Amount = 9.99M
                    },
                    new OrderLine()
                    {
                        Id = Guid.NewGuid(),
                        Count = 1,
                        OrderId = orderId,
                        Name = "Sale 10%",
                        SKU = null,
                        Amount = 2.997M
                    }
                }
            };

            order.Is(@"{
                Id = guid-0
                OrderDate = now
                LatestDeliveryDate = 2020-02-01T00:00:00
                TotalPrice = 26.98
                OrderLines = [
                    {
                        Id = guid-1
                        Count = 3
                        OrderId = guid-0
                        Name = `Rubber duck special`
                        SKU = `RD17930827`
                        Amount = 9.99
                    },
                    {
                        Id = guid-2
                        Count = 1
                        OrderId = guid-0
                        Name = `Sale 10%`
                        SKU = null
                        Amount = 2.997
                    }
                ]
            }");
        }

        [Test]
        public void Order_and_oderlines_example_traditional()
        {
            var orderId = Guid.NewGuid();

            var order = new Order()
            {
                Id = orderId,
                OrderDate = DateTime.Now,
                LatestDeliveryDate = new DateTime(2020, 02, 01),
                TotalPrice = 26.98M,
                OrderLines = new List<OrderLine>()
                {
                    new OrderLine()
                    {
                        Id = Guid.NewGuid(),
                        Count = 3,
                        OrderId = orderId,
                        Name = "Rubber duck special",
                        SKU = "RD17930827",
                        Amount = 9.99M
                    },
                    new OrderLine()
                    {
                        Id = Guid.NewGuid(),
                        Count = 1,
                        OrderId = orderId,
                        Name = "Sale 10%",
                        SKU = null,
                        Amount = 2.997M
                    }
                }
            };

            Assert.IsNotNull(order);
            Assert.AreNotEqual(order.Id, Guid.Empty);
            Assert.IsTrue((order.OrderDate-DateTime.Now).Duration() < TimeSpan.FromSeconds(2));
            Assert.AreEqual(order.LatestDeliveryDate, new DateTime(2020, 02, 01));
            Assert.AreEqual(order.TotalPrice, 26.98M);

            Assert.AreEqual(order.OrderLines.Count, 2);
            Assert.AreNotEqual(order.OrderLines[0].Id, order.Id);
            Assert.AreEqual(order.OrderLines[0].Count, 3);
            Assert.AreEqual(order.OrderLines[0].OrderId, order.Id);
            Assert.AreEqual(order.OrderLines[0].Name, "Rubber duck special");
            Assert.AreEqual(order.OrderLines[0].SKU, "RD17930827");
            Assert.AreEqual(order.OrderLines[0].Amount, 9.99);

            Assert.AreNotEqual(order.OrderLines[1].Id, order.Id);
            Assert.AreEqual(order.OrderLines[1].Count, 1);
            Assert.AreEqual(order.OrderLines[1].OrderId, order.Id);
            Assert.AreEqual(order.OrderLines[1].Name, "Sale 10%");
            Assert.AreEqual(order.OrderLines[1].SKU, null);
            Assert.AreEqual(order.OrderLines[1].Amount, 2.997);
        }
    }
}
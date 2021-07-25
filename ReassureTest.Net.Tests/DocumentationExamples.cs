using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace ReassureTest.Tests
{
    class DocumentationExamples
    {
        class Order
        {
            public Guid Id{ get; set; }
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
    }
}
using System;
using NUnit.Framework;

namespace ReassureTest.Tests
{
    public class FieldTranslatorTests
    {
        [Test]
        public void Domain_types_can_be_simplified_unmodified()
        {
            CreateOrder().Is(@"{
                OrderDate = {
                    Value = now
                }
                LatestDeliveryDate = {
                    Value = 2021-03-04T00:00:00
                }
                Note = `Leave at front door`
            }");
        }

        [Test]
        public void Domain_types_can_be_simplified_config()
        {
            var cfg = Reassure.DefaultConfiguration.DeepClone();
            cfg.Harvesting.FieldValueTranslators.Add(o =>
                o switch
                {
                    OrderDate od => od.Value,
                    LatestDeliveryDate ldd => ldd?.Value,
                    _ => o
                });

            CreateOrder().With(cfg).Is(@"{
                OrderDate = now
                LatestDeliveryDate = 2021-03-04T00:00:00
                Note = `Leave at front door`
            }");
        }

        [Test]
        public void Domain_types_can_be_simplified_config_2()
        {
            var cfg = Reassure.DefaultConfiguration.DeepClone();
            cfg.Harvesting.FieldValueTranslators.Add(o => o is OrderDate d ? d.Value : o);
            cfg.Harvesting.FieldValueTranslators.Add(o => o is LatestDeliveryDate d ? d.Value : o);

            CreateOrder().With(cfg).Is(@"{
                OrderDate = now
                LatestDeliveryDate = 2021-03-04T00:00:00
                Note = `Leave at front door`
            }");
        }

        private static Order CreateOrder()
        {
            return new Order
            {
                OrderDate = new OrderDate { Value = DateTime.Now },
                LatestDeliveryDate = new LatestDeliveryDate { Value = new DateTime(2021, 3, 4) },
                Note = "Leave at front door"
            };
        }

        class OrderDate
        {
            public DateTime Value { get; set; }
        }

        class LatestDeliveryDate
        {
            public DateTime Value { get; set; }
        }

        class Order
        {
            public OrderDate OrderDate { get; set; }
            public LatestDeliveryDate LatestDeliveryDate { get; set; }
            public string Note { get; set; }
        }
    }
}
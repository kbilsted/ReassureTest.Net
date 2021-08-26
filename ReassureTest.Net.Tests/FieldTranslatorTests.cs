using System;
using NUnit.Framework;

namespace ReassureTest.Tests
{
    public class DataProjectionTests
    {
        [Test]
        public void When_comparing_agains_untraversable_types_Then_just_ignore_thos_types()
        {
            new Unharvesable() { F = () => "xxx" }.Is("");
        }

        class Unharvesable
        {
            public Func<string> F { get; set; }
        }

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
            cfg.Harvesting.Add((parent, field, pi) =>
                field switch
                {
                    OrderDate od => Flow.Use(od.Value),
                    LatestDeliveryDate ldd => Flow.Use(ldd?.Value),
                    _ => Flow.Use(field)
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

            cfg.Harvesting
                .Add((parent, value, pi) => Flow.Use(value is OrderDate d ? d?.Value : value))
                .Add((parent, value, pi) => Flow.Use(value is LatestDeliveryDate d ? d?.Value : value));

            CreateOrder().With(cfg).Is(@"{
                OrderDate = now
                LatestDeliveryDate = 2021-03-04T00:00:00
                Note = `Leave at front door`
            }");
        }

        [Test]
        public void Domain_types_can_be_simplified_config_3()
        {
            CreateOrder()
                .With(cfg => cfg.Harvesting
                    .Add((parent, value, pi) => Flow.Use(value is OrderDate d ? d?.Value : value))
                    .Add((parent, value, pi) => Flow.Use(value is LatestDeliveryDate d ? d?.Value : value)))
                .Is(@"{
                OrderDate = now
                LatestDeliveryDate = 2021-03-04T00:00:00
                Note = `Leave at front door`
            }");

            CreateOrder()
                .With(cfg => cfg.Harvesting.Add((parent, value, pi) => Flow.Use(value is OrderDate d ? d?.Value : value)))
                .With(cfg => cfg.Harvesting.Add((parent, value, pi) => Flow.Use(value is LatestDeliveryDate d ? d?.Value : value)))
                .Is(@"{
                OrderDate = now
                LatestDeliveryDate = 2021-03-04T00:00:00
                Note = `Leave at front door`
            }");
        }

        [Test]
        public void Domain_types_can_be_set_to_null()
        {
            var cfg = Reassure.DefaultConfiguration.DeepClone();

            cfg.Harvesting
                .Add((parent, value, info) => Flow.Use(value is OrderDate d ? null : value))
                .Add((parent, value, info) => Flow.Use(value is LatestDeliveryDate d ? null : value));

            CreateOrder().With(cfg).Is(@"{
                OrderDate = null
                LatestDeliveryDate = null
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
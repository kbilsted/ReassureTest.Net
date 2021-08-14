using NUnit.Framework;

namespace ReassureTest.Tests
{
    public class NestedTests
    {
        [Test]
        public void Nested()
        {
            var o = CreateNestedTop();
            o.Is(@"{
                A = {
                    I = 33
                }
                B = {
                    B = true
                }
                C = {
                    S = `some string`
                    D = {
                        G = *
                    }
                }
                S2 = `s2s2s2`
            }");
        }

        [Test]
        public void When_expecting_more_fields_than_actual_in_nested_object_Then_report_missing_fields()
        {
            Reassure
                .Catch(() => new NestedChildC { D = new NestedChildChildD { G = 3 } }.Is("{ D = { G = 3 h=4 KK=5 } S=? }"))
                .Message.Is(@"`Path: 'D'.
Expected fields 'h', 'KK'
But fields were not found!`");
        }

        [Test]
        public void When_filtering_only_on_string_fields_Then_only_get_s2()
        {
            var cfg = Reassure.DefaultConfiguration.DeepClone();
            cfg.Harvesting
                .Add((parent, field, pi) => pi.PropertyType == typeof(string) ? Flow.Use(field) : Flow.Skip);

            CreateNestedTop().With(cfg).Is("{ S2 = `s2s2s2` }");
        }

        [Test]
        public void When_filtering_only_on_string_fields_using_without_Then_only_get_s2()
        {
            CreateNestedTop()
                .Without(pi => pi.PropertyType != typeof(string))
                .Is("{ S2 = `s2s2s2` }");
        }

        [Test]
        public void When_filtering_only_fields_starting_with_s_Then_only_get_s2()
        {
            var cfg = Reassure.DefaultConfiguration.DeepClone();
            cfg.Harvesting
                .Add((parent, field, pi) => pi.Name.StartsWith("S") ? Flow.Use(field) : Flow.Skip);

            CreateNestedTop().With(cfg).Is("{ S2 = `s2s2s2` }");
        }

        [Test]
        public void When_filtering_only_fields_starting_with_s_using_without_Then_only_get_s2()
        {
            CreateNestedTop()
                .Without(pi => !pi.Name.StartsWith("S"))
                .Is("{ S2 = `s2s2s2` }");
        }

        [Test]
        public void When_filtering_on_values_of_string_fields_Then_only_get_s2()
        {
            var cfg = Reassure.DefaultConfiguration.DeepClone();
            cfg.Harvesting
                .Add((parent, field, pi) => pi.PropertyType == typeof(string) && field.Equals("hello") ? Flow.Use(field) : Flow.Skip);

            new ThreeStrings() { S1 = "world", S2 = "hello", S3 = "foobar" }.With(cfg).Is("{ S2 = `hello` }");
        }

        [Test]
        public void When_filtering_on_values_of_string_fields_using_without_Then_only_get_s2()
        {
            new ThreeStrings() { S1 = "world", S2 = "hello", S3 = "foobar" }
                .With((parent, field, pi) => pi.PropertyType == typeof(string) && field.Equals("hello") ? Flow.Use(field) : Flow.Skip)
                .Is("{ S2 = `hello` }");
        }

        [Test]
        public void When_multiple_filtering_on_named_fields_using_without_Then_only_get_s2()
        {
            new ThreeStrings() { S1 = "world", S2 = "hello", S3 = "foobar" }
                .Without(pi => pi.Name == "S1")
                .Without(pi => pi.Name == "S3")
                .Is("{ S2 = `hello` }");
        }

        [Test]
        public void When_filtering_only_complex_types_Then_only_get_s2()
        {
            var cfg = Reassure.DefaultConfiguration.DeepClone();
            cfg.Harvesting.Add((parent, value, info) => !info.PropertyType.IsPrimitive ? Flow.Use(value) : Flow.Skip);

            CreateNestedTop().With(cfg).Is(@"{
                C = {
                    S = `some string`
                }
                S2 = `s2s2s2`
            }");
        }

        [Test]
        public void When_filtering_only_complex_types_using_without_Then_only_get_s2()
        {
            CreateNestedTop()
                .Without(pi => pi.PropertyType.IsPrimitive)
                .Is(@"{
                C = {
                    S = `some string`
                }
                S2 = `s2s2s2`
            }");
        }

        [Test]
        public void When_filtering_all_types_Then_get_empty()
        {
            var cfg = Reassure.DefaultConfiguration.DeepClone();
            cfg.Harvesting.Add((parent, value, info) => Flow.Skip);

            CreateNestedTop().With(cfg).Is(@"");
        }

        [Test]
        public void When_filtering_all_types_using_without_Then_get_empty()
        {
            var cfg = Reassure.DefaultConfiguration.DeepClone();
            cfg.Harvesting.Add((parent, value, info) => Flow.Skip);

            CreateNestedTop().Without(p => true).Is(@"");
        }

        [Test]
        public void When_filtering_all_types_and_asserting_nonempty_Then_get_error()
        {
            var cfg = Reassure.DefaultConfiguration.DeepClone();
            cfg.Harvesting.Add((parent, o, pi) => Flow.Skip);

            var ex = Assert.Throws<AssertionException>(() => CreateNestedTop().With(cfg).Is(@"{}"));

            Assert.AreEqual("Expected: {}\r\nBut was:  <empty>    (all fields have been filtered away)", ex.Message);
        }


        private static NestedTop CreateNestedTop()
        {
            return new NestedTop()
            {
                A = new NestedChildA()
                {
                    I = 33
                },
                B = new NestedChildB()
                {
                    B = true
                },
                C = new NestedChildC()
                {
                    D = new NestedChildChildD() { G = 5 },
                    S = "some string"
                },
                S2 = "s2s2s2"
            };
        }

        [Test]
        public void Nested_null()
        {
            var o = new NestedTop();
            var ex = Assert.Throws<AssertionException>(() => o.Is(@"{ A = { I = 33 }}"));
            Assert.AreEqual("Path: 'A'.\r\nExpected: not null\r\nBut was: null", ex.Message);
        }

        [Test]
        public void Nested_null_is_matched_with_nullmatch()
        {
            var o = new NestedTop();
            o.Is(@"{ 
                A = ? 
                B = ? 
                C = ? 
                S2 = ? }");
        }

        [Test]
        public void Nestedarray_wildcard_match()
        {
            var o = CreateNestedTopArray();

            o.Is(@"{ C = * }");
        }

        [Test]
        public void Nestedarray_array_elems_wildcard_match()
        {
            var o = CreateNestedTopArray();

            o.Is(@"{ C = [ *, * ] }");
        }

        [Test]
        public void Nestedarray_partialwildcard_match()
        {
            var o = CreateNestedTopArray();

            o.Is(@"{
    C = [ *, {
            S = `hello world`
            D = { G = 4 }
        } ] }");
        }

        [Test]
        public void Nestedarray_full_match()
        {
            var o = CreateNestedTopArray();

            o.Is(@"{
    C = [
        {
            S = `some string`
            D = {
                G = 3
            }
        },
        {
            S = `hello world`
            D = {
                G = 4
            }
        }
    ]
}");
        }

        [Test]
        public void Nestedarray_failed_match()
        {
            var o = CreateNestedTopArray();

            var ex = Assert.Throws<AssertionException>(() => o.Is(@"{ C = [ * ] }"));
            Assert.AreEqual("Path: 'C[2]'.\r\nArray length mismatch. Expected array lengh: 1 but was: 2.", ex.Message);
        }

        private static NestedTopArray CreateNestedTopArray()
        {
            return new NestedTopArray()
            {
                C = new[]
                {
                    new NestedChildC()
                    {
                        D = new NestedChildChildD() { G = 3 },
                        S = "some string"
                    },new NestedChildC()
                    {
                        D = new NestedChildChildD() { G = 4 },
                        S = "hello world"
                    }
                }
            };
        }

        class ThreeStrings
        {
            public string S1 { get; set; }
            public string S2 { get; set; }
            public string S3 { get; set; }
        }

        class NestedTop
        {
            public NestedChildA A { get; set; }
            public NestedChildB B { get; set; }
            public NestedChildC C { get; set; }
            public string S2 { get; set; }
        }

        class NestedTopArray
        {
            public NestedChildC[] C { get; set; }
        }

        class NestedChildA
        {
            public int I { get; set; }
        }

        class NestedChildB
        {
            public bool B { get; set; }
        }

        class NestedChildC
        {
            public string S { get; set; }
            public NestedChildChildD D { get; set; }
        }

        class NestedChildChildD
        {
            public int G { get; set; }
        }
    }
}
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace ReassureTest.Tests
{
    public class FileTests
    {
        [Test]
        public void File_is_not_found()
        {
            FileInfo fileInfo = new FileInfo("Files\\unknownfile.txt");

            "".Is(fileInfo);
        }

        [Test]
        public void File_contains_json_formatted_content()
        {
            FileInfo fileInfo = new FileInfo("Files\\jsonformattedentities.txt");

            List<TestEntity> actualValue = new List<TestEntity>()
            {
                new TestEntity()
                {
                    Field2 = 45,
                    Field3 = new List<string>(){ "field3_1","field3_2"},
                    Field1 = "field1_1"
                },
                new TestEntity()
                {
                    Field2 = 20,
                    Field3 = new List<string>(){ "field3_3", "field3_4"},
                    Field1 = "field1_2"
                }

            };

            actualValue.Is(fileInfo);
        }

        private class TestEntity
        {
            public string Field1 { get; set; }
            public int Field2 { get; set; }
            public List<string> Field3 { get; set; } = new List<string>();
        }
    }
}

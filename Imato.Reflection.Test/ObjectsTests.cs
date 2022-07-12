using NUnit.Framework;
using System;

namespace Imato.Reflection.Test
{
    public class ObjectsTests
    {
        private TestClass testObj = new TestClass
        {
            Id = 1,
            Name = "Test 1",
            Date = DateTime.Parse("2022-08-03"),
            Flag = false,
            Test = new Nested
            {
                SubId = 2,
                Name = "Test 2"
            }
        };

        [Test]
        public void GetFieldsTest()
        {
            var obj = new TestClass
            {
                Id = 1,
                Name = "Test 1",
                Date = DateTime.Parse("2022-08-03"),
                Flag = false
            };

            var result = obj.GetFields();
            Assert.AreEqual(5, result.Count);
            Assert.True(result.ContainsKey("Name"));
        }

        [Test]
        public void GetNestedFieldsTest()
        {
            var result = testObj.GetFields();
            Assert.AreEqual(6, result.Count);
            Assert.True(result.ContainsKey("Test.Name"));
            Assert.True(result.ContainsKey("Test.SubId"));
        }

        [Test]
        public void SkipFieldsTest()
        {
            var result = testObj.GetFields(new string[] { "Name", "Test.Name" });
            Assert.AreEqual(4, result.Count);
            Assert.False(result.ContainsKey("Test.Name"));
            Assert.False(result.ContainsKey("Name"));
        }

        [Test]
        public void SkipChildrenTest()
        {
            var result = testObj.GetFields(skipChildren: true);
            Assert.AreEqual(4, result.Count);
            Assert.False(result.ContainsKey("Test.Name"));
            Assert.False(result.ContainsKey("Test.SubId"));
        }

        [Test]
        public void GetDiffTest()
        {
            var obj1 = new TestClass
            {
                Id = 1,
                Name = "Test 1",
                Date = DateTime.Parse("2022-08-03"),
                Flag = false
            };
            var obj2 = new TestClass
            {
                Id = 1,
                Name = "Test 2",
                Date = DateTime.Parse("2022-08-03"),
                Flag = false
            };
            var diff = Objects.GetDiff(obj1, obj2);
            Assert.AreEqual(1, diff.Count);
            Assert.AreEqual("Test 1", diff["Name"]);

            diff = Objects.GetDiff(obj2, obj1);
            Assert.AreEqual(1, diff.Count);
            Assert.AreEqual("Test 2", diff["Name"]);
        }

        [Test]
        public void ToStringTest()
        {
            var obj1 = new TestClass
            {
                Id = 1,
                Name = "Test 1",
                Date = DateTime.Parse("2022-08-03"),
                Flag = false
            };
            var dic = obj1.GetFields();
            var result = dic.ToCsvString();
            Assert.AreEqual(@"""Date"": ""2022-08-03T00:00:00.000""; ""Flag"": ""False""; ""Id"": ""1""; ""Name"": ""Test 1""; ""Test"": ""null""",
                result);
        }

        [Test]
        public void ListTest()
        {
            var obj = new ListTest
            {
                Id = 100,
                Name = "Test"
            };
            obj.List.Add(new Nested { Name = "Nested 1", SubId = 1 });
            obj.List.Add(new Nested { Name = "Nested 2", SubId = 2 });
            var resutl = obj.GetFields();
            Assert.AreEqual(6, resutl.Count);
            Assert.AreEqual(1, resutl["List[1].SubId"]);
            Assert.AreEqual("Nested 2", resutl["List[2].Name"]);
        }
    }
}
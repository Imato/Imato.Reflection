using NUnit.Framework;
using System;

namespace Imato.Reflection.Test
{
    public class ObjectsTests
    {
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
            var obj = new TestClass
            {
                Id = 1,
                Name = "Test 1",
                Date = DateTime.Parse("2022-08-03"),
                Flag = false,
                Test = new TestClass
                {
                    Id = 2,
                    Name = "Test 2"
                }
            };

            var result = obj.GetFields();
            Assert.AreEqual(5, result.Count);
            Assert.True(result.ContainsKey("Name"));
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
    }
}
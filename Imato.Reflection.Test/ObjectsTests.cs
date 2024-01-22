using NUnit.Framework;
using System;
using System.Linq;

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
            Assert.That(result.Count, Is.EqualTo(5));
            Assert.That(result.ContainsKey("Name"), Is.True);
            Assert.That(result["Id"], Is.EqualTo(obj.Id));

            result = obj.GetFields(skipFields: "flag,name".Split(","));
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result.ContainsKey("Date"), Is.True);
            Assert.That(result.ContainsKey("Fame"), Is.False);

            result = obj.GetFields(fields: "Name,date".Split(","));
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.ContainsKey("Name"), Is.True);
            Assert.That(result["Name"], Is.EqualTo(obj.Name));
            Assert.That(result.ContainsKey("Date"), Is.True);
        }

        [Test]
        public void GetDynamicFieldsTest()
        {
            var obj = new
            {
                id = 1,
                enabled = true
            };

            var result = Objects.GetDynamicFields(obj);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.ContainsKey("enabled"), Is.True);
            Assert.That(result["id"], Is.EqualTo(obj.id));

            result = Objects.GetFields(obj);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.ContainsKey("enabled"), Is.True);
            Assert.That(result["id"], Is.EqualTo(obj.id));
        }

        [Test]
        public void GetFieldNamesTest()
        {
            var result = Objects.GetFieldNames<TestClass>();
            Assert.That(result.Count(), Is.EqualTo(6));
            Assert.That(result.Contains("Name"), Is.True);
            Assert.That(result.Contains("Test.Name"), Is.True);
        }

        [Test]
        public void GetNestedFieldsTest()
        {
            var result = testObj.GetFields();
            Assert.That(result.Count, Is.EqualTo(6));
            Assert.That(result.ContainsKey("Test.Name"), Is.True);
            Assert.That(result.ContainsKey("Test.SubId"), Is.True);
        }

        [Test]
        public void SkipFieldsTest()
        {
            var result = testObj.GetFields(new string[] { "Name", "Test.Name" });
            Assert.That(result.Count, Is.EqualTo(4));
            Assert.That(result.ContainsKey("Test.Name"), Is.False);
            Assert.That(result.ContainsKey("Name"), Is.False);
        }

        [Test]
        public void SkipChildrenTest()
        {
            var result = testObj.GetFields(skipChildren: true);
            Assert.That(result.Count, Is.EqualTo(4));
            Assert.That(result.ContainsKey("Test.Name"), Is.False);
            Assert.That(result.ContainsKey("Test.SubId"), Is.False);
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
            var diff = obj1.GetDiff(obj2);
            Assert.That(diff.Count, Is.EqualTo(1));
            Assert.That(diff["Name"], Is.EqualTo("Test 1"));

            diff = obj2.GetDiff(obj1);
            Assert.That(diff.Count, Is.EqualTo(1));
            Assert.That(diff["Name"], Is.EqualTo("Test 2"));

            Assert.That(obj1.IsEqual(obj2), Is.False);
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
            var result = dic.ToCsv();
            Assert.That(result,
                Is.EqualTo(@"""Date"":""2022-08-03T00:00:00.000"";""Flag"":False;""Id"":1;""Name"":""Test 1"";""Test"":null"));
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
            Assert.That(resutl.Count, Is.EqualTo(6));
            Assert.That(resutl["List[1].SubId"], Is.EqualTo(1));
            Assert.That(resutl["List[2].Name"], Is.EqualTo("Nested 2"));
        }

        [Test]
        public void CsvTest()
        {
            var dic = testObj.GetFields();
            var result = dic.ToCsv();
        }
    }
}
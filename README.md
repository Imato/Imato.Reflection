### Imato.Reflection

- read public object data as dictionary ([property] = value)
- test object equality
- compare object and get differences as dictionary ([property] = value)
- desalinize object to JSON or CSV string

#### Examples

```csharp
// Same object 

using Imato.Reflection;

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
```

```csharp
// Read object data as dictionary

var result = testObj.GetFields();
Assert.AreEqual(6, result.Count);
Assert.True(result.ContainsKey("Name"));
Assert.True(result.ContainsKey("Test.Name"));
Assert.True(result.ContainsKey("Test.SubId"));
```

```csharp
// Get differences

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
Assert.AreEqual(1, diff.Count);
Assert.AreEqual("Test 1", diff["Name"]);

// Test equality
Assert.IsFalse(obj1.IsEqual(obj2));
```

```csharp
// Serialization

var json = testObj.ToJson();
var csv = testObj.ToCsv()
```


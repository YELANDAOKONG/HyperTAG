using HyperTAG.Attributes;
using HyperTAG.Converters;
using HyperTAG.Core;
using HyperTAG.Exceptions;
using HyperTAG.Models;

namespace TagTests.Converters;

public class ListConverterTests
{
    [TagStruct]
    public class TestPerson
    {
        [TagData]
        public string Name = string.Empty;
        
        [TagData]
        public int Age;

        public override bool Equals(object? obj)
        {
            if (obj is not TestPerson other) return false;
            return Name == other.Name && Age == other.Age;
        }

        public override int GetHashCode() => HashCode.Combine(Name, Age);
    }

    [Fact]
    public void ToTag_WithIntList_ReturnsValidTag()
    {
        // Arrange
        var converter = new ListConverter<int>();
        var list = new List<int> { 1, 2, 3, 4, 5 };

        // Act
        var tag = converter.ToTag(list);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Empty, tag.Type);
        Assert.Equal(5, tag.Entities.Count);
        
        for (int i = 0; i < list.Count; i++)
        {
            Assert.Equal(TagDataType.Int, tag.Entities[i].Type);
            Assert.Equal(list[i], tag.Entities[i].Value);
        }
    }

    [Fact]
    public void ToTag_WithStringList_ReturnsValidTag()
    {
        // Arrange
        var converter = new ListConverter<string>();
        var list = new List<string> { "apple", "banana", "cherry" };

        // Act
        var tag = converter.ToTag(list);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Empty, tag.Type);
        Assert.Equal(3, tag.Entities.Count);
        
        Assert.Equal(TagDataType.String, tag.Entities[0].Type);
        Assert.Equal("apple", tag.Entities[0].Value);
        Assert.Equal(TagDataType.String, tag.Entities[1].Type);
        Assert.Equal("banana", tag.Entities[1].Value);
        Assert.Equal(TagDataType.String, tag.Entities[2].Type);
        Assert.Equal("cherry", tag.Entities[2].Value);
    }

    [Fact]
    public void ToTag_WithEmptyList_ReturnsEmptyTag()
    {
        // Arrange
        var converter = new ListConverter<int>();
        var list = new List<int>();

        // Act
        var tag = converter.ToTag(list);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Empty, tag.Type);
        Assert.Empty(tag.Entities);
    }

    [Fact]
    public void ToTag_WithNullElements_HandlesNullsCorrectly()
    {
        // Arrange
        var converter = new ListConverter<string>();
        var list = new List<string?> { "first", null, "third" };

        // Act
        var tag = converter.ToTag(list!);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(3, tag.Entities.Count);
        Assert.Equal(TagDataType.String, tag.Entities[0].Type);
        Assert.Equal("first", tag.Entities[0].Value);
        Assert.Equal(TagDataType.Null, tag.Entities[1].Type);
        Assert.Equal(TagDataType.String, tag.Entities[2].Type);
        Assert.Equal("third", tag.Entities[2].Value);
    }

    [Fact]
    public void ToTag_WithComplexObjects_ReturnsValidTag()
    {
        // Arrange
        var converter = new ListConverter<TestPerson>();
        var list = new List<TestPerson>
        {
            new TestPerson { Name = "Alice", Age = 30 },
            new TestPerson { Name = "Bob", Age = 25 }
        };

        // Act
        var tag = converter.ToTag(list);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Empty, tag.Type);
        Assert.Equal(2, tag.Entities.Count);
        
        // Each person should be serialized as a nested structure
        foreach (var entityTag in tag.Entities)
        {
            Assert.Equal(TagDataType.Empty, entityTag.Type);
        }
    }

    [Fact]
    public void ToTag_WithNullInput_ReturnsNullTag()
    {
        // Arrange
        var converter = new ListConverter<int>();

        // Act
        var tag = converter.ToTag(null);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Null, tag.Type);
    }

    [Fact]
    public void FromTag_WithIntList_ReturnsValidList()
    {
        // Arrange
        var converter = new ListConverter<int>();
        var original = new List<int> { 10, 20, 30 };
        var tag = converter.ToTag(original);

        // Act
        var result = converter.FromTag(tag);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal(10, result[0]);
        Assert.Equal(20, result[1]);
        Assert.Equal(30, result[2]);
    }

    [Fact]
    public void FromTag_WithStringList_ReturnsValidList()
    {
        // Arrange
        var converter = new ListConverter<string>();
        var original = new List<string> { "red", "green", "blue" };
        var tag = converter.ToTag(original);

        // Act
        var result = converter.FromTag(tag);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("red", result[0]);
        Assert.Equal("green", result[1]);
        Assert.Equal("blue", result[2]);
    }

    [Fact]
    public void FromTag_WithEmptyTag_ReturnsEmptyList()
    {
        // Arrange
        var converter = new ListConverter<int>();
        var tag = new HyperTag(TagDataType.Empty);

        // Act
        var result = converter.FromTag(tag);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void FromTag_WithNullTag_ReturnsNull()
    {
        // Arrange
        var converter = new ListConverter<int>();

        // Act
        var result = converter.FromTag(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void FromTag_WithNullTypeTag_ReturnsNull()
    {
        // Arrange
        var converter = new ListConverter<int>();
        var tag = new HyperTag(TagDataType.Null);

        // Act
        var result = converter.FromTag(tag);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void FromTag_WithInvalidType_ThrowsException()
    {
        // Arrange
        var converter = new ListConverter<int>();
        var invalidTag = new HyperTag(TagDataType.String, "invalid");

        // Act & Assert
        Assert.Throws<TagAutoDeserializeException>(() => converter.FromTag(invalidTag));
    }

    [Fact]
    public void FromTag_WithComplexObjects_ReturnsValidList()
    {
        // Arrange
        var converter = new ListConverter<TestPerson>();
        var original = new List<TestPerson>
        {
            new TestPerson { Name = "Charlie", Age = 35 },
            new TestPerson { Name = "Diana", Age = 28 }
        };
        var tag = converter.ToTag(original);

        // Act
        var result = converter.FromTag(tag);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Charlie", result[0].Name);
        Assert.Equal(35, result[0].Age);
        Assert.Equal("Diana", result[1].Name);
        Assert.Equal(28, result[1].Age);
    }

    [Fact]
    public void RoundTrip_WithIntegers_PreservesData()
    {
        // Arrange
        var converter = new ListConverter<int>();
        var original = new List<int> { 100, 200, 300, 400 };

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(original.Count, restored.Count);
        for (int i = 0; i < original.Count; i++)
        {
            Assert.Equal(original[i], restored[i]);
        }
    }

    [Fact]
    public void RoundTrip_WithStrings_PreservesData()
    {
        // Arrange
        var converter = new ListConverter<string>();
        var original = new List<string> { "hello", "world", "test" };

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(original, restored);
    }

    [Fact]
    public void RoundTrip_WithNullElements_PreservesNulls()
    {
        // Arrange
        var converter = new ListConverter<string>();
        var original = new List<string?> { "start", null, "middle", null, "end" };

        // Act
        var tag = converter.ToTag(original!);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(5, restored.Count);
        Assert.Equal("start", restored[0]);
        Assert.Null(restored[1]);
        Assert.Equal("middle", restored[2]);
        Assert.Null(restored[3]);
        Assert.Equal("end", restored[4]);
    }

    [Fact]
    public void RoundTrip_WithComplexObjects_PreservesData()
    {
        // Arrange
        var converter = new ListConverter<TestPerson>();
        var original = new List<TestPerson>
        {
            new TestPerson { Name = "Eve", Age = 40 },
            new TestPerson { Name = "Frank", Age = 33 }
        };

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(original.Count, restored.Count);
        for (int i = 0; i < original.Count; i++)
        {
            Assert.Equal(original[i], restored[i]);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(100)]
    public void RoundTrip_WithVariousSizes_PreservesCount(int count)
    {
        // Arrange
        var converter = new ListConverter<int>();
        var original = new List<int>();
        for (int i = 0; i < count; i++)
        {
            original.Add(i * 10);
        }

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(count, restored.Count);
        Assert.Equal(original, restored);
    }

    [Fact]
    public void RoundTrip_WithMixedBasicTypes_PreservesData()
    {
        // Arrange
        var converter = new ListConverter<double>();
        var original = new List<double> { 1.5, 2.7, 3.14159, -5.5 };

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(original, restored);
    }

    [Fact]
    public void ToTag_WithBoolList_ReturnsValidTag()
    {
        // Arrange
        var converter = new ListConverter<bool>();
        var list = new List<bool> { true, false, true, true };

        // Act
        var tag = converter.ToTag(list);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(4, tag.Entities.Count);
        Assert.Equal(TagDataType.Bool, tag.Entities[0].Type);
        Assert.True((bool)tag.Entities[0].Value!);
    }

    [Fact]
    public void FromTag_WithTypeMismatch_ThrowsException()
    {
        // Arrange
        var converter = new ListConverter<int>();
        var tag = new HyperTag(TagDataType.Empty);
        tag.Entities.Add(new HyperTag(TagDataType.String, "not an int"));

        // Act & Assert
        Assert.Throws<TagAutoDeserializeException>(() => converter.FromTag(tag));
    }
}

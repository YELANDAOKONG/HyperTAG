using HyperTAG.Converters;
using HyperTAG.Core;
using HyperTAG.Models;

namespace TagTests.Converters;

public class HyperTagConverterTests
{
    private readonly HyperTagConverter _converter = new();

    [Fact]
    public void ToTag_WithHyperTag_ReturnsDataTag()
    {
        // Arrange
        var hyperTag = new HyperTag(TagDataType.String, "test");

        // Act
        var tag = _converter.ToTag(hyperTag);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Data, tag.Type);
        Assert.NotNull(tag.Value);
        Assert.IsType<byte[]>(tag.Value);
    }

    [Fact]
    public void ToTag_WithNullInput_ReturnsNull()
    {
        // Act
        var tag = _converter.ToTag(null);

        // Assert
        Assert.Null(tag);
    }

    [Fact]
    public void ToTag_WithComplexHyperTag_ReturnsValidTag()
    {
        // Arrange
        var hyperTag = new HyperTag(TagDataType.Empty);
        hyperTag.Entities.Add(new HyperTag(TagDataType.String, "field1"));
        hyperTag.Entities.Add(new HyperTag(TagDataType.Int, 42));

        // Act
        var tag = _converter.ToTag(hyperTag);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Data, tag.Type);
    }

    [Fact]
    public void FromTag_WithValidTag_ReturnsHyperTag()
    {
        // Arrange
        var original = new HyperTag(TagDataType.Long, 123L);
        var serialized = original.Serialize();
        var tag = new HyperTag(TagDataType.Data, serialized);

        // Act
        var result = _converter.FromTag(tag);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TagDataType.Long, result.Type);
        Assert.Equal(123L, result.Value);
    }

    [Fact]
    public void FromTag_WithNullTag_ReturnsNull()
    {
        // Act
        var result = _converter.FromTag(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void FromTag_WithNullTypeTag_ReturnsNull()
    {
        // Arrange
        var tag = new HyperTag(TagDataType.Null);

        // Act
        var result = _converter.FromTag(tag);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void FromTag_WithWrongType_ReturnsNull()
    {
        // Arrange
        var tag = new HyperTag(TagDataType.String, "invalid");

        // Act
        var result = _converter.FromTag(tag);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void RoundTrip_WithSimpleHyperTag_PreservesData()
    {
        // Arrange
        var original = new HyperTag(TagDataType.String, "hello world");

        // Act
        var tag = _converter.ToTag(original);
        var restored = _converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(original.Type, restored.Type);
        Assert.Equal(original.Value, restored.Value);
    }

    [Fact]
    public void RoundTrip_WithNestedHyperTag_PreservesData()
    {
        // Arrange
        var original = new HyperTag(TagDataType.Empty);
        var child1 = new HyperTag(TagDataType.String, "name");
        child1.Entities.Add(new HyperTag(TagDataType.String, "John"));
        original.Entities.Add(child1);

        var child2 = new HyperTag(TagDataType.String, "age");
        child2.Entities.Add(new HyperTag(TagDataType.Int, 30));
        original.Entities.Add(child2);

        // Act
        var tag = _converter.ToTag(original);
        var restored = _converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(original.Type, restored.Type);
        Assert.Equal(2, restored.Entities.Count);
        Assert.Equal("name", restored.Entities[0].Value);
        Assert.Equal("John", restored.Entities[0].Entities[0].Value);
        Assert.Equal("age", restored.Entities[1].Value);
        Assert.Equal(30, restored.Entities[1].Entities[0].Value);
    }

    [Fact]
    public void RoundTrip_WithEmptyHyperTag_PreservesData()
    {
        // Arrange
        var original = new HyperTag(TagDataType.Empty);

        // Act
        var tag = _converter.ToTag(original);
        var restored = _converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(TagDataType.Empty, restored.Type);
        Assert.Empty(restored.Entities);
    }

    [Fact]
    public void RoundTrip_WithAllBasicTypes_PreservesData()
    {
        // Arrange
        var original = new HyperTag(TagDataType.Empty);
        original.Entities.Add(new HyperTag(TagDataType.Bool, true));
        original.Entities.Add(new HyperTag(TagDataType.Int, 42));
        original.Entities.Add(new HyperTag(TagDataType.Long, 123L));
        original.Entities.Add(new HyperTag(TagDataType.Double, 3.14));
        original.Entities.Add(new HyperTag(TagDataType.String, "test"));

        // Act
        var tag = _converter.ToTag(original);
        var restored = _converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(5, restored.Entities.Count);
        Assert.Equal(true, restored.Entities[0].Value);
        Assert.Equal(42, restored.Entities[1].Value);
        Assert.Equal(123L, restored.Entities[2].Value);
        Assert.Equal(3.14, restored.Entities[3].Value);
        Assert.Equal("test", restored.Entities[4].Value);
    }
}

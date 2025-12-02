using HyperTAG.Converters;
using HyperTAG.Core;
using HyperTAG.Exceptions;
using HyperTAG.Models;
using HyperTAG.Utilities;

namespace TagTests.Converters;

public class TupleConverterTests
{
    [Fact]
    public void TupleConverter1_ToTag_WithNull_ReturnsNullTag()
    {
        // Arrange
        var converter = new TupleConverter<int>();
        Tuple<int>? tuple = null;

        // Act
        var tag = converter.ToTag(tuple);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Null, tag.Type);
    }

    [Fact]
    public void TupleConverter1_RoundTrip_PreservesData()
    {
        // Arrange
        var converter = new TupleConverter<int>();
        var original = new Tuple<int>(42);

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(original.Item1, restored.Item1);
    }

    [Fact]
    public void TupleConverter2_RoundTrip_PreservesData()
    {
        // Arrange
        var converter = new TupleConverter<int, string>();
        var original = new Tuple<int, string>(42, "hello");

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(original.Item1, restored.Item1);
        Assert.Equal(original.Item2, restored.Item2);
    }

    [Fact]
    public void TupleConverter3_RoundTrip_PreservesData()
    {
        // Arrange
        var converter = new TupleConverter<int, string, bool>();
        var original = new Tuple<int, string, bool>(42, "hello", true);

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(original.Item1, restored.Item1);
        Assert.Equal(original.Item2, restored.Item2);
        Assert.Equal(original.Item3, restored.Item3);
    }

    [Fact]
    public void TupleConverter4_RoundTrip_PreservesData()
    {
        // Arrange
        var converter = new TupleConverter<int, string, bool, double>();
        var original = new Tuple<int, string, bool, double>(42, "hello", true, 3.14);

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(original.Item1, restored.Item1);
        Assert.Equal(original.Item2, restored.Item2);
        Assert.Equal(original.Item3, restored.Item3);
        Assert.Equal(original.Item4, restored.Item4);
    }

    [Fact]
    public void TupleConverter5_RoundTrip_PreservesData()
    {
        // Arrange
        var converter = new TupleConverter<int, string, bool, double, long>();
        var original = new Tuple<int, string, bool, double, long>(42, "hello", true, 3.14, 999L);

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(original.Item1, restored.Item1);
        Assert.Equal(original.Item2, restored.Item2);
        Assert.Equal(original.Item3, restored.Item3);
        Assert.Equal(original.Item4, restored.Item4);
        Assert.Equal(original.Item5, restored.Item5);
    }

    [Fact]
    public void TupleConverter6_RoundTrip_PreservesData()
    {
        // Arrange
        var converter = new TupleConverter<int, string, bool, double, long, byte>();
        var original = new Tuple<int, string, bool, double, long, byte>(42, "hello", true, 3.14, 999L, 255);

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(original.Item1, restored.Item1);
        Assert.Equal(original.Item2, restored.Item2);
        Assert.Equal(original.Item3, restored.Item3);
        Assert.Equal(original.Item4, restored.Item4);
        Assert.Equal(original.Item5, restored.Item5);
        Assert.Equal(original.Item6, restored.Item6);
    }

    [Fact]
    public void TupleConverter7_RoundTrip_PreservesData()
    {
        // Arrange
        var converter = new TupleConverter<int, string, bool, double, long, byte, short>();
        var original = new Tuple<int, string, bool, double, long, byte, short>(42, "hello", true, 3.14, 999L, 255, 1000);

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(original.Item1, restored.Item1);
        Assert.Equal(original.Item2, restored.Item2);
        Assert.Equal(original.Item3, restored.Item3);
        Assert.Equal(original.Item4, restored.Item4);
        Assert.Equal(original.Item5, restored.Item5);
        Assert.Equal(original.Item6, restored.Item6);
        Assert.Equal(original.Item7, restored.Item7);
    }

    [Fact]
    public void TupleConverter8_RoundTrip_PreservesData()
    {
        // Arrange
        var registry = new TagConverterRegistry();
        registry.Register(new TupleConverter<int>()); // Register nested tuple converter
        
        var converter = new TupleConverter<int, string, bool, double, long, byte, short, Tuple<int>>(registry);
        var original = new Tuple<int, string, bool, double, long, byte, short, Tuple<int>>(
            42, "hello", true, 3.14, 999L, 255, 1000, new Tuple<int>(8));

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(original.Item1, restored.Item1);
        Assert.Equal(original.Item2, restored.Item2);
        Assert.Equal(original.Item3, restored.Item3);
        Assert.Equal(original.Item4, restored.Item4);
        Assert.Equal(original.Item5, restored.Item5);
        Assert.Equal(original.Item6, restored.Item6);
        Assert.Equal(original.Item7, restored.Item7);
        Assert.Equal(original.Rest.Item1, restored.Rest.Item1);
    }

    [Fact]
    public void TupleConverter8_WithNestedTuple2_RoundTrip_PreservesData()
    {
        // Arrange
        var registry = new TagConverterRegistry();
        registry.Register(new TupleConverter<int, string>()); // Register nested tuple converter
        
        var converter = new TupleConverter<int, int, int, int, int, int, int, Tuple<int, string>>(registry);
        var original = new Tuple<int, int, int, int, int, int, int, Tuple<int, string>>(
            1, 2, 3, 4, 5, 6, 7, new Tuple<int, string>(8, "ninth"));

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(original.Item1, restored.Item1);
        Assert.Equal(original.Item2, restored.Item2);
        Assert.Equal(original.Item3, restored.Item3);
        Assert.Equal(original.Item4, restored.Item4);
        Assert.Equal(original.Item5, restored.Item5);
        Assert.Equal(original.Item6, restored.Item6);
        Assert.Equal(original.Item7, restored.Item7);
        Assert.Equal(original.Rest.Item1, restored.Rest.Item1);
        Assert.Equal(original.Rest.Item2, restored.Rest.Item2);
    }

    [Fact]
    public void TupleConverter8_WithDeeplyNestedTuples_RoundTrip_PreservesData()
    {
        // Arrange - simulating a 10-element tuple structure
        var registry = new TagConverterRegistry();
        registry.Register(new TupleConverter<int, int, int>()); // Register nested tuple converter
        
        var converter = new TupleConverter<int, int, int, int, int, int, int, 
            Tuple<int, int, int>>(registry);
        var original = new Tuple<int, int, int, int, int, int, int, 
            Tuple<int, int, int>>(
            1, 2, 3, 4, 5, 6, 7, 
            new Tuple<int, int, int>(8, 9, 10));

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(original.Item1, restored.Item1);
        Assert.Equal(original.Item2, restored.Item2);
        Assert.Equal(original.Item3, restored.Item3);
        Assert.Equal(original.Item4, restored.Item4);
        Assert.Equal(original.Item5, restored.Item5);
        Assert.Equal(original.Item6, restored.Item6);
        Assert.Equal(original.Item7, restored.Item7);
        Assert.Equal(original.Rest.Item1, restored.Rest.Item1);
        Assert.Equal(original.Rest.Item2, restored.Rest.Item2);
        Assert.Equal(original.Rest.Item3, restored.Rest.Item3);
    }

    [Fact]
    public void TupleConverter2_FromTag_WithWrongEntityCount_ThrowsException()
    {
        // Arrange
        var converter = new TupleConverter<int, string>();
        var invalidTag = new HyperTag(TagDataType.Empty);
        invalidTag.Entities.Add(new HyperTag(TagDataType.Int, 42));
        // Missing second entity

        // Act & Assert
        Assert.Throws<TagAutoDeserializeException>(() => converter.FromTag(invalidTag));
    }

    [Fact]
    public void TupleConverter2_FromTag_WithWrongTagType_ThrowsException()
    {
        // Arrange
        var converter = new TupleConverter<int, string>();
        var invalidTag = new HyperTag(TagDataType.String, "wrong");

        // Act & Assert
        Assert.Throws<TagAutoDeserializeException>(() => converter.FromTag(invalidTag));
    }

    [Fact]
    public void TupleConverter2_FromTag_WithNull_ReturnsNull()
    {
        // Arrange
        var converter = new TupleConverter<int, string>();

        // Act
        var result = converter.FromTag(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void TupleConverter2_FromTag_WithNullTypeTag_ReturnsNull()
    {
        // Arrange
        var converter = new TupleConverter<int, string>();
        var nullTag = new HyperTag(TagDataType.Null);

        // Act
        var result = converter.FromTag(nullTag);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void TupleConverter3_WithMixedTypes_RoundTrip_PreservesData()
    {
        // Arrange
        var converter = new TupleConverter<int, string, double>();
        var original = new Tuple<int, string, double>(123, "test", 45.67);

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(original.Item1, restored.Item1);
        Assert.Equal(original.Item2, restored.Item2);
        Assert.Equal(original.Item3, restored.Item3);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void TupleConverter1_WithVariousIntegers_RoundTrip_PreservesData(int value)
    {
        // Arrange
        var converter = new TupleConverter<int>();
        var original = new Tuple<int>(value);

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(original.Item1, restored.Item1);
    }

    [Fact]
    public void TupleConverter2_WithEmptyStrings_RoundTrip_PreservesData()
    {
        // Arrange
        var converter = new TupleConverter<string, string>();
        var original = new Tuple<string, string>("", "");

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(original.Item1, restored.Item1);
        Assert.Equal(original.Item2, restored.Item2);
    }

    [Fact]
    public void TupleConverter1_ToTag_CreatesCorrectStructure()
    {
        // Arrange
        var converter = new TupleConverter<int>();
        var tuple = new Tuple<int>(42);

        // Act
        var tag = converter.ToTag(tuple);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Empty, tag.Type);
        Assert.Single(tag.Entities);
        Assert.Equal(TagDataType.Int, tag.Entities[0].Type);
        Assert.Equal(42, tag.Entities[0].Value);
    }

    [Fact]
    public void TupleConverter2_ToTag_CreatesCorrectStructure()
    {
        // Arrange
        var converter = new TupleConverter<int, string>();
        var tuple = new Tuple<int, string>(42, "hello");

        // Act
        var tag = converter.ToTag(tuple);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Empty, tag.Type);
        Assert.Equal(2, tag.Entities.Count);
        Assert.Equal(TagDataType.Int, tag.Entities[0].Type);
        Assert.Equal(42, tag.Entities[0].Value);
        Assert.Equal(TagDataType.String, tag.Entities[1].Type);
        Assert.Equal("hello", tag.Entities[1].Value);
    }
}

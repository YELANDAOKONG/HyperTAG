using HyperTAG.Converters;
using HyperTAG.Core;
using HyperTAG.Exceptions;
using HyperTAG.Models;
using HyperTAG.Utilities;

namespace TagTests.Converters;

public class ValueTupleConverterTests
{
    [Fact]
    public void ValueTupleConverter1_FromTag_WithNull_ThrowsArgumentNullException()
    {
        // Arrange
        var converter = new ValueTupleConverter<int>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => converter.FromTag(null));
    }

    [Fact]
    public void ValueTupleConverter1_FromTag_WithNullTypeTag_ThrowsInvalidOperationException()
    {
        // Arrange
        var converter = new ValueTupleConverter<int>();
        var nullTag = new HyperTag(TagDataType.Null);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => converter.FromTag(nullTag));
    }

    [Fact]
    public void ValueTupleConverter1_RoundTrip_PreservesData()
    {
        // Arrange
        var converter = new ValueTupleConverter<int>();
        var original = new ValueTuple<int>(42);

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.Equal(original.Item1, restored.Item1);
    }

    [Fact]
    public void ValueTupleConverter2_RoundTrip_PreservesData()
    {
        // Arrange
        var converter = new ValueTupleConverter<int, string>();
        var original = (42, "hello");

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.Equal(original.Item1, restored.Item1);
        Assert.Equal(original.Item2, restored.Item2);
    }

    [Fact]
    public void ValueTupleConverter3_RoundTrip_PreservesData()
    {
        // Arrange
        var converter = new ValueTupleConverter<int, string, bool>();
        var original = (42, "hello", true);

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.Equal(original.Item1, restored.Item1);
        Assert.Equal(original.Item2, restored.Item2);
        Assert.Equal(original.Item3, restored.Item3);
    }

    [Fact]
    public void ValueTupleConverter4_RoundTrip_PreservesData()
    {
        // Arrange
        var converter = new ValueTupleConverter<int, string, bool, double>();
        var original = (42, "hello", true, 3.14);

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.Equal(original.Item1, restored.Item1);
        Assert.Equal(original.Item2, restored.Item2);
        Assert.Equal(original.Item3, restored.Item3);
        Assert.Equal(original.Item4, restored.Item4);
    }

    [Fact]
    public void ValueTupleConverter5_RoundTrip_PreservesData()
    {
        // Arrange
        var converter = new ValueTupleConverter<int, string, bool, double, long>();
        var original = (42, "hello", true, 3.14, 999L);

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.Equal(original.Item1, restored.Item1);
        Assert.Equal(original.Item2, restored.Item2);
        Assert.Equal(original.Item3, restored.Item3);
        Assert.Equal(original.Item4, restored.Item4);
        Assert.Equal(original.Item5, restored.Item5);
    }

    [Fact]
    public void ValueTupleConverter6_RoundTrip_PreservesData()
    {
        // Arrange
        var converter = new ValueTupleConverter<int, string, bool, double, long, byte>();
        var original = (42, "hello", true, 3.14, 999L, (byte)255);

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.Equal(original.Item1, restored.Item1);
        Assert.Equal(original.Item2, restored.Item2);
        Assert.Equal(original.Item3, restored.Item3);
        Assert.Equal(original.Item4, restored.Item4);
        Assert.Equal(original.Item5, restored.Item5);
        Assert.Equal(original.Item6, restored.Item6);
    }

    [Fact]
    public void ValueTupleConverter7_RoundTrip_PreservesData()
    {
        // Arrange
        var converter = new ValueTupleConverter<int, string, bool, double, long, byte, short>();
        var original = (42, "hello", true, 3.14, 999L, (byte)255, (short)1000);

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.Equal(original.Item1, restored.Item1);
        Assert.Equal(original.Item2, restored.Item2);
        Assert.Equal(original.Item3, restored.Item3);
        Assert.Equal(original.Item4, restored.Item4);
        Assert.Equal(original.Item5, restored.Item5);
        Assert.Equal(original.Item6, restored.Item6);
        Assert.Equal(original.Item7, restored.Item7);
    }

    [Fact]
    public void ValueTupleConverter8_RoundTrip_PreservesData()
    {
        // Arrange
        var registry = new TagConverterRegistry();
        registry.Register(new ValueTupleConverter<int>()); // Register nested value tuple converter
        
        var converter = new ValueTupleConverter<int, string, bool, double, long, byte, short, ValueTuple<int>>(registry);
        var original = new ValueTuple<int, string, bool, double, long, byte, short, ValueTuple<int>>(
            42, "hello", true, 3.14, 999L, 255, 1000, new ValueTuple<int>(8));

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
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
    public void ValueTupleConverter8_WithNestedValueTuple2_RoundTrip_PreservesData()
    {
        // Arrange
        var registry = new TagConverterRegistry();
        registry.Register(new ValueTupleConverter<int, string>()); // Register nested value tuple converter
        
        var converter = new ValueTupleConverter<int, int, int, int, int, int, int, (int, string)>(registry);
        var original = new ValueTuple<int, int, int, int, int, int, int, (int, string)>(
            1, 2, 3, 4, 5, 6, 7, (8, "ninth"));

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
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
    public void ValueTupleConverter8_WithDeeplyNestedValueTuples_RoundTrip_PreservesData()
    {
        // Arrange - simulating a 10-element value tuple structure
        var registry = new TagConverterRegistry();
        registry.Register(new ValueTupleConverter<int, int, int>()); // Register nested value tuple converter
        
        var converter = new ValueTupleConverter<int, int, int, int, int, int, int, 
            (int, int, int)>(registry);
        var original = new ValueTuple<int, int, int, int, int, int, int, 
            (int, int, int)>(
            1, 2, 3, 4, 5, 6, 7, 
            (8, 9, 10));

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
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
    public void ValueTupleConverter2_FromTag_WithWrongEntityCount_ThrowsException()
    {
        // Arrange
        var converter = new ValueTupleConverter<int, string>();
        var invalidTag = new HyperTag(TagDataType.Empty);
        invalidTag.Entities.Add(new HyperTag(TagDataType.Int, 42));
        // Missing second entity

        // Act & Assert
        Assert.Throws<TagAutoDeserializeException>(() => converter.FromTag(invalidTag));
    }

    [Fact]
    public void ValueTupleConverter2_FromTag_WithWrongTagType_ThrowsException()
    {
        // Arrange
        var converter = new ValueTupleConverter<int, string>();
        var invalidTag = new HyperTag(TagDataType.String, "wrong");

        // Act & Assert
        Assert.Throws<TagAutoDeserializeException>(() => converter.FromTag(invalidTag));
    }

    [Fact]
    public void ValueTupleConverter3_WithMixedTypes_RoundTrip_PreservesData()
    {
        // Arrange
        var converter = new ValueTupleConverter<int, string, double>();
        var original = (123, "test", 45.67);

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
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
    public void ValueTupleConverter1_WithVariousIntegers_RoundTrip_PreservesData(int value)
    {
        // Arrange
        var converter = new ValueTupleConverter<int>();
        var original = new ValueTuple<int>(value);

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.Equal(original.Item1, restored.Item1);
    }

    [Fact]
    public void ValueTupleConverter2_WithEmptyStrings_RoundTrip_PreservesData()
    {
        // Arrange
        var converter = new ValueTupleConverter<string, string>();
        var original = ("", "");

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.Equal(original.Item1, restored.Item1);
        Assert.Equal(original.Item2, restored.Item2);
    }

    [Fact]
    public void ValueTupleConverter1_ToTag_CreatesCorrectStructure()
    {
        // Arrange
        var converter = new ValueTupleConverter<int>();
        var tuple = new ValueTuple<int>(42);

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
    public void ValueTupleConverter2_ToTag_CreatesCorrectStructure()
    {
        // Arrange
        var converter = new ValueTupleConverter<int, string>();
        var tuple = (42, "hello");

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

    [Fact]
    public void ValueTupleConverter2_WithDefaultValues_RoundTrip_PreservesData()
    {
        // Arrange
        var converter = new ValueTupleConverter<int, string>();
        var original = (0, default(string)!);

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.Equal(original.Item1, restored.Item1);
        Assert.Equal(original.Item2, restored.Item2);
    }

    [Fact]
    public void ValueTupleConverter3_WithAllSameType_RoundTrip_PreservesData()
    {
        // Arrange
        var converter = new ValueTupleConverter<int, int, int>();
        var original = (1, 2, 3);

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.Equal(original.Item1, restored.Item1);
        Assert.Equal(original.Item2, restored.Item2);
        Assert.Equal(original.Item3, restored.Item3);
    }

    [Fact]
    public void ValueTupleConverter4_WithBooleanValues_RoundTrip_PreservesData()
    {
        // Arrange
        var converter = new ValueTupleConverter<bool, bool, bool, bool>();
        var original = (true, false, true, false);

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.Equal(original.Item1, restored.Item1);
        Assert.Equal(original.Item2, restored.Item2);
        Assert.Equal(original.Item3, restored.Item3);
        Assert.Equal(original.Item4, restored.Item4);
    }
}

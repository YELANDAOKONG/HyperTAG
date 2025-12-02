using HyperTAG.Converters;
using HyperTAG.Core;
using HyperTAG.Interfaces;
using HyperTAG.Models;

namespace TagTests.Converters;

public class NullableConverterTests
{
    [Fact]
    public void Constructor_WithNullInnerConverter_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new NullableConverter<int>(null!));
    }

    [Fact]
    public void ToTag_WithNullValue_ReturnsNull()
    {
        // Arrange
        var innerConverter = new IntInnerConverter();
        var converter = new NullableConverter<int>(innerConverter);
        int? value = null;

        // Act
        var tag = converter.ToTag(value);

        // Assert
        Assert.Null(tag);
    }

    [Fact]
    public void ToTag_WithValue_ReturnsTagFromInnerConverter()
    {
        // Arrange
        var innerConverter = new IntInnerConverter();
        var converter = new NullableConverter<int>(innerConverter);
        int? value = 42;

        // Act
        var tag = converter.ToTag(value);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Int, tag.Type);
        Assert.Equal(42, tag.Value);
    }

    [Fact]
    public void FromTag_WithNullTag_ReturnsNull()
    {
        // Arrange
        var innerConverter = new IntInnerConverter();
        var converter = new NullableConverter<int>(innerConverter);

        // Act
        var result = converter.FromTag(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void FromTag_WithNullTypeTag_ReturnsNull()
    {
        // Arrange
        var innerConverter = new IntInnerConverter();
        var converter = new NullableConverter<int>(innerConverter);
        var tag = new HyperTag(TagDataType.Null);

        // Act
        var result = converter.FromTag(tag);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void FromTag_WithValidTag_ReturnsValue()
    {
        // Arrange
        var innerConverter = new IntInnerConverter();
        var converter = new NullableConverter<int>(innerConverter);
        var tag = new HyperTag(TagDataType.Int, 123);

        // Act
        var result = converter.FromTag(tag);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(123, result.Value);
    }

    [Fact]
    public void RoundTrip_WithValue_PreservesData()
    {
        // Arrange
        var innerConverter = new IntInnerConverter();
        var converter = new NullableConverter<int>(innerConverter);
        int? original = 999;

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.Equal(original, restored);
    }

    [Fact]
    public void RoundTrip_WithNull_PreservesNull()
    {
        // Arrange
        var innerConverter = new IntInnerConverter();
        var converter = new NullableConverter<int>(innerConverter);
        int? original = null;

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.Null(restored);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void RoundTrip_WithVariousValues_PreservesData(int value)
    {
        // Arrange
        var innerConverter = new IntInnerConverter();
        var converter = new NullableConverter<int>(innerConverter);
        int? original = value;

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.Equal(original, restored);
    }

    // Helper inner converter for testing
    private class IntInnerConverter : ITagConverter<int>
    {
        public HyperTag? ToTag(int obj)
        {
            return new HyperTag(TagDataType.Int, obj);
        }

        public int FromTag(HyperTag? tag)
        {
            if (tag == null || tag.Type != TagDataType.Int)
                throw new InvalidOperationException("Invalid tag");
            return (int)tag.Value!;
        }
    }
}

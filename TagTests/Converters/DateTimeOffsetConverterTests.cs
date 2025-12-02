using HyperTAG.Converters;
using HyperTAG.Core;
using HyperTAG.Models;

namespace TagTests.Converters;

public class DateTimeOffsetConverterTests
{
    private readonly DateTimeOffsetConverter _converter = new();

    [Fact]
    public void ToTag_WithDateTimeOffset_ReturnsValidTag()
    {
        // Arrange
        var dateTimeOffset = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(5));

        // Act
        var tag = _converter.ToTag(dateTimeOffset);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Empty, tag.Type);
        Assert.Equal(2, tag.Entities.Count);

        // Check Ticks
        var ticksField = tag.Entities[0];
        Assert.Equal("Ticks", ticksField.Value);
        Assert.Equal(TagDataType.Long, ticksField.Entities[0].Type);
        Assert.Equal(dateTimeOffset.Ticks, ticksField.Entities[0].Value);

        // Check Offset
        var offsetField = tag.Entities[1];
        Assert.Equal("Offset", offsetField.Value);
        Assert.Equal(TagDataType.Long, offsetField.Entities[0].Type);
        Assert.Equal(dateTimeOffset.Offset.Ticks, offsetField.Entities[0].Value);
    }

    [Fact]
    public void ToTag_WithUtcNow_ReturnsValidTag()
    {
        // Arrange
        var dateTimeOffset = DateTimeOffset.UtcNow;

        // Act
        var tag = _converter.ToTag(dateTimeOffset);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Empty, tag.Type);
        Assert.Equal(2, tag.Entities.Count);
    }

    [Fact]
    public void FromTag_WithValidTag_ReturnsDateTimeOffset()
    {
        // Arrange
        var original = new DateTimeOffset(2023, 12, 25, 18, 0, 0, TimeSpan.FromHours(-8));
        var tag = _converter.ToTag(original);

        // Act
        var result = _converter.FromTag(tag);

        // Assert
        Assert.Equal(original, result);
    }

    [Fact]
    public void FromTag_WithNullTag_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _converter.FromTag(null));
    }

    [Fact]
    public void FromTag_WithNullTypeTag_ThrowsInvalidOperationException()
    {
        // Arrange
        var tag = new HyperTag(TagDataType.Null);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _converter.FromTag(tag));
    }

    [Fact]
    public void FromTag_WithWrongType_ThrowsInvalidOperationException()
    {
        // Arrange
        var tag = new HyperTag(TagDataType.String, "invalid");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _converter.FromTag(tag));
    }

    [Fact]
    public void FromTag_WithMissingFields_ThrowsInvalidOperationException()
    {
        // Arrange
        var tag = new HyperTag(TagDataType.Empty);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _converter.FromTag(tag));
    }

    [Fact]
    public void RoundTrip_WithVariousOffsets_PreservesData()
    {
        // Arrange
        var original = new DateTimeOffset(2024, 6, 1, 12, 0, 0, TimeSpan.FromHours(3.5));

        // Act
        var tag = _converter.ToTag(original);
        var restored = _converter.FromTag(tag);

        // Assert
        Assert.Equal(original, restored);
        Assert.Equal(original.Offset, restored.Offset);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-5)]
    [InlineData(8)]
    [InlineData(-12)]
    public void RoundTrip_WithDifferentOffsetHours_PreservesData(int offsetHours)
    {
        // Arrange
        var original = new DateTimeOffset(2024, 3, 15, 9, 30, 0, TimeSpan.FromHours(offsetHours));

        // Act
        var tag = _converter.ToTag(original);
        var restored = _converter.FromTag(tag);

        // Assert
        Assert.Equal(original, restored);
    }

    [Fact]
    public void RoundTrip_WithMinValue_PreservesData()
    {
        // Arrange
        var original = DateTimeOffset.MinValue;

        // Act
        var tag = _converter.ToTag(original);
        var restored = _converter.FromTag(tag);

        // Assert
        Assert.Equal(original, restored);
    }

    [Fact]
    public void RoundTrip_WithMaxValue_PreservesData()
    {
        // Arrange
        var original = DateTimeOffset.MaxValue;

        // Act
        var tag = _converter.ToTag(original);
        var restored = _converter.FromTag(tag);

        // Assert
        Assert.Equal(original, restored);
    }
}

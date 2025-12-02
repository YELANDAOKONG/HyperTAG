using HyperTAG.Converters;
using HyperTAG.Core;
using HyperTAG.Models;

namespace TagTests.Converters;

public class TimeSpanConverterTests
{
    private readonly TimeSpanConverter _converter = new();

    [Fact]
    public void ToTag_WithTimeSpan_ReturnsLongTag()
    {
        // Arrange
        var timeSpan = TimeSpan.FromHours(2.5);

        // Act
        var tag = _converter.ToTag(timeSpan);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Long, tag.Type);
        Assert.Equal(timeSpan.Ticks, tag.Value);
    }

    [Fact]
    public void ToTag_WithZeroTimeSpan_ReturnsValidTag()
    {
        // Arrange
        var timeSpan = TimeSpan.Zero;

        // Act
        var tag = _converter.ToTag(timeSpan);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Long, tag.Type);
        Assert.Equal(0L, tag.Value);
    }

    [Fact]
    public void FromTag_WithValidTag_ReturnsTimeSpan()
    {
        // Arrange
        var ticks = TimeSpan.FromMinutes(45).Ticks;
        var tag = new HyperTag(TagDataType.Long, ticks);

        // Act
        var result = _converter.FromTag(tag);

        // Assert
        Assert.Equal(TimeSpan.FromMinutes(45), result);
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
        var tag = new HyperTag(TagDataType.Int, 123);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _converter.FromTag(tag));
    }

    [Fact]
    public void RoundTrip_WithPositiveTimeSpan_PreservesData()
    {
        // Arrange
        var original = TimeSpan.FromDays(3.5);

        // Act
        var tag = _converter.ToTag(original);
        var restored = _converter.FromTag(tag);

        // Assert
        Assert.Equal(original, restored);
    }

    [Fact]
    public void RoundTrip_WithNegativeTimeSpan_PreservesData()
    {
        // Arrange
        var original = TimeSpan.FromHours(-12);

        // Act
        var tag = _converter.ToTag(original);
        var restored = _converter.FromTag(tag);

        // Assert
        Assert.Equal(original, restored);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(24)]
    [InlineData(168)] // 1 week in hours
    public void RoundTrip_WithVariousHours_PreservesData(int hours)
    {
        // Arrange
        var original = TimeSpan.FromHours(hours);

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
        var original = TimeSpan.MaxValue;

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
        var original = TimeSpan.MinValue;

        // Act
        var tag = _converter.ToTag(original);
        var restored = _converter.FromTag(tag);

        // Assert
        Assert.Equal(original, restored);
    }
}

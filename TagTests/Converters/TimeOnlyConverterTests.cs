using HyperTAG.Converters;
using HyperTAG.Core;
using HyperTAG.Models;

namespace TagTests.Converters;

public class TimeOnlyConverterTests
{
    private readonly TimeOnlyConverter _converter = new();

    [Fact]
    public void ToTag_WithTimeOnly_ReturnsLongTag()
    {
        // Arrange
        var time = new TimeOnly(14, 30, 45);

        // Act
        var tag = _converter.ToTag(time);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Long, tag.Type);
        Assert.Equal(time.Ticks, tag.Value);
    }

    [Fact]
    public void ToTag_WithMidnight_ReturnsValidTag()
    {
        // Arrange
        var time = TimeOnly.MinValue;

        // Act
        var tag = _converter.ToTag(time);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Long, tag.Type);
        Assert.Equal(0L, tag.Value);
    }

    [Fact]
    public void FromTag_WithValidTag_ReturnsTimeOnly()
    {
        // Arrange
        var ticks = new TimeOnly(9, 15, 30).Ticks;
        var tag = new HyperTag(TagDataType.Long, ticks);

        // Act
        var result = _converter.FromTag(tag);

        // Assert
        Assert.Equal(new TimeOnly(9, 15, 30), result);
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
    public void RoundTrip_WithValidTime_PreservesData()
    {
        // Arrange
        var original = new TimeOnly(18, 45, 30, 500);

        // Act
        var tag = _converter.ToTag(original);
        var restored = _converter.FromTag(tag);

        // Assert
        Assert.Equal(original, restored);
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(12, 0, 0)]
    [InlineData(23, 59, 59)]
    [InlineData(6, 30, 15)]
    public void RoundTrip_WithVariousTimes_PreservesData(int hour, int minute, int second)
    {
        // Arrange
        var original = new TimeOnly(hour, minute, second);

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
        var original = TimeOnly.MinValue;

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
        var original = TimeOnly.MaxValue;

        // Act
        var tag = _converter.ToTag(original);
        var restored = _converter.FromTag(tag);

        // Assert
        Assert.Equal(original, restored);
    }
}

using HyperTAG.Converters;
using HyperTAG.Core;
using HyperTAG.Models;

namespace TagTests.Converters;

public class DateTimeConverterTests
{
    private readonly DateTimeConverter _converter = new();

    [Fact]
    public void ToTag_WithDateTime_ReturnsLongTag()
    {
        // Arrange
        var dateTime = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);

        // Act
        var tag = _converter.ToTag(dateTime);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Long, tag.Type);
        Assert.Equal(dateTime.ToBinary(), tag.Value);
    }

    [Fact]
    public void ToTag_WithLocalDateTime_PreservesKind()
    {
        // Arrange
        var dateTime = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Local);

        // Act
        var tag = _converter.ToTag(dateTime);
        var restored = _converter.FromTag(tag);

        // Assert
        Assert.Equal(DateTimeKind.Local, restored.Kind);
    }

    [Fact]
    public void ToTag_WithUtcDateTime_PreservesKind()
    {
        // Arrange
        var dateTime = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);

        // Act
        var tag = _converter.ToTag(dateTime);
        var restored = _converter.FromTag(tag);

        // Assert
        Assert.Equal(DateTimeKind.Utc, restored.Kind);
    }

    [Fact]
    public void ToTag_WithUnspecifiedDateTime_PreservesKind()
    {
        // Arrange
        var dateTime = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Unspecified);

        // Act
        var tag = _converter.ToTag(dateTime);
        var restored = _converter.FromTag(tag);

        // Assert
        Assert.Equal(DateTimeKind.Unspecified, restored.Kind);
    }

    [Fact]
    public void FromTag_WithValidTag_ReturnsDateTime()
    {
        // Arrange
        var original = new DateTime(2023, 12, 25, 18, 0, 0, DateTimeKind.Utc);
        var binary = original.ToBinary();
        var tag = new HyperTag(TagDataType.Long, binary);

        // Act
        var result = _converter.FromTag(tag);

        // Assert
        Assert.Equal(original, result);
        Assert.Equal(DateTimeKind.Utc, result.Kind);
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
    public void RoundTrip_WithUtcDateTime_PreservesData()
    {
        // Arrange
        var original = new DateTime(2024, 6, 15, 14, 30, 45, DateTimeKind.Utc);

        // Act
        var tag = _converter.ToTag(original);
        var restored = _converter.FromTag(tag);

        // Assert
        Assert.Equal(original, restored);
        Assert.Equal(original.Kind, restored.Kind);
    }

    [Fact]
    public void RoundTrip_WithLocalDateTime_PreservesData()
    {
        // Arrange
        var original = DateTime.Now;

        // Act
        var tag = _converter.ToTag(original);
        var restored = _converter.FromTag(tag);

        // Assert
        Assert.Equal(original, restored);
        Assert.Equal(original.Kind, restored.Kind);
    }

    [Theory]
    [InlineData(2024, 1, 1, 0, 0, 0)]
    [InlineData(2024, 12, 31, 23, 59, 59)]
    [InlineData(2000, 2, 29, 12, 0, 0)] // Leap year
    [InlineData(1999, 7, 15, 6, 30, 15)]
    public void RoundTrip_WithVariousDates_PreservesData(int year, int month, int day, int hour, int minute, int second)
    {
        // Arrange
        var original = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);

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
        var original = DateTime.MinValue;

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
        var original = DateTime.MaxValue;

        // Act
        var tag = _converter.ToTag(original);
        var restored = _converter.FromTag(tag);

        // Assert
        Assert.Equal(original, restored);
    }
}

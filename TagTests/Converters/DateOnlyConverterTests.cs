using HyperTAG.Converters;
using HyperTAG.Core;
using HyperTAG.Models;

namespace TagTests.Converters;

public class DateOnlyConverterTests
{
    private readonly DateOnlyConverter _converter = new();

    [Fact]
    public void ToTag_WithDateOnly_ReturnsIntTag()
    {
        // Arrange
        var date = new DateOnly(2024, 1, 15);

        // Act
        var tag = _converter.ToTag(date);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Int, tag.Type);
        Assert.Equal(date.DayNumber, tag.Value);
    }

    [Fact]
    public void ToTag_WithMinValue_ReturnsValidTag()
    {
        // Arrange
        var date = DateOnly.MinValue;

        // Act
        var tag = _converter.ToTag(date);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Int, tag.Type);
        Assert.Equal(date.DayNumber, tag.Value);
    }

    [Fact]
    public void ToTag_WithMaxValue_ReturnsValidTag()
    {
        // Arrange
        var date = DateOnly.MaxValue;

        // Act
        var tag = _converter.ToTag(date);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Int, tag.Type);
        Assert.Equal(date.DayNumber, tag.Value);
    }

    [Fact]
    public void FromTag_WithValidTag_ReturnsDateOnly()
    {
        // Arrange
        var dayNumber = new DateOnly(2023, 12, 25).DayNumber;
        var tag = new HyperTag(TagDataType.Int, dayNumber);

        // Act
        var result = _converter.FromTag(tag);

        // Assert
        Assert.Equal(new DateOnly(2023, 12, 25), result);
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
        var tag = new HyperTag(TagDataType.Long, 123L);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _converter.FromTag(tag));
    }

    [Fact]
    public void RoundTrip_WithValidDate_PreservesData()
    {
        // Arrange
        var original = new DateOnly(2024, 6, 15);

        // Act
        var tag = _converter.ToTag(original);
        var restored = _converter.FromTag(tag);

        // Assert
        Assert.Equal(original, restored);
    }

    [Theory]
    [InlineData(2024, 1, 1)]
    [InlineData(2024, 12, 31)]
    [InlineData(2000, 2, 29)] // Leap year
    [InlineData(1999, 7, 15)]
    public void RoundTrip_WithVariousDates_PreservesData(int year, int month, int day)
    {
        // Arrange
        var original = new DateOnly(year, month, day);

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
        var original = DateOnly.MinValue;

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
        var original = DateOnly.MaxValue;

        // Act
        var tag = _converter.ToTag(original);
        var restored = _converter.FromTag(tag);

        // Assert
        Assert.Equal(original, restored);
    }
}

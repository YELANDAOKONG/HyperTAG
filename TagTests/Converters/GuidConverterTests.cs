using HyperTAG.Converters;
using HyperTAG.Core;
using HyperTAG.Models;

namespace TagTests.Converters;

public class GuidConverterTests
{
    private readonly GuidConverter _converter = new();

    [Fact]
    public void ToTag_WithGuid_ReturnsDataTag()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var tag = _converter.ToTag(guid);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Data, tag.Type);
        Assert.NotNull(tag.Value);
        Assert.IsType<byte[]>(tag.Value);
        Assert.Equal(16, ((byte[])tag.Value).Length);
    }

    [Fact]
    public void ToTag_WithEmptyGuid_ReturnsValidTag()
    {
        // Arrange
        var guid = Guid.Empty;

        // Act
        var tag = _converter.ToTag(guid);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Data, tag.Type);
        var bytes = (byte[])tag.Value!;
        Assert.Equal(16, bytes.Length);
        Assert.All(bytes, b => Assert.Equal(0, b));
    }

    [Fact]
    public void FromTag_WithValidTag_ReturnsGuid()
    {
        // Arrange
        var original = Guid.NewGuid();
        var bytes = original.ToByteArray();
        var tag = new HyperTag(TagDataType.Data, bytes);

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
    public void FromTag_WithInvalidByteLength_ThrowsInvalidOperationException()
    {
        // Arrange
        var tag = new HyperTag(TagDataType.Data, new byte[8]); // Wrong length

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _converter.FromTag(tag));
    }

    [Fact]
    public void RoundTrip_WithNewGuid_PreservesData()
    {
        // Arrange
        var original = Guid.NewGuid();

        // Act
        var tag = _converter.ToTag(original);
        var restored = _converter.FromTag(tag);

        // Assert
        Assert.Equal(original, restored);
    }

    [Fact]
    public void RoundTrip_WithEmptyGuid_PreservesData()
    {
        // Arrange
        var original = Guid.Empty;

        // Act
        var tag = _converter.ToTag(original);
        var restored = _converter.FromTag(tag);

        // Assert
        Assert.Equal(original, restored);
    }

    [Fact]
    public void RoundTrip_WithMultipleGuids_PreservesEach()
    {
        // Arrange
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        var guid3 = Guid.NewGuid();

        // Act & Assert
        var tag1 = _converter.ToTag(guid1);
        var restored1 = _converter.FromTag(tag1);
        Assert.Equal(guid1, restored1);

        var tag2 = _converter.ToTag(guid2);
        var restored2 = _converter.FromTag(tag2);
        Assert.Equal(guid2, restored2);

        var tag3 = _converter.ToTag(guid3);
        var restored3 = _converter.FromTag(tag3);
        Assert.Equal(guid3, restored3);
    }

    [Fact]
    public void RoundTrip_WithKnownGuid_PreservesData()
    {
        // Arrange
        var original = new Guid("12345678-1234-1234-1234-123456789abc");

        // Act
        var tag = _converter.ToTag(original);
        var restored = _converter.FromTag(tag);

        // Assert
        Assert.Equal(original, restored);
    }
}

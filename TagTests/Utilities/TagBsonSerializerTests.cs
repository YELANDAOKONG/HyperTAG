using HyperTAG.Models;
using HyperTAG.Structs;
using HyperTAG.Utilities;

namespace TagTests.Utilities;

public class TagBsonSerializerTests
{
    [Fact]
    public void Serialize_EmptyTag_ReturnsByteArray()
    {
        // Arrange
        var tag = new TagStruct(TagDataType.Empty, null);
        
        // Act
        var bson = TagBsonSerializer.Serialize(tag);
        
        // Assert
        Assert.NotNull(bson);
        Assert.NotEmpty(bson);
    }
    
    [Fact]
    public void Serialize_NullTag_ReturnsByteArray()
    {
        // Arrange
        var tag = new TagStruct(TagDataType.Null, null);
        
        // Act
        var bson = TagBsonSerializer.Serialize(tag);
        
        // Assert
        Assert.NotNull(bson);
        Assert.NotEmpty(bson);
    }
    
    [Fact]
    public void Serialize_BoolTag_ReturnsByteArray()
    {
        // Arrange
        var tag = new TagStruct(TagDataType.Bool, true);
        
        // Act
        var bson = TagBsonSerializer.Serialize(tag);
        
        // Assert
        Assert.NotNull(bson);
        Assert.NotEmpty(bson);
    }
    
    [Fact]
    public void Serialize_IntTag_ReturnsByteArray()
    {
        // Arrange
        var tag = new TagStruct(TagDataType.Int, 42);
        
        // Act
        var bson = TagBsonSerializer.Serialize(tag);
        
        // Assert
        Assert.NotNull(bson);
        Assert.NotEmpty(bson);
    }
    
    [Fact]
    public void Serialize_StringTag_ReturnsByteArray()
    {
        // Arrange
        var tag = new TagStruct(TagDataType.String, "Hello BSON");
        
        // Act
        var bson = TagBsonSerializer.Serialize(tag);
        
        // Assert
        Assert.NotNull(bson);
        Assert.NotEmpty(bson);
    }
    
    [Fact]
    public void Serialize_DecimalTag_ReturnsByteArray()
    {
        // Arrange
        var tag = new TagStruct(TagDataType.Decimal, 999.999m);
        
        // Act
        var bson = TagBsonSerializer.Serialize(tag);
        
        // Assert
        Assert.NotNull(bson);
        Assert.NotEmpty(bson);
    }
    
    [Fact]
    public void Serialize_DoubleTag_ReturnsByteArray()
    {
        // Arrange
        var tag = new TagStruct(TagDataType.Double, 3.14159);
        
        // Act
        var bson = TagBsonSerializer.Serialize(tag);
        
        // Assert
        Assert.NotNull(bson);
        Assert.NotEmpty(bson);
    }
    
    [Fact]
    public void Serialize_IntArrayTag_ReturnsByteArray()
    {
        // Arrange
        var tag = new TagStruct(TagDataType.IntArray, new int[] { 1, 2, 3, 4, 5 });
        
        // Act
        var bson = TagBsonSerializer.Serialize(tag);
        
        // Assert
        Assert.NotNull(bson);
        Assert.NotEmpty(bson);
    }
    
    [Fact]
    public void Serialize_StringArrayTag_ReturnsByteArray()
    {
        // Arrange
        var tag = new TagStruct(TagDataType.StringArray, new string[] { "A", "B", "C" });
        
        // Act
        var bson = TagBsonSerializer.Serialize(tag);
        
        // Assert
        Assert.NotNull(bson);
        Assert.NotEmpty(bson);
    }
    
    [Fact]
    public void Serialize_ByteArrayTag_ReturnsByteArray()
    {
        // Arrange
        var tag = new TagStruct(TagDataType.Data, new byte[] { 0x01, 0x02, 0x03 });
        
        // Act
        var bson = TagBsonSerializer.Serialize(tag);
        
        // Assert
        Assert.NotNull(bson);
        Assert.NotEmpty(bson);
    }
    
    [Fact]
    public void Serialize_NestedTag_ReturnsByteArray()
    {
        // Arrange
        var parent = new TagStruct(TagDataType.Empty, null);
        parent.Entities.Add(new TagStruct(TagDataType.String, "Child1"));
        parent.Entities.Add(new TagStruct(TagDataType.Int, 100));
        
        // Act
        var bson = TagBsonSerializer.Serialize(parent);
        
        // Assert
        Assert.NotNull(bson);
        Assert.NotEmpty(bson);
    }
    
    [Fact]
    public void Serialize_ComplexNestedStructure_ReturnsByteArray()
    {
        // Arrange
        var root = new TagStruct(TagDataType.Empty, null);
        var level1 = new TagStruct(TagDataType.Empty, null);
        level1.Entities.Add(new TagStruct(TagDataType.String, "Deep"));
        level1.Entities.Add(new TagStruct(TagDataType.Bool, false));
        root.Entities.Add(level1);
        root.Entities.Add(new TagStruct(TagDataType.Double, 3.14));
        
        // Act
        var bson = TagBsonSerializer.Serialize(root);
        
        // Assert
        Assert.NotNull(bson);
        Assert.NotEmpty(bson);
    }
    
    [Fact]
    public void Serialize_WithThrowExceptionsFalse_ReturnsValidData()
    {
        // Arrange
        var tag = new TagStruct(TagDataType.Int, 42);
        
        // Act
        var bson = TagBsonSerializer.Serialize(tag, throwExceptions: false);
        
        // Assert
        Assert.NotNull(bson);
    }
    
    [Fact]
    public void Serialize_ProducesSmallerThanJson()
    {
        // Arrange
        var tag = new TagStruct(TagDataType.String, "This is a test string for size comparison");
        
        // Act
        var bson = TagBsonSerializer.Serialize(tag);
        var json = TagJsonSerializer.Serialize(tag);
        
        // Assert
        Assert.NotNull(bson);
        Assert.NotNull(json);
        // BSON is typically more compact than formatted JSON
        Assert.True(bson.Length < json.Length);
    }
}

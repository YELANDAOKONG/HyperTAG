using HyperTAG.Models;
using HyperTAG.Structs;
using HyperTAG.Utilities;

namespace TagTests.Utilities;

public class TagJsonSerializerTests
{
    [Fact]
    public void Serialize_EmptyTag_ReturnsValidJson()
    {
        // Arrange
        var tag = new TagStruct(TagDataType.Empty, null);
        
        // Act
        var json = TagJsonSerializer.Serialize(tag);
        
        // Assert
        Assert.NotNull(json);
        Assert.Contains("\"Type\": 0", json);
    }
    
    [Fact]
    public void Serialize_NullTag_ReturnsValidJson()
    {
        // Arrange
        var tag = new TagStruct(TagDataType.Null, null);
        
        // Act
        var json = TagJsonSerializer.Serialize(tag);
        
        // Assert
        Assert.NotNull(json);
        Assert.Contains("\"Type\": 1", json);
    }
    
    [Fact]
    public void Serialize_BoolTag_ReturnsValidJson()
    {
        // Arrange
        var tag = new TagStruct(TagDataType.Bool, true);
        
        // Act
        var json = TagJsonSerializer.Serialize(tag);
        
        // Assert
        Assert.NotNull(json);
        Assert.Contains("\"Type\": 3", json);
        Assert.Contains("\"Value\": true", json);
    }
    
    [Fact]
    public void Serialize_IntTag_ReturnsValidJson()
    {
        // Arrange
        var tag = new TagStruct(TagDataType.Int, 42);
        
        // Act
        var json = TagJsonSerializer.Serialize(tag);
        
        // Assert
        Assert.NotNull(json);
        Assert.Contains("\"Type\": 7", json);
        Assert.Contains("\"Value\": 42", json);
    }
    
    [Fact]
    public void Serialize_StringTag_ReturnsValidJson()
    {
        // Arrange
        var tag = new TagStruct(TagDataType.String, "Hello World");
        
        // Act
        var json = TagJsonSerializer.Serialize(tag);
        
        // Assert
        Assert.NotNull(json);
        Assert.Contains("\"Type\": 11", json);
        Assert.Contains("\"Value\": \"Hello World\"", json);
    }
    
    [Fact]
    public void Serialize_DecimalTag_ReturnsValidJson()
    {
        // Arrange
        var tag = new TagStruct(TagDataType.Decimal, 123.456m);
        
        // Act
        var json = TagJsonSerializer.Serialize(tag);
        
        // Assert
        Assert.NotNull(json);
        Assert.Contains("\"Type\": 12", json);
        Assert.Contains("123.456", json);
    }
    
    [Fact]
    public void Serialize_IntArrayTag_ReturnsValidJson()
    {
        // Arrange
        var tag = new TagStruct(TagDataType.IntArray, new int[] { 1, 2, 3, 4, 5 });
        
        // Act
        var json = TagJsonSerializer.Serialize(tag);
        
        // Assert
        Assert.NotNull(json);
        Assert.Contains("\"Type\": 21", json);
        Assert.Contains("1", json);
        Assert.Contains("5", json);
    }
    
    [Fact]
    public void Serialize_StringArrayTag_ReturnsValidJson()
    {
        // Arrange
        var tag = new TagStruct(TagDataType.StringArray, new string[] { "A", "B", "C" });
        
        // Act
        var json = TagJsonSerializer.Serialize(tag);
        
        // Assert
        Assert.NotNull(json);
        Assert.Contains("\"Type\": 25", json);
        Assert.Contains("\"A\"", json);
        Assert.Contains("\"C\"", json);
    }
    
    [Fact]
    public void Serialize_ByteArrayTag_ReturnsValidJson()
    {
        // Arrange
        var tag = new TagStruct(TagDataType.ByteArray, new byte[] { 1, 2, 3 });
        
        // Act
        var json = TagJsonSerializer.Serialize(tag);
        
        // Assert
        Assert.NotNull(json);
        Assert.Contains("\"Type\": 19", json);
    }
    
    [Fact]
    public void Serialize_NestedTag_ReturnsValidJson()
    {
        // Arrange
        var parent = new TagStruct(TagDataType.Empty, null);
        var child1 = new TagStruct(TagDataType.String, "Child1");
        var child2 = new TagStruct(TagDataType.Int, 100);
        parent.Entities.Add(child1);
        parent.Entities.Add(child2);
        
        // Act
        var json = TagJsonSerializer.Serialize(parent);
        
        // Assert
        Assert.NotNull(json);
        Assert.Contains("\"Entities\"", json);
        Assert.Contains("Child1", json);
        Assert.Contains("100", json);
    }
    
    [Fact]
    public void Serialize_DeeplyNestedTag_ReturnsValidJson()
    {
        // Arrange
        var root = new TagStruct(TagDataType.Empty, null);
        var level1 = new TagStruct(TagDataType.Empty, null);
        var level2 = new TagStruct(TagDataType.String, "Deep");
        level1.Entities.Add(level2);
        root.Entities.Add(level1);
        
        // Act
        var json = TagJsonSerializer.Serialize(root);
        
        // Assert
        Assert.NotNull(json);
        Assert.Contains("Deep", json);
    }
    
    [Fact]
    public void Serialize_WithThrowExceptionsFalse_ReturnsValidJson()
    {
        // Arrange
        var tag = new TagStruct(TagDataType.Int, 42);
        
        // Act
        var json = TagJsonSerializer.Serialize(tag, throwExceptions: false);
        
        // Assert
        Assert.NotNull(json);
    }
    
    [Fact]
    public void Serialize_UsesIndentedFormatting()
    {
        // Arrange
        var tag = new TagStruct(TagDataType.String, "Test");
        
        // Act
        var json = TagJsonSerializer.Serialize(tag);
        
        // Assert
        Assert.NotNull(json);
        Assert.Contains("\n", json); // Check for newlines (indented format)
    }
}

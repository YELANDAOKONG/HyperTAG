using HyperTAG.Models;
using HyperTAG.Structs;
using HyperTAG.Utilities;

namespace TagTests.Utilities;

public class TagJsonDeserializerTests
{
    [Fact]
    public void Deserialize_EmptyTagJson_ReturnsTagStruct()
    {
        // Arrange
        var json = @"{""Type"": 0, ""Value"": null, ""Entities"": []}";
        
        // Act
        var result = TagJsonDeserializer.Deserialize(json);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(TagDataType.Empty, result.Type);
        Assert.Null(result.Value);
        Assert.Empty(result.Entities);
    }
    
    [Fact]
    public void Deserialize_BoolTagJson_ReturnsTagStruct()
    {
        // Arrange
        var json = @"{""Type"": 3, ""Value"": true, ""Entities"": []}";
        
        // Act
        var result = TagJsonDeserializer.Deserialize(json);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(TagDataType.Bool, result.Type);
        Assert.True((bool)result.Value!);
    }
    
    [Fact]
    public void Deserialize_IntTagJson_ReturnsTagStruct()
    {
        // Arrange
        var json = @"{""Type"": 7, ""Value"": 42, ""Entities"": []}";
        
        // Act
        var result = TagJsonDeserializer.Deserialize(json);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(TagDataType.Int, result.Type);
        Assert.Equal(42L, result.Value); // JSON.NET deserializes numbers as long
    }
    
    [Fact]
    public void Deserialize_StringTagJson_ReturnsTagStruct()
    {
        // Arrange
        var json = @"{""Type"": 11, ""Value"": ""Hello World"", ""Entities"": []}";
        
        // Act
        var result = TagJsonDeserializer.Deserialize(json);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(TagDataType.String, result.Type);
        Assert.Equal("Hello World", result.Value);
    }
    
    [Fact]
    public void Deserialize_DecimalTagJson_ReturnsTagStruct()
    {
        // Arrange
        var json = @"{""Type"": 12, ""Value"": 123.456, ""Entities"": []}";
        
        // Act
        var result = TagJsonDeserializer.Deserialize(json);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(TagDataType.Decimal, result.Type);
        Assert.NotNull(result.Value);
    }
    
    [Fact]
    public void Deserialize_NestedTagJson_ReturnsTagStruct()
    {
        // Arrange
        var json = @"{
            ""Type"": 0,
            ""Value"": null,
            ""Entities"": [
                {""Type"": 11, ""Value"": ""Child1"", ""Entities"": []},
                {""Type"": 7, ""Value"": 100, ""Entities"": []}
            ]
        }";
        
        // Act
        var result = TagJsonDeserializer.Deserialize(json);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Entities.Count);
        Assert.Equal(TagDataType.String, result.Entities[0].Type);
        Assert.Equal("Child1", result.Entities[0].Value);
        Assert.Equal(TagDataType.Int, result.Entities[1].Type);
    }
    
    [Fact]
    public void Deserialize_InvalidJson_ReturnsNull()
    {
        // Arrange
        var json = "not valid json";
        
        // Act
        var result = TagJsonDeserializer.Deserialize(json, throwExceptions: false);
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public void Deserialize_InvalidJson_ThrowsException()
    {
        // Arrange
        var json = "not valid json";
        
        // Act & Assert
        Assert.ThrowsAny<Exception>(() => 
            TagJsonDeserializer.Deserialize(json, throwExceptions: true));
    }
    
    [Fact]
    public void Deserialize_EmptyString_ReturnsNull()
    {
        // Arrange
        var json = "";
        
        // Act
        var result = TagJsonDeserializer.Deserialize(json, throwExceptions: false);
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public void Deserialize_RoundTrip_PreservesData()
    {
        // Arrange
        var original = new TagStruct(TagDataType.String, "Test");
        original.Entities.Add(new TagStruct(TagDataType.Int, 123));
        
        var json = TagJsonSerializer.Serialize(original);
        
        // Act
        var result = TagJsonDeserializer.Deserialize(json!);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(original.Type, result.Type);
        Assert.Equal(original.Value, result.Value);
        Assert.Equal(original.Entities.Count, result.Entities.Count);
    }
    
    [Fact]
    public void Deserialize_ComplexRoundTrip_PreservesStructure()
    {
        // Arrange
        var original = new TagStruct(TagDataType.Empty, null);
        var child1 = new TagStruct(TagDataType.String, "Child");
        child1.Entities.Add(new TagStruct(TagDataType.Bool, true));
        original.Entities.Add(child1);
        original.Entities.Add(new TagStruct(TagDataType.Double, 3.14));
        
        var json = TagJsonSerializer.Serialize(original);
        
        // Act
        var result = TagJsonDeserializer.Deserialize(json!);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Entities.Count);
        Assert.Single(result.Entities[0].Entities);
    }
}

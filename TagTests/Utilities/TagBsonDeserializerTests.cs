using HyperTAG.Models;
using HyperTAG.Structs;
using HyperTAG.Utilities;

namespace TagTests.Utilities;

public class TagBsonDeserializerTests
{
    [Fact]
    public void Deserialize_EmptyTag_ReturnsTagStruct()
    {
        // Arrange
        var original = new TagStruct(TagDataType.Empty, null);
        var bson = TagBsonSerializer.Serialize(original);
        
        // Act
        var result = TagBsonDeserializer.Deserialize(bson!);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(TagDataType.Empty, result.Type);
        Assert.Null(result.Value);
    }
    
    [Fact]
    public void Deserialize_BoolTag_ReturnsTagStruct()
    {
        // Arrange
        var original = new TagStruct(TagDataType.Bool, true);
        var bson = TagBsonSerializer.Serialize(original);
        
        // Act
        var result = TagBsonDeserializer.Deserialize(bson!);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(TagDataType.Bool, result.Type);
        Assert.True((bool)result.Value!);
    }
    
    [Fact]
    public void Deserialize_IntTag_ReturnsTagStruct()
    {
        // Arrange
        var original = new TagStruct(TagDataType.Int, 42);
        var bson = TagBsonSerializer.Serialize(original);
        
        // Act
        var result = TagBsonDeserializer.Deserialize(bson!);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(TagDataType.Int, result.Type);
        Assert.Equal(42, Convert.ToInt32(result.Value));
    }
    
    [Fact]
    public void Deserialize_StringTag_ReturnsTagStruct()
    {
        // Arrange
        var original = new TagStruct(TagDataType.String, "Hello BSON");
        var bson = TagBsonSerializer.Serialize(original);
        
        // Act
        var result = TagBsonDeserializer.Deserialize(bson!);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(TagDataType.String, result.Type);
        Assert.Equal("Hello BSON", result.Value);
    }
    
    [Fact]
    public void Deserialize_DecimalTag_ReturnsTagStruct()
    {
        // Arrange
        var original = new TagStruct(TagDataType.Decimal, 123.456m);
        var bson = TagBsonSerializer.Serialize(original);
        
        // Act
        var result = TagBsonDeserializer.Deserialize(bson!);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(TagDataType.Decimal, result.Type);
        Assert.NotNull(result.Value);
    }
    
    [Fact]
    public void Deserialize_DoubleTag_ReturnsTagStruct()
    {
        // Arrange
        var original = new TagStruct(TagDataType.Double, 3.14159);
        var bson = TagBsonSerializer.Serialize(original);
        
        // Act
        var result = TagBsonDeserializer.Deserialize(bson!);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(TagDataType.Double, result.Type);
        Assert.Equal(3.14159, (double)result.Value!, 5);
    }
    
    [Fact]
    public void Deserialize_IntArrayTag_ReturnsTagStruct()
    {
        // Arrange
        var original = new TagStruct(TagDataType.IntArray, new int[] { 1, 2, 3 });
        var bson = TagBsonSerializer.Serialize(original);
        
        // Act
        var result = TagBsonDeserializer.Deserialize(bson!);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(TagDataType.IntArray, result.Type);
        Assert.NotNull(result.Value);
    }
    
    [Fact]
    public void Deserialize_NestedTag_ReturnsTagStruct()
    {
        // Arrange
        var original = new TagStruct(TagDataType.Empty, null);
        original.Entities.Add(new TagStruct(TagDataType.String, "Child1"));
        original.Entities.Add(new TagStruct(TagDataType.Int, 100));
        var bson = TagBsonSerializer.Serialize(original);
        
        // Act
        var result = TagBsonDeserializer.Deserialize(bson!);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Entities.Count);
        Assert.Equal(TagDataType.String, result.Entities[0].Type);
        Assert.Equal("Child1", result.Entities[0].Value);
        Assert.Equal(TagDataType.Int, result.Entities[1].Type);
    }
    
    [Fact]
    public void Deserialize_InvalidBson_ReturnsNull()
    {
        // Arrange - Use clearly invalid BSON data (random bytes)
        var invalidBson = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
        
        // Act
        var result = TagBsonDeserializer.Deserialize(invalidBson, throwExceptions: false);
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public void Deserialize_InvalidBson_ThrowsException()
    {
        // Arrange - Use clearly invalid BSON data (random bytes)
        var invalidBson = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
        
        // Act & Assert
        Assert.ThrowsAny<Exception>(() => 
            TagBsonDeserializer.Deserialize(invalidBson, throwExceptions: true));
    }
    
    [Fact]
    public void Deserialize_EmptyArray_ReturnsEmptyTagStruct()
    {
        // Arrange
        var emptyBson = Array.Empty<byte>();
        
        // Act
        var result = TagBsonDeserializer.Deserialize(emptyBson, throwExceptions: false);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(TagDataType.Empty, result.Type);
        Assert.Null(result.Value);
        Assert.Empty(result.Entities);
    }
    
    [Fact]
    public void Deserialize_RoundTrip_PreservesData()
    {
        // Arrange
        var original = new TagStruct(TagDataType.String, "RoundTrip Test");
        original.Entities.Add(new TagStruct(TagDataType.Int, 999));
        original.Entities.Add(new TagStruct(TagDataType.Bool, false));
        
        var bson = TagBsonSerializer.Serialize(original);
        
        // Act
        var result = TagBsonDeserializer.Deserialize(bson!);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(original.Type, result.Type);
        Assert.Equal(original.Value, result.Value);
        Assert.Equal(original.Entities.Count, result.Entities.Count);
    }
    
    [Fact]
    public void Deserialize_ComplexNestedStructure_PreservesStructure()
    {
        // Arrange
        var root = new TagStruct(TagDataType.Empty, null);
        var level1 = new TagStruct(TagDataType.Empty, null);
        level1.Entities.Add(new TagStruct(TagDataType.String, "Deep"));
        level1.Entities.Add(new TagStruct(TagDataType.Double, 2.718));
        root.Entities.Add(level1);
        root.Entities.Add(new TagStruct(TagDataType.Long, 1234567890L));
        
        var bson = TagBsonSerializer.Serialize(root);
        
        // Act
        var result = TagBsonDeserializer.Deserialize(bson!);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Entities.Count);
        Assert.NotEmpty(result.Entities[0].Entities);
        Assert.Equal("Deep", result.Entities[0].Entities[0].Value);
    }
    
    [Fact]
    public void Deserialize_MultipleRoundTrips_MaintainsConsistency()
    {
        // Arrange
        var original = new TagStruct(TagDataType.String, "Consistency Test");
        
        // Act - Multiple serialization/deserialization cycles
        var bson1 = TagBsonSerializer.Serialize(original);
        var result1 = TagBsonDeserializer.Deserialize(bson1!);
        var bson2 = TagBsonSerializer.Serialize(result1!);
        var result2 = TagBsonDeserializer.Deserialize(bson2!);
        
        // Assert
        Assert.NotNull(result2);
        Assert.Equal(original.Type, result2.Type);
        Assert.Equal(original.Value, result2.Value);
    }
}

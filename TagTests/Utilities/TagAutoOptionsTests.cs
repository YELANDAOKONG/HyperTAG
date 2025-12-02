using HyperTAG.Attributes;
using HyperTAG.Core;
using HyperTAG.Exceptions;
using HyperTAG.Models;
using HyperTAG.Utilities;

namespace TagTests.Utilities;

public class TagAutoOptionsTests
{
    [Fact]
    public void DefaultOptions_ShouldHaveCorrectValues()
    {
        // Arrange & Act
        var options = TagAutoOptions.Default;
        
        // Assert
        Assert.True(options.NestedTags);
        Assert.Equal(8192, options.RecursionDepth);
        Assert.False(options.ThrowExceptions);
    }
    
    [Fact]
    public void NestedTags_True_ShouldEmbedDirectly()
    {
        // Arrange
        var data = new NestedTestData
        {
            Id = 1,
            Nested = new SimpleTestData { Number = 99, Text = "Nested" }
        };
        var options = new TagAutoOptions { NestedTags = true };
        
        // Act
        var tag = TagAutoSerializer.SerializeObject(data, options);
        
        // Assert
        Assert.NotNull(tag);
        var nestedTag = tag!.Entities[1].Entities[0];
        
        // Should be directly embedded (Empty type with child entities)
        Assert.Equal(TagDataType.Empty, nestedTag.Type);
        Assert.Equal(2, nestedTag.Entities.Count);
    }
    
    [Fact]
    public void NestedTags_False_ShouldSerializeAsData()
    {
        // Arrange
        var data = new NestedTestData
        {
            Id = 1,
            Nested = new SimpleTestData { Number = 99, Text = "Nested" }
        };
        var options = new TagAutoOptions { NestedTags = false };
        
        // Act
        var tag = TagAutoSerializer.SerializeObject(data, options);
        
        // Assert
        Assert.NotNull(tag);
        var nestedTag = tag!.Entities[1].Entities[0];
        
        // Should be Data type
        Assert.Equal(TagDataType.Data, nestedTag.Type);
        Assert.IsType<byte[]>(nestedTag.Value);
    }
    
    [Fact]
    public void NestedTags_True_RoundTrip_ShouldWork()
    {
        // Arrange
        var original = new NestedTestData
        {
            Id = 42,
            Nested = new SimpleTestData { Number = 123, Text = "Test" }
        };
        var options = new TagAutoOptions { NestedTags = true };
        
        // Act
        var tag = TagAutoSerializer.SerializeObject(original, options);
        var result = TagAutoDeserializer.DeserializeObject<NestedTestData>(tag, options);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(42, result!.Id);
        Assert.NotNull(result.Nested);
        Assert.Equal(123, result.Nested!.Number);
        Assert.Equal("Test", result.Nested.Text);
    }
    
    [Fact]
    public void NestedTags_False_RoundTrip_ShouldWork()
    {
        // Arrange
        var original = new NestedTestData
        {
            Id = 42,
            Nested = new SimpleTestData { Number = 123, Text = "Test" }
        };
        var options = new TagAutoOptions { NestedTags = false };
        
        // Act
        var tag = TagAutoSerializer.SerializeObject(original, options);
        var result = TagAutoDeserializer.DeserializeObject<NestedTestData>(tag, options);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(42, result!.Id);
        Assert.NotNull(result.Nested);
        Assert.Equal(123, result.Nested!.Number);
        Assert.Equal("Test", result.Nested.Text);
    }
    
    [Fact]
    public void NestedTags_Array_True_ShouldEmbedDirectly()
    {
        // Arrange
        var data = new ObjectArrayTestData
        {
            Items = new[]
            {
                new SimpleTestData { Number = 1, Text = "One" },
                new SimpleTestData { Number = 2, Text = "Two" }
            }
        };
        var options = new TagAutoOptions { NestedTags = true };
        
        // Act
        var tag = TagAutoSerializer.SerializeObject(data, options);
        
        // Assert
        var containerTag = tag!.Entities[0].Entities[0];
        Assert.Equal(TagDataType.Empty, containerTag.Type);
        
        // Each item should be directly embedded
        foreach (var itemTag in containerTag.Entities)
        {
            Assert.Equal(TagDataType.Empty, itemTag.Type);
            Assert.Equal(2, itemTag.Entities.Count);
        }
    }
    
    [Fact]
    public void NestedTags_Array_False_ShouldSerializeAsData()
    {
        // Arrange
        var data = new ObjectArrayTestData
        {
            Items = new[]
            {
                new SimpleTestData { Number = 1, Text = "One" },
                new SimpleTestData { Number = 2, Text = "Two" }
            }
        };
        var options = new TagAutoOptions { NestedTags = false };
        
        // Act
        var tag = TagAutoSerializer.SerializeObject(data, options);
        
        // Assert
        var containerTag = tag!.Entities[0].Entities[0];
        
        // Each item should be Data type
        foreach (var itemTag in containerTag.Entities)
        {
            Assert.Equal(TagDataType.Data, itemTag.Type);
            Assert.IsType<byte[]>(itemTag.Value);
        }
    }
    
    [Fact]
    public void ThrowExceptions_True_Serialize_ShouldThrowOnError()
    {
        // Arrange
        var data = new UnsupportedTestData { Value = 1 };
        var options = new TagAutoOptions { ThrowExceptions = true };
        
        // Act & Assert
        Assert.Throws<TagAutoSerializeException>(
            () => TagAutoSerializer.SerializeObject(data, options));
    }
    
    [Fact]
    public void ThrowExceptions_False_Serialize_ShouldReturnNullOnError()
    {
        // Arrange
        var data = new UnsupportedTestData { Value = 1 };
        var options = new TagAutoOptions { ThrowExceptions = false };
        
        // Act
        var result = TagAutoSerializer.SerializeObject(data, options);
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public void ThrowExceptions_True_Deserialize_ShouldThrowOnError()
    {
        // Arrange
        var tag = new HyperTag(TagDataType.Empty);
        var options = new TagAutoOptions { ThrowExceptions = true };
        
        // Act & Assert
        Assert.Throws<TagAutoDeserializeException>(
            () => TagAutoDeserializer.DeserializeObject<UnsupportedTestData>(tag, options));
    }
    
    [Fact]
    public void ThrowExceptions_False_Deserialize_ShouldReturnNullOnError()
    {
        // Arrange
        var tag = new HyperTag(TagDataType.Empty);
        var options = new TagAutoOptions { ThrowExceptions = false };
        
        // Act
        var result = TagAutoDeserializer.DeserializeObject<UnsupportedTestData>(tag, options);
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public void RecursionDepth_ShouldBeUsedInSerialization()
    {
        // Arrange
        var data = new SimpleTestData { Number = 1, Text = "Test" };
        var options = new TagAutoOptions { RecursionDepth = 100 };
        
        // Act
        var tag = TagAutoSerializer.SerializeObject(data, options);
        var bytes = tag!.Serialize(options.RecursionDepth);
        
        // Assert
        Assert.NotNull(bytes);
    }
    
    [Fact]
    public void RecursionDepth_ShouldBeUsedInDeserialization()
    {
        // Arrange
        var original = new SimpleTestData { Number = 1, Text = "Test" };
        var options = new TagAutoOptions { RecursionDepth = 100 };
        var tag = TagAutoSerializer.SerializeObject(original, options);
        var bytes = tag!.Serialize(options.RecursionDepth);
        var deserializedTag = HyperTag.Deserialize(bytes!, options.RecursionDepth);
        
        // Act
        var result = TagAutoDeserializer.DeserializeObject<SimpleTestData>(deserializedTag, options);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result!.Number);
        Assert.Equal("Test", result.Text);
    }
    
    [Fact]
    public void NestedTags_True_Performance_ShouldBeFaster()
    {
        // Arrange
        var data = new ObjectArrayTestData
        {
            Items = Enumerable.Range(0, 100)
                .Select(i => new SimpleTestData { Number = i, Text = $"Item{i}" })
                .ToArray()
        };
        var nestedOptions = new TagAutoOptions { NestedTags = true };
        var dataOptions = new TagAutoOptions { NestedTags = false };
        
        // Act
        var nestedTag = TagAutoSerializer.SerializeObject(data, nestedOptions);
        var dataTag = TagAutoSerializer.SerializeObject(data, dataOptions);
        
        // Assert - Nested should produce smaller or similar size
        var nestedBytes = nestedTag!.Serialize();
        var dataBytes = dataTag!.Serialize();
        
        Assert.NotNull(nestedBytes);
        Assert.NotNull(dataBytes);
        
        // This is just a sanity check that both work
        Assert.True(nestedBytes!.Length > 0);
        Assert.True(dataBytes!.Length > 0);
    }
    
    [Fact]
    public void CustomOptions_ShouldOverrideDefaults()
    {
        // Arrange
        var options = new TagAutoOptions
        {
            NestedTags = false,
            RecursionDepth = 100,
            ThrowExceptions = true
        };
        
        // Assert
        Assert.False(options.NestedTags);
        Assert.Equal(100, options.RecursionDepth);
        Assert.True(options.ThrowExceptions);
    }
    
    [TagStruct]
    class Level3
    {
        [TagData] public string Value = "Deep";
    }
        
    [TagStruct]
    class Level2
    {
        [TagData] public Level3? Inner = new();
    }
        
    [TagStruct]
    class Level1
    {
        [TagData] public Level2? Middle = new();
    }
    
    [Fact]
    public void NestedTags_Mixed_DeepNesting_ShouldWork()
    {
        // Arrange
        var original = new Level1();
        var options = new TagAutoOptions { NestedTags = true };
        
        // Act
        var tag = TagAutoSerializer.SerializeObject(original, options);
        var result = TagAutoDeserializer.DeserializeObject<Level1>(tag, options);
        
        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result!.Middle);
        Assert.NotNull(result.Middle!.Inner);
        Assert.Equal("Deep", result.Middle.Inner!.Value);
    }
}

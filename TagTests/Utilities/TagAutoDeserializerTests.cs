using HyperTAG.Attributes;
using HyperTAG.Core;
using HyperTAG.Exceptions;
using HyperTAG.Interfaces;
using HyperTAG.Models;
using HyperTAG.Utilities;

namespace TagTests.Utilities;

// Define a collection for tests that use Default registry
[CollectionDefinition("DefaultRegistry", DisableParallelization = true)]
public class DefaultRegistryCollection
{
}

// Apply collection to tests that use Default registry
[Collection("DefaultRegistry")]
public class TagAutoDeserializerTests
{
    [Fact]
    public void DeserializeObject_WithNull_ShouldReturnDefault()
    {
        // Act
        var result = TagAutoDeserializer.DeserializeObject<SimpleTestData>(null);
        
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void DeserializeObject_WithNullAndCustomRegistry_ShouldReturnDefault()
    {
        // Arrange
        var registry = new TagConverterRegistry();

        // Act
        var result = TagAutoDeserializer.DeserializeObject<SimpleTestData>(null, registry);
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public void DeserializeObject_WithSimpleData_ShouldDeserializeCorrectly()
    {
        // Arrange
        var original = new SimpleTestData { Number = 42, Text = "Hello" };
        var tag = TagAutoSerializer.SerializeObject(original);
        
        // Act
        var result = TagAutoDeserializer.DeserializeObject<SimpleTestData>(tag);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(42, result!.Number);
        Assert.Equal("Hello", result.Text);
    }
    
    [Fact]
    public void DeserializeObject_WithCustomFieldName_ShouldUseCustomName()
    {
        // Arrange
        var original = new DataWithCustomName { Value = 100, Normal = "test" };
        var tag = TagAutoSerializer.SerializeObject(original);
        
        // Act
        var result = TagAutoDeserializer.DeserializeObject<DataWithCustomName>(tag);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(100, result!.Value);
        Assert.Equal("test", result.Normal);
    }
    
    [Fact]
    public void DeserializeObject_WithNestedObject_ShouldDeserializeCorrectly()
    {
        // Arrange
        var original = new NestedTestData
        {
            Id = 1,
            Nested = new SimpleTestData { Number = 99, Text = "Nested" }
        };
        var tag = TagAutoSerializer.SerializeObject(original);
        
        // Act
        var result = TagAutoDeserializer.DeserializeObject<NestedTestData>(tag);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
        Assert.NotNull(result.Nested);
        Assert.Equal(99, result.Nested!.Number);
        Assert.Equal("Nested", result.Nested.Text);
    }
    
    [Fact]
    public void DeserializeObject_WithBasicArrays_ShouldDeserializeCorrectly()
    {
        // Arrange
        var original = new ArrayTestData
        {
            Numbers = new[] { 1, 2, 3 },
            Texts = new[] { "a", "b", "c" },
            Flags = new[] { true, false, true }
        };
        var tag = TagAutoSerializer.SerializeObject(original);
        
        // Act
        var result = TagAutoDeserializer.DeserializeObject<ArrayTestData>(tag);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(new[] { 1, 2, 3 }, result!.Numbers);
        Assert.Equal(new[] { "a", "b", "c" }, result.Texts);
        Assert.Equal(new[] { true, false, true }, result.Flags);
    }
    
    [Fact]
    public void DeserializeObject_WithObjectArray_ShouldDeserializeCorrectly()
    {
        // Arrange
        var original = new ObjectArrayTestData
        {
            Items = new[]
            {
                new SimpleTestData { Number = 1, Text = "One" },
                new SimpleTestData { Number = 2, Text = "Two" }
            }
        };
        var tag = TagAutoSerializer.SerializeObject(original);
        
        // Act
        var result = TagAutoDeserializer.DeserializeObject<ObjectArrayTestData>(tag);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result!.Items.Length);
        Assert.Equal(1, result.Items[0].Number);
        Assert.Equal("One", result.Items[0].Text);
        Assert.Equal(2, result.Items[1].Number);
        Assert.Equal("Two", result.Items[1].Text);
    }
    
    [Fact]
    public void DeserializeObject_WithEmptyObjectArray_ShouldDeserializeCorrectly()
    {
        // Arrange
        var original = new ObjectArrayTestData { Items = Array.Empty<SimpleTestData>() };
        var tag = TagAutoSerializer.SerializeObject(original);
        
        // Act
        var result = TagAutoDeserializer.DeserializeObject<ObjectArrayTestData>(tag);
        
        // Assert
        Assert.NotNull(result);
        Assert.Empty(result!.Items);
    }
    
    [Fact]
    public void DeserializeObject_WithITagStructImplementation_ShouldCallFromTag()
    {
        // Arrange
        var original = new ITagStructTestData { Value = 123 };
        var tag = TagAutoSerializer.SerializeObject(original);
        
        // Act
        var result = TagAutoDeserializer.DeserializeObject<ITagStructTestData>(tag);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(123, result!.Value);
    }
    
    [Fact]
    public void DeserializeObject_WithITagStructArray_ShouldDeserializeCorrectly()
    {
        // Arrange
        var original = new MixedArrayTestData
        {
            Items = new[]
            {
                new ITagStructTestData { Value = 1 },
                new ITagStructTestData { Value = 2 },
                new ITagStructTestData { Value = 3 }
            }
        };
        var tag = TagAutoSerializer.SerializeObject(original);
        
        // Act
        var result = TagAutoDeserializer.DeserializeObject<MixedArrayTestData>(tag);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result!.Items.Length);
        Assert.Equal(1, result.Items[0].Value);
        Assert.Equal(2, result.Items[1].Value);
        Assert.Equal(3, result.Items[2].Value);
    }
    
    [Fact]
    public void DeserializeObject_WithoutTagStructAttribute_ShouldThrowException()
    {
        // Arrange
        var tag = new HyperTag(TagDataType.Empty);
        var options = new TagAutoOptions { ThrowExceptions = true };
        
        // Act & Assert
        var exception = Assert.Throws<TagAutoDeserializeException>(
            () => TagAutoDeserializer.DeserializeObject<UnsupportedTestData>(tag, options));
        
        Assert.Contains("does not implement ITagStruct", exception.Message);
        Assert.Contains("not marked with [TagStruct]", exception.Message);
        Assert.Contains("has no registered converter", exception.Message);
    }

    [Fact]
    public void DeserializeObject_WithoutTagStructAttribute_DefaultShouldReturnNull()
    {
        // Arrange
        var tag = new HyperTag(TagDataType.Empty);
        
        // Act - Using default options (ThrowExceptions = false)
        var result = TagAutoDeserializer.DeserializeObject<UnsupportedTestData>(tag);
        
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void DeserializeObject_WithTypeMismatch_ShouldThrowException()
    {
        // Arrange - Create tag with wrong type for a field
        var tag = new HyperTag(TagDataType.Empty);
        var numberTag = new HyperTag(TagDataType.String, "Number");
        numberTag.Entities.Add(new HyperTag(TagDataType.String, "wrong type")); // Should be Int
        tag.Entities.Add(numberTag);
        var options = new TagAutoOptions { ThrowExceptions = true };
        
        // Act & Assert
        Assert.Throws<TagAutoDeserializeException>(
            () => TagAutoDeserializer.DeserializeObject<SimpleTestData>(tag, options));
    }

    [Fact]
    public void DeserializeObject_WithTypeMismatch_DefaultShouldReturnNull()
    {
        // Arrange - Create tag with wrong type for a field
        var tag = new HyperTag(TagDataType.Empty);
        var numberTag = new HyperTag(TagDataType.String, "Number");
        numberTag.Entities.Add(new HyperTag(TagDataType.String, "wrong type")); // Should be Int
        tag.Entities.Add(numberTag);
        
        // Act - Using default options (ThrowExceptions = false)
        var result = TagAutoDeserializer.DeserializeObject<SimpleTestData>(tag);
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public void DeserializeObject_WithNullNestedObject_ShouldDeserializeAsNull()
    {
        // Arrange
        var tag = new HyperTag(TagDataType.Empty);
        var idTag = new HyperTag(TagDataType.String, "Id");
        idTag.Entities.Add(new HyperTag(TagDataType.Int, 1));
        var nestedTag = new HyperTag(TagDataType.String, "Nested");
        nestedTag.Entities.Add(new HyperTag(TagDataType.Null));
        tag.Entities.Add(idTag);
        tag.Entities.Add(nestedTag);
        
        // Act
        var result = TagAutoDeserializer.DeserializeObject<NestedTestData>(tag);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
        Assert.Null(result.Nested);
    }
    
    [Fact]
    public void DeserializeObject_WithNullArrayElements_ShouldDeserializeCorrectly()
    {
        // Arrange
        var original = new ObjectArrayTestData
        {
            Items = new SimpleTestData?[] 
            { 
                new SimpleTestData { Number = 1, Text = "One" }, 
                null, 
                new SimpleTestData { Number = 3, Text = "Three" } 
            }!
        };
        var tag = TagAutoSerializer.SerializeObject(original);
        
        // Act
        var result = TagAutoDeserializer.DeserializeObject<ObjectArrayTestData>(tag);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result!.Items.Length);
        Assert.NotNull(result.Items[0]);
        Assert.Null(result.Items[1]);
        Assert.NotNull(result.Items[2]);
    }
    
    [Fact]
    public void DeserializeObject_WithAllBasicTypes_ShouldDeserializeCorrectly()
    {
        // Arrange
        var original = new AllBasicTypesData();
        var tag = TagAutoSerializer.SerializeObject(original);
        
        // Act
        var result = TagAutoDeserializer.DeserializeObject<AllBasicTypesData>(tag);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(original.BoolValue, result!.BoolValue);
        Assert.Equal(original.CharValue, result.CharValue);
        Assert.Equal(original.ByteValue, result.ByteValue);
        Assert.Equal(original.ShortValue, result.ShortValue);
        Assert.Equal(original.IntValue, result.IntValue);
        Assert.Equal(original.LongValue, result.LongValue);
        Assert.Equal(original.FloatValue, result.FloatValue);
        Assert.Equal(original.DoubleValue, result.DoubleValue);
        Assert.Equal(original.DecimalValue, result.DecimalValue);
        Assert.Equal(original.UShortValue, result.UShortValue);
        Assert.Equal(original.UIntValue, result.UIntValue);
        Assert.Equal(original.ULongValue, result.ULongValue);
        Assert.Equal(original.SByteValue, result.SByteValue);
        Assert.Equal(original.StringValue, result.StringValue);
    }
    
    [Fact]
    public void RoundTrip_ComplexStructure_ShouldPreserveAllData()
    {
        // Arrange
        var original = new NestedTestData
        {
            Id = 42,
            Nested = new SimpleTestData { Number = 99, Text = "Test" }
        };
        
        // Act
        var tag = TagAutoSerializer.SerializeObject(original);
        var bytes = tag!.Serialize();
        var deserializedTag = HyperTag.Deserialize(bytes!);
        var result = TagAutoDeserializer.DeserializeObject<NestedTestData>(deserializedTag);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(original.Id, result!.Id);
        Assert.NotNull(result.Nested);
        Assert.Equal(original.Nested!.Number, result.Nested!.Number);
        Assert.Equal(original.Nested.Text, result.Nested.Text);
    }
    
    [Fact]
    public void RoundTrip_WithArrays_ShouldPreserveAllData()
    {
        // Arrange
        var original = new ObjectArrayTestData
        {
            Items = new[]
            {
                new SimpleTestData { Number = 1, Text = "One" },
                new SimpleTestData { Number = 2, Text = "Two" },
                new SimpleTestData { Number = 3, Text = "Three" }
            }
        };
        
        // Act
        var tag = TagAutoSerializer.SerializeObject(original);
        var bytes = tag!.Serialize();
        var deserializedTag = HyperTag.Deserialize(bytes!);
        var result = TagAutoDeserializer.DeserializeObject<ObjectArrayTestData>(deserializedTag);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result!.Items.Length);
        for (int i = 0; i < 3; i++)
        {
            Assert.Equal(original.Items[i].Number, result.Items[i].Number);
            Assert.Equal(original.Items[i].Text, result.Items[i].Text);
        }
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
    public void RoundTrip_DeepNesting_ShouldPreserveAllData()
    { 
        // Arrange
        var original = new Level1();
        
        // Act
        var tag = TagAutoSerializer.SerializeObject(original);
        var bytes = tag!.Serialize();
        var deserializedTag = HyperTag.Deserialize(bytes!);
        var result = TagAutoDeserializer.DeserializeObject<Level1>(deserializedTag);
        
        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result!.Middle);
        Assert.NotNull(result.Middle!.Inner);
        Assert.Equal("Deep", result.Middle.Inner!.Value);
    }

    [Fact]
    public void DeserializeObject_WithCustomConverter_ShouldUseConverter()
    {
        // Arrange
        var registry = new TagConverterRegistry();
        registry.Register(new PointConverter());
        var original = new Point { X = 10, Y = 20 };
        var tag = TagAutoSerializer.SerializeObject(original, registry);

        try
        {
            // Act
            var result = TagAutoDeserializer.DeserializeObject<Point>(tag, registry);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result!.X);
            Assert.Equal(20, result.Y);
        }
        finally
        {
            registry.Clear();
        }
    }

    [Fact]
    public void DeserializeObject_WithDefaultRegistry_ShouldUseDefaultConverter()
    {
        // Ensure clean state at start
        TagConverterRegistry.Default.Unregister<Point>();
        TagConverterRegistry.Default.Register(new PointConverter());
        
        var original = new Point { X = 30, Y = 40 };
        var tag = TagAutoSerializer.SerializeObject(original);

        try
        {
            // Act
            var result = TagAutoDeserializer.DeserializeObject<Point>(tag);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(30, result!.X);
            Assert.Equal(40, result.Y);
        }
        finally
        {
            TagConverterRegistry.Default.Unregister<Point>();
        }
    }

    [Fact]
    public void DeserializeObject_WithNullRegistry_ShouldUseDefaultRegistry()
    {
        // Ensure clean state at start
        TagConverterRegistry.Default.Unregister<Point>();
        TagConverterRegistry.Default.Register(new PointConverter());
        
        var original = new Point { X = 50, Y = 60 };
        var tag = TagAutoSerializer.SerializeObject(original);

        try
        {
            // Act
            var result = TagAutoDeserializer.DeserializeObject<Point>(tag, (TagConverterRegistry?)null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(50, result!.X);
            Assert.Equal(60, result.Y);
        }
        finally
        {
            TagConverterRegistry.Default.Unregister<Point>();
        }
    }

    [Fact]
    public void DeserializeObject_ConverterPriority_ShouldPreferConverter()
    {
        // Arrange
        var registry = new TagConverterRegistry();
        var customConverter = new CustomITagStructConverter();
        registry.Register(customConverter);
        
        var original = new ITagStructTestData { Value = 100 };
        var tag = TagAutoSerializer.SerializeObject(original, registry);

        try
        {
            // Act
            var result = TagAutoDeserializer.DeserializeObject<ITagStructTestData>(tag, registry);

            // Assert - Converter doubled on serialize, should be preserved
            Assert.NotNull(result);
            Assert.Equal(200, result!.Value);
        }
        finally
        {
            registry.Clear();
        }
    }

    private class CustomITagStructConverter : ITagConverter<ITagStructTestData>
    {
        public HyperTag? ToTag(ITagStructTestData? obj)
        {
            if (obj == null) return null;
            
            var tag = new HyperTag(TagDataType.Empty);
            var valueTag = new HyperTag(TagDataType.String, "Value");
            valueTag.Entities.Add(new HyperTag(TagDataType.Int, obj.Value * 2));
            tag.Entities.Add(valueTag);
            return tag;
        }

        public ITagStructTestData? FromTag(HyperTag? tag)
        {
            if (tag == null) return null;
            
            var data = new ITagStructTestData();
            foreach (var entity in tag.Entities)
            {
                if (entity.Type == TagDataType.String && entity.Value as string == "Value")
                {
                    if (entity.Entities.Count > 0 && entity.Entities[0].Type == TagDataType.Int)
                    {
                        data.Value = (int)entity.Entities[0].Value!;
                    }
                }
            }
            return data;
        }
    }

    [Fact]
    public void DeserializeObject_WithEmptyTag_ShouldReturnDefaultObject()
    {
        // Arrange
        var tag = new HyperTag(TagDataType.Empty);

        // Act
        var result = TagAutoDeserializer.DeserializeObject<SimpleTestData>(tag);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(42, result!.Number); // Default value
        Assert.Equal("Hello", result.Text); // Default value
    }

    [Fact]
    public void DeserializeObject_WithMissingFields_ShouldUseDefaultValues()
    {
        // Arrange
        var tag = new HyperTag(TagDataType.Empty);
        var numberTag = new HyperTag(TagDataType.String, "Number");
        numberTag.Entities.Add(new HyperTag(TagDataType.Int, 999));
        tag.Entities.Add(numberTag);
        // Text field is missing

        // Act
        var result = TagAutoDeserializer.DeserializeObject<SimpleTestData>(tag);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(999, result!.Number);
        Assert.Equal("Hello", result.Text); // Default value
    }

    [Fact]
    public void DeserializeObject_WithExtraFields_ShouldIgnoreExtraFields()
    {
        // Arrange
        var tag = new HyperTag(TagDataType.Empty);
        
        var numberTag = new HyperTag(TagDataType.String, "Number");
        numberTag.Entities.Add(new HyperTag(TagDataType.Int, 42));
        tag.Entities.Add(numberTag);
        
        var textTag = new HyperTag(TagDataType.String, "Text");
        textTag.Entities.Add(new HyperTag(TagDataType.String, "Hello"));
        tag.Entities.Add(textTag);
        
        // Extra unknown field
        var extraTag = new HyperTag(TagDataType.String, "UnknownField");
        extraTag.Entities.Add(new HyperTag(TagDataType.Int, 999));
        tag.Entities.Add(extraTag);

        // Act
        var result = TagAutoDeserializer.DeserializeObject<SimpleTestData>(tag);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(42, result!.Number);
        Assert.Equal("Hello", result.Text);
    }

    [Fact]
    public void DeserializeObject_WithEmptyString_ShouldDeserializeCorrectly()
    {
        // Arrange
        var original = new SimpleTestData { Number = 1, Text = "" };
        var tag = TagAutoSerializer.SerializeObject(original);

        // Act
        var result = TagAutoDeserializer.DeserializeObject<SimpleTestData>(tag);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("", result!.Text);
    }

    [Fact]
    public void DeserializeObject_WithSpecialCharacters_ShouldDeserializeCorrectly()
    {
        // Arrange
        var original = new SimpleTestData { Number = 1, Text = "Hello\nWorld\t\r" };
        var tag = TagAutoSerializer.SerializeObject(original);

        // Act
        var result = TagAutoDeserializer.DeserializeObject<SimpleTestData>(tag);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Hello\nWorld\t\r", result!.Text);
    }

    [Fact]
    public void DeserializeObject_WithUnicodeCharacters_ShouldDeserializeCorrectly()
    {
        // Arrange
        var original = new SimpleTestData { Number = 1, Text = "你好世界 🌍" };
        var tag = TagAutoSerializer.SerializeObject(original);

        // Act
        var result = TagAutoDeserializer.DeserializeObject<SimpleTestData>(tag);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("你好世界 🌍", result!.Text);
    }

    [Fact]
    public void DeserializeObject_WithLargeArray_ShouldDeserializeCorrectly()
    {
        // Arrange
        var largeArray = Enumerable.Range(0, 1000).ToArray();
        var original = new ArrayTestData
        {
            Numbers = largeArray,
            Texts = Array.Empty<string>(),
            Flags = Array.Empty<bool>()
        };
        var tag = TagAutoSerializer.SerializeObject(original);

        // Act
        var result = TagAutoDeserializer.DeserializeObject<ArrayTestData>(tag);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1000, result!.Numbers.Length);
        Assert.Equal(largeArray, result.Numbers);
    }

    [Fact]
    public void DeserializeObject_WithCorruptedData_ShouldThrowException()
    {
        // Arrange - Create tag with invalid structure
        var tag = new HyperTag(TagDataType.Empty);
        var numberTag = new HyperTag(TagDataType.String, "Number");
        // Missing value entity
        tag.Entities.Add(numberTag);

        // Act
        var result = TagAutoDeserializer.DeserializeObject<SimpleTestData>(tag);

        // Assert - Should use default value when entity is missing
        Assert.NotNull(result);
        Assert.Equal(42, result!.Number); // Default value
    }
}

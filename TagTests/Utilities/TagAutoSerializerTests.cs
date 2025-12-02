using HyperTAG.Attributes;
using HyperTAG.Core;
using HyperTAG.Exceptions;
using HyperTAG.Interfaces;
using HyperTAG.Models;
using HyperTAG.Utilities;

namespace TagTests.Utilities;

// ===== Test Helper Classes =====

[TagStruct]
public class SimpleTestData
{
    [TagData]
    public int Number = 42;
    
    [TagData]
    public string Text = "Hello";
}

[TagStruct]
public class DataWithCustomName
{
    [TagData("custom_name")]
    public int Value = 100;
    
    [TagData]
    public string Normal = "test";
}

[TagStruct]
public class NestedTestData
{
    [TagData]
    public int Id = 1;
    
    [TagData]
    public SimpleTestData? Nested = new();
}

[TagStruct]
public class ArrayTestData
{
    [TagData]
    public int[] Numbers = new[] { 1, 2, 3 };
    
    [TagData]
    public string[] Texts = new[] { "a", "b", "c" };
    
    [TagData]
    public bool[] Flags = new[] { true, false };
}

[TagStruct]
public class ObjectArrayTestData
{
    [TagData]
    public SimpleTestData[] Items = new[]
    {
        new SimpleTestData { Number = 1, Text = "One" },
        new SimpleTestData { Number = 2, Text = "Two" }
    };
}

public class ITagStructTestData : ITagStruct
{
    public int Value = 123;
    
    public HyperTag? ToTag()
    {
        var tag = new HyperTag(TagDataType.Empty);
        var valueTag = new HyperTag(TagDataType.String, "Value");
        valueTag.Entities.Add(new HyperTag(TagDataType.Int, Value));
        tag.Entities.Add(valueTag);
        return tag;
    }
    
    public void FromTag(HyperTag? tag)
    {
        if (tag == null) return;
        foreach (var entity in tag.Entities)
        {
            if (entity.Type == TagDataType.String && entity.Value as string == "Value")
            {
                if (entity.Entities.Count > 0 && entity.Entities[0].Type == TagDataType.Int)
                {
                    Value = (int)entity.Entities[0].Value!;
                }
            }
        }
    }
}

[TagStruct]
public class MixedArrayTestData
{
    [TagData]
    public ITagStructTestData[] Items = new[]
    {
        new ITagStructTestData { Value = 1 },
        new ITagStructTestData { Value = 2 }
    };
}

public class UnsupportedTestData
{
    [TagData]
    public int Value = 1;
}

[TagStruct]
public class AllBasicTypesData
{
    [TagData] public bool BoolValue = true;
    [TagData] public char CharValue = 'A';
    [TagData] public byte ByteValue = 255;
    [TagData] public short ShortValue = -32000;
    [TagData] public int IntValue = -2000000;
    [TagData] public long LongValue = -9000000000L;
    [TagData] public float FloatValue = 3.14f;
    [TagData] public double DoubleValue = 2.71828;
    [TagData] public decimal DecimalValue = 123.456m;
    [TagData] public ushort UShortValue = 65000;
    [TagData] public uint UIntValue = 4000000000u;
    [TagData] public ulong ULongValue = 18000000000000000000ul;
    [TagData] public sbyte SByteValue = -100;
    [TagData] public string StringValue = "test";
}

// Test converter for custom registry tests
public class Point
{
    public int X { get; set; }
    public int Y { get; set; }
}

public class PointConverter : ITagConverter<Point>
{
    public HyperTag? ToTag(Point? obj)
    {
        if (obj == null) return null;
        
        var tag = new HyperTag(TagDataType.Empty);
        var xTag = new HyperTag(TagDataType.String, "X");
        xTag.Entities.Add(new HyperTag(TagDataType.Int, obj.X));
        tag.Entities.Add(xTag);
        
        var yTag = new HyperTag(TagDataType.String, "Y");
        yTag.Entities.Add(new HyperTag(TagDataType.Int, obj.Y));
        tag.Entities.Add(yTag);
        
        return tag;
    }

    public Point? FromTag(HyperTag? tag)
    {
        if (tag == null) return null;
        
        var point = new Point();
        foreach (var entity in tag.Entities)
        {
            if (entity.Type != TagDataType.String || entity.Value == null)
                continue;
                
            var fieldName = (string)entity.Value;
            if (entity.Entities.Count == 0)
                continue;
                
            var valueTag = entity.Entities[0];
            
            switch (fieldName)
            {
                case "X":
                    if (valueTag.Type == TagDataType.Int)
                        point.X = (int)valueTag.Value!;
                    break;
                case "Y":
                    if (valueTag.Type == TagDataType.Int)
                        point.Y = (int)valueTag.Value!;
                    break;
            }
        }
        
        return point;
    }
}

// ===== Test Class =====

public class TagAutoSerializerTests
{
    [Fact]
    public void SerializeObject_WithNull_ShouldReturnNull()
    {
        // Act
        var result = TagAutoSerializer.SerializeObject(null);
        
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void SerializeObject_WithNullAndCustomRegistry_ShouldReturnNull()
    {
        // Arrange
        var registry = new TagConverterRegistry();

        // Act
        var result = TagAutoSerializer.SerializeObject(null, registry);
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public void SerializeObject_WithSimpleData_ShouldSerializeCorrectly()
    {
        // Arrange
        var data = new SimpleTestData { Number = 42, Text = "Hello" };
        
        // Act
        var tag = TagAutoSerializer.SerializeObject(data);
        
        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Empty, tag!.Type);
        Assert.Equal(2, tag.Entities.Count);
        
        // Check Number field
        var numberTag = tag.Entities[0];
        Assert.Equal(TagDataType.String, numberTag.Type);
        Assert.Equal("Number", numberTag.Value);
        Assert.Single(numberTag.Entities);
        Assert.Equal(TagDataType.Int, numberTag.Entities[0].Type);
        Assert.Equal(42, numberTag.Entities[0].Value);
        
        // Check Text field
        var textTag = tag.Entities[1];
        Assert.Equal(TagDataType.String, textTag.Type);
        Assert.Equal("Text", textTag.Value);
        Assert.Single(textTag.Entities);
        Assert.Equal(TagDataType.String, textTag.Entities[0].Type);
        Assert.Equal("Hello", textTag.Entities[0].Value);
    }
    
    [Fact]
    public void SerializeObject_WithCustomFieldName_ShouldUseCustomName()
    {
        // Arrange
        var data = new DataWithCustomName { Value = 100, Normal = "test" };
        
        // Act
        var tag = TagAutoSerializer.SerializeObject(data);
        
        // Assert
        Assert.NotNull(tag);
        Assert.Equal(2, tag!.Entities.Count);
        Assert.Equal("custom_name", tag.Entities[0].Value);
        Assert.Equal("Normal", tag.Entities[1].Value);
    }
    
    [Fact]
    public void SerializeObject_WithNestedObject_ShouldSerializeAsData()
    {
        // Arrange
        var data = new NestedTestData
        {
            Id = 1,
            Nested = new SimpleTestData { Number = 99, Text = "Nested" }
        };
        // Use NestedTags = false to get Data format
        var options = new TagAutoOptions { NestedTags = false };
        
        // Act
        var tag = TagAutoSerializer.SerializeObject(data, options);
        
        // Assert
        Assert.NotNull(tag);
        Assert.Equal(2, tag!.Entities.Count);
        
        // Check Nested field
        var nestedTag = tag.Entities[1];
        Assert.Equal("Nested", nestedTag.Value);
        Assert.Single(nestedTag.Entities);
        Assert.Equal(TagDataType.Data, nestedTag.Entities[0].Type);
        Assert.IsType<byte[]>(nestedTag.Entities[0].Value);
    }

    [Fact]
    public void SerializeObject_WithNestedObject_DefaultShouldEmbedDirectly()
    {
        // Arrange
        var data = new NestedTestData
        {
            Id = 1,
            Nested = new SimpleTestData { Number = 99, Text = "Nested" }
        };
        
        // Act - Using default options (NestedTags = true)
        var tag = TagAutoSerializer.SerializeObject(data);
        
        // Assert
        Assert.NotNull(tag);
        Assert.Equal(2, tag!.Entities.Count);
        
        // Check Nested field - should be directly embedded
        var nestedTag = tag.Entities[1];
        Assert.Equal("Nested", nestedTag.Value);
        Assert.Single(nestedTag.Entities);
        Assert.Equal(TagDataType.Empty, nestedTag.Entities[0].Type);
        Assert.Equal(2, nestedTag.Entities[0].Entities.Count);
    }

    [Fact]
    public void SerializeObject_WithObjectArray_ShouldSerializeAsMultipleDataTags()
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
        // Use NestedTags = false to get Data format
        var options = new TagAutoOptions { NestedTags = false };
        
        // Act
        var tag = TagAutoSerializer.SerializeObject(data, options);
        
        // Assert
        Assert.NotNull(tag);
        Assert.Single(tag!.Entities);
        
        var itemsTag = tag.Entities[0];
        Assert.Equal("Items", itemsTag.Value);
        Assert.Single(itemsTag.Entities);
        
        var containerTag = itemsTag.Entities[0];
        Assert.Equal(TagDataType.Empty, containerTag.Type);
        Assert.Equal(2, containerTag.Entities.Count);
        
        // Each item should be serialized as Data
        Assert.Equal(TagDataType.Data, containerTag.Entities[0].Type);
        Assert.Equal(TagDataType.Data, containerTag.Entities[1].Type);
    }

    [Fact]
    public void SerializeObject_WithObjectArray_DefaultShouldEmbedDirectly()
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
        
        // Act - Using default options (NestedTags = true)
        var tag = TagAutoSerializer.SerializeObject(data);
        
        // Assert
        Assert.NotNull(tag);
        Assert.Single(tag!.Entities);
        
        var itemsTag = tag.Entities[0];
        Assert.Equal("Items", itemsTag.Value);
        Assert.Single(itemsTag.Entities);
        
        var containerTag = itemsTag.Entities[0];
        Assert.Equal(TagDataType.Empty, containerTag.Type);
        Assert.Equal(2, containerTag.Entities.Count);
        
        // Each item should be directly embedded
        Assert.Equal(TagDataType.Empty, containerTag.Entities[0].Type);
        Assert.Equal(TagDataType.Empty, containerTag.Entities[1].Type);
    }

    [Fact]
    public void SerializeObject_WithoutTagStructAttribute_ShouldThrowException()
    {
        // Arrange
        var data = new UnsupportedTestData { Value = 1 };
        var options = new TagAutoOptions { ThrowExceptions = true };
        
        // Act & Assert
        var exception = Assert.Throws<TagAutoSerializeException>(
            () => TagAutoSerializer.SerializeObject(data, options));
        
        Assert.Contains("does not implement ITagStruct", exception.Message);
        Assert.Contains("not marked with [TagStruct]", exception.Message);
        Assert.Contains("has no registered converter", exception.Message);
    }

    [Fact]
    public void SerializeObject_WithoutTagStructAttribute_DefaultShouldReturnNull()
    {
        // Arrange
        var data = new UnsupportedTestData { Value = 1 };
        
        // Act - Using default options (ThrowExceptions = false)
        var result = TagAutoSerializer.SerializeObject(data);
        
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void SerializeObject_WithNullArrayElements_ShouldSerializeAsNullTags()
    {
        // Arrange
        var data = new ObjectArrayTestData
        {
            Items = new SimpleTestData?[] { new SimpleTestData(), null, new SimpleTestData() }!
        };
        // Use NestedTags = false to get Data format
        var options = new TagAutoOptions { NestedTags = false };
        
        // Act
        var tag = TagAutoSerializer.SerializeObject(data, options);
        
        // Assert
        var containerTag = tag!.Entities[0].Entities[0];
        Assert.Equal(3, containerTag.Entities.Count);
        Assert.Equal(TagDataType.Data, containerTag.Entities[0].Type);
        Assert.Equal(TagDataType.Null, containerTag.Entities[1].Type);
        Assert.Equal(TagDataType.Data, containerTag.Entities[2].Type);
    }

    [Fact]
    public void SerializeObject_WithNullArrayElements_DefaultShouldEmbedDirectly()
    {
        // Arrange
        var data = new ObjectArrayTestData
        {
            Items = new SimpleTestData?[] { new SimpleTestData(), null, new SimpleTestData() }!
        };
        
        // Act - Using default options (NestedTags = true)
        var tag = TagAutoSerializer.SerializeObject(data);
        
        // Assert
        var containerTag = tag!.Entities[0].Entities[0];
        Assert.Equal(3, containerTag.Entities.Count);
        Assert.Equal(TagDataType.Empty, containerTag.Entities[0].Type);
        Assert.Equal(TagDataType.Null, containerTag.Entities[1].Type);
        Assert.Equal(TagDataType.Empty, containerTag.Entities[2].Type);
    }
    
    [Fact]
    public void SerializeObject_WithBasicArrays_ShouldSerializeCorrectly()
    {
        // Arrange
        var data = new ArrayTestData
        {
            Numbers = new[] { 1, 2, 3 },
            Texts = new[] { "a", "b", "c" },
            Flags = new[] { true, false, true }
        };
        
        // Act
        var tag = TagAutoSerializer.SerializeObject(data);
        
        // Assert
        Assert.NotNull(tag);
        Assert.Equal(3, tag!.Entities.Count);
        
        // Check Numbers array
        var numbersTag = tag.Entities[0];
        Assert.Equal("Numbers", numbersTag.Value);
        Assert.Single(numbersTag.Entities);
        Assert.Equal(TagDataType.IntArray, numbersTag.Entities[0].Type);
        Assert.Equal(new[] { 1, 2, 3 }, numbersTag.Entities[0].Value);
        
        // Check Texts array
        var textsTag = tag.Entities[1];
        Assert.Equal("Texts", textsTag.Value);
        Assert.Single(textsTag.Entities);
        Assert.Equal(TagDataType.StringArray, textsTag.Entities[0].Type);
        Assert.Equal(new[] { "a", "b", "c" }, textsTag.Entities[0].Value);
    }
    
    [Fact]
    public void SerializeObject_WithEmptyObjectArray_ShouldSerializeCorrectly()
    {
        // Arrange
        var data = new ObjectArrayTestData { Items = Array.Empty<SimpleTestData>() };
        
        // Act
        var tag = TagAutoSerializer.SerializeObject(data);
        
        // Assert
        var containerTag = tag!.Entities[0].Entities[0];
        Assert.Equal(TagDataType.Empty, containerTag.Type);
        Assert.Empty(containerTag.Entities);
    }
    
    [Fact]
    public void SerializeObject_WithITagStructImplementation_ShouldCallToTag()
    {
        // Arrange
        var data = new ITagStructTestData { Value = 123 };
        
        // Act
        var tag = TagAutoSerializer.SerializeObject(data);
        
        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Empty, tag!.Type);
        Assert.Single(tag.Entities);
        Assert.Equal("Value", tag.Entities[0].Value);
    }
    
    [Fact]
    public void SerializeObject_WithITagStructArray_ShouldSerializeCorrectly()
    {
        // Arrange
        var data = new MixedArrayTestData
        {
            Items = new[]
            {
                new ITagStructTestData { Value = 1 },
                new ITagStructTestData { Value = 2 }
            }
        };
        
        // Act
        var tag = TagAutoSerializer.SerializeObject(data);
        
        // Assert
        Assert.NotNull(tag);
        var containerTag = tag!.Entities[0].Entities[0];
        Assert.Equal(TagDataType.Empty, containerTag.Type);
        Assert.Equal(2, containerTag.Entities.Count);
    }
    
    [Fact]
    public void SerializeObject_WithNullNestedObject_ShouldSerializeAsNull()
    {
        // Arrange
        var data = new NestedTestData
        {
            Id = 1,
            Nested = null
        };
        
        // Act
        var tag = TagAutoSerializer.SerializeObject(data);
        
        // Assert
        Assert.NotNull(tag);
        var nestedTag = tag!.Entities[1].Entities[0];
        Assert.Equal(TagDataType.Null, nestedTag.Type);
    }
    
    [Fact]
    public void SerializeObject_WithAllBasicTypes_ShouldSerializeCorrectly()
    {
        // Arrange
        var data = new AllBasicTypesData();
        
        // Act
        var tag = TagAutoSerializer.SerializeObject(data);
        
        // Assert
        Assert.NotNull(tag);
        Assert.Equal(14, tag!.Entities.Count);
    }
    
    [Fact]
    public void SerializeObject_CanSerializeThenDeserializeWithHyperTag()
    {
        // Arrange
        var data = new SimpleTestData { Number = 999, Text = "RoundTrip" };
        
        // Act
        var tag = TagAutoSerializer.SerializeObject(data);
        var bytes = tag!.Serialize();
        var deserializedTag = HyperTag.Deserialize(bytes!);
        
        // Assert
        Assert.NotNull(deserializedTag);
        Assert.Equal(TagDataType.Empty, deserializedTag!.Type);
        Assert.Equal(2, deserializedTag.Entities.Count);
    }

    [Fact]
    public void SerializeObject_WithCustomConverter_ShouldUseConverter()
    {
        // Arrange
        var registry = new TagConverterRegistry();
        registry.Register(new PointConverter());
        var point = new Point { X = 10, Y = 20 };

        try
        {
            // Act
            var tag = TagAutoSerializer.SerializeObject(point, registry);

            // Assert
            Assert.NotNull(tag);
            Assert.Equal(TagDataType.Empty, tag!.Type);
            Assert.Equal(2, tag.Entities.Count);
            
            var xTag = tag.Entities[0];
            Assert.Equal("X", xTag.Value);
            Assert.Equal(10, xTag.Entities[0].Value);
            
            var yTag = tag.Entities[1];
            Assert.Equal("Y", yTag.Value);
            Assert.Equal(20, yTag.Entities[0].Value);
        }
        finally
        {
            registry.Clear();
        }
    }

    [Fact]
    public void SerializeObject_WithDefaultRegistry_ShouldUseDefaultConverter()
    {
        // Arrange
        TagConverterRegistry.Default.Register(new PointConverter());
        var point = new Point { X = 30, Y = 40 };

        try
        {
            // Act
            var tag = TagAutoSerializer.SerializeObject(point);

            // Assert
            Assert.NotNull(tag);
            Assert.Equal(2, tag!.Entities.Count);
        }
        finally
        {
            TagConverterRegistry.Default.Unregister<Point>();
        }
    }

    [Fact]
    public void SerializeObject_WithNullRegistry_ShouldUseDefaultRegistry()
    {
        // Arrange
        TagConverterRegistry.Default.Register(new PointConverter());
        var point = new Point { X = 50, Y = 60 };

        try
        {
            // Act
            var tag = TagAutoSerializer.SerializeObject(point, (TagConverterRegistry?)null);

            // Assert
            Assert.NotNull(tag);
        }
        finally
        {
            TagConverterRegistry.Default.Unregister<Point>();
        }
    }

    [Fact]
    public void SerializeObject_ConverterPriority_ShouldPreferConverter()
    {
        // Arrange
        var registry = new TagConverterRegistry();
        var customConverter = new CustomITagStructConverter();
        registry.Register(customConverter);
        
        var data = new ITagStructTestData { Value = 100 };

        try
        {
            // Act
            var tag = TagAutoSerializer.SerializeObject(data, registry);

            // Assert - Converter should double the value
            Assert.NotNull(tag);
            var valueTag = tag!.Entities[0].Entities[0];
            Assert.Equal(200, valueTag.Value); // 100 * 2
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
            throw new NotImplementedException();
        }
    }

    [Fact]
    public void SerializeObject_WithEmptyString_ShouldSerializeCorrectly()
    {
        // Arrange
        var data = new SimpleTestData { Number = 1, Text = "" };

        // Act
        var tag = TagAutoSerializer.SerializeObject(data);

        // Assert
        Assert.NotNull(tag);
        var textTag = tag!.Entities[1].Entities[0];
        Assert.Equal(TagDataType.String, textTag.Type);
        Assert.Equal("", textTag.Value);
    }

    [Fact]
    public void SerializeObject_WithSpecialCharacters_ShouldSerializeCorrectly()
    {
        // Arrange
        var data = new SimpleTestData { Number = 1, Text = "Hello\nWorld\t\r" };

        // Act
        var tag = TagAutoSerializer.SerializeObject(data);

        // Assert
        Assert.NotNull(tag);
        var textTag = tag!.Entities[1].Entities[0];
        Assert.Equal("Hello\nWorld\t\r", textTag.Value);
    }

    [Fact]
    public void SerializeObject_WithUnicodeCharacters_ShouldSerializeCorrectly()
    {
        // Arrange
        var data = new SimpleTestData { Number = 1, Text = "你好世界 🌍" };

        // Act
        var tag = TagAutoSerializer.SerializeObject(data);

        // Assert
        Assert.NotNull(tag);
        var textTag = tag!.Entities[1].Entities[0];
        Assert.Equal("你好世界 🌍", textTag.Value);
    }

    [Fact]
    public void SerializeObject_WithLargeArray_ShouldSerializeCorrectly()
    {
        // Arrange
        var largeArray = Enumerable.Range(0, 1000).ToArray();
        var data = new ArrayTestData
        {
            Numbers = largeArray,
            Texts = Array.Empty<string>(),
            Flags = Array.Empty<bool>()
        };

        // Act
        var tag = TagAutoSerializer.SerializeObject(data);

        // Assert
        Assert.NotNull(tag);
        var numbersTag = tag!.Entities[0].Entities[0];
        Assert.Equal(TagDataType.IntArray, numbersTag.Type);
        var resultArray = (int[])numbersTag.Value!;
        Assert.Equal(1000, resultArray.Length);
        Assert.Equal(largeArray, resultArray);
    }
}

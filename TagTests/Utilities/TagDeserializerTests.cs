using HyperTAG.Exceptions;
using HyperTAG.Models;
using HyperTAG.Structs;
using HyperTAG.Utilities;

namespace TagTests.Utilities;

public class TagDeserializerTests
{
    [Fact]
    public void Deserialize_ValidData_ShouldReturnTagStruct()
    {
        // Arrange
        var original = new TagStruct(TagDataType.String, "test data");
        var serialized = TagSerializer.Serialize(original)!;

        // Act
        var result = TagDeserializer.Deserialize(serialized);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(original.Type, result!.Type);
        Assert.Equal(original.Value, result.Value);
    }

    [Fact]
    public void Deserialize_WithCustomMaxRecursionDepth_ShouldWork()
    {
        // Arrange
        var tagStruct = CreateNestedTagStruct(5);
        var serialized = TagSerializer.Serialize(tagStruct)!;

        // Act
        var result = TagDeserializer.Deserialize(serialized, maxRecursionDepth: 10);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void Deserialize_ExceedingMaxRecursionDepth_ShouldThrowExceptionWhenRequested()
    {
        // Arrange
        var tagStruct = CreateNestedTagStruct(10);
        var serialized = TagSerializer.Serialize(tagStruct)!;

        // Act & Assert
        Assert.Throws<TagRecursionException>(() => 
            TagDeserializer.Deserialize(serialized, maxRecursionDepth: 5, throwExceptions: true));
    }

    [Fact]
    public void Deserialize_ExceedingMaxRecursionDepth_ShouldReturnNullWhenNotThrowing()
    {
        // Arrange
        var tagStruct = CreateNestedTagStruct(10);
        var serialized = TagSerializer.Serialize(tagStruct)!;

        // Act
        var result = TagDeserializer.Deserialize(serialized, maxRecursionDepth: 5, throwExceptions: false);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Deserialize_InvalidMagicString_ShouldThrowException()
    {
        // Arrange
        var invalidData = CreateInvalidDataWithWrongMagic();

        // Act & Assert
        Assert.Throws<TagDeserializeException>(() => 
            TagDeserializer.Deserialize(invalidData, throwExceptions: true));
    }

    [Fact]
    public void Deserialize_InvalidTypeString_ShouldThrowException()
    {
        // Arrange
        var invalidData = CreateInvalidDataWithWrongType();

        // Act & Assert
        Assert.Throws<TagDeserializeException>(() => 
            TagDeserializer.Deserialize(invalidData, throwExceptions: true));
    }

    [Fact]
    public void Deserialize_InvalidData_ShouldReturnNullWhenNotThrowing()
    {
        // Arrange
        var invalidData = new byte[] { 0, 1, 2, 3, 4, 5 };

        // Act
        var result = TagDeserializer.Deserialize(invalidData, throwExceptions: false);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Deserialize_AllSupportedSingleDataTypes_ShouldReconstructCorrectly()
    {
        // Arrange
        var testCases = new[]
        {
            (TagDataType.Data, (object)new byte[] { 1, 2, 3, 4, 5 }),
            (TagDataType.Bool, (object)true),
            (TagDataType.Char, (object)'A'),
            (TagDataType.Byte, (object)(byte)42),
            (TagDataType.Short, (object)(short)1000),
            (TagDataType.Int, (object)100000),
            (TagDataType.Long, (object)10000000000L),
            (TagDataType.Float, (object)3.14f),
            (TagDataType.Double, (object)3.14159),
            (TagDataType.String, (object)"test string"),
            (TagDataType.Decimal, (object)123.456m),
            (TagDataType.UShort, (object)(ushort)50000),
            (TagDataType.UInt, (object)3000000000u),
            (TagDataType.ULong, (object)10000000000000000000ul),
            (TagDataType.SByte, (object)(sbyte)-50)
        };

        foreach (var (dataType, value) in testCases)
        {
            // Act
            var original = new TagStruct(dataType, value);
            var serialized = TagSerializer.Serialize(original)!;
            var deserialized = TagDeserializer.Deserialize(serialized);

            // Assert
            Assert.NotNull(deserialized);
            Assert.Equal(dataType, deserialized!.Type);
            Assert.Equal(value, deserialized.Value);
        }
    }

    [Fact]
    public void Deserialize_AllSupportedArrayDataTypes_ShouldReconstructCorrectly()
    {
        // Arrange
        var testCases = new[]
        {
            (TagDataType.BoolArray, (object)new[] { true, false, true }),
            (TagDataType.CharArray, (object)new[] { 'a', 'b', 'c' }),
            (TagDataType.ByteArray, (object)new byte[] { 1, 2, 3, 4, 5 }),
            (TagDataType.ShortArray, (object)new short[] { 1, 2, 3 }),
            (TagDataType.IntArray, (object)new[] { 1, 2, 3 }),
            (TagDataType.LongArray, (object)new long[] { 1L, 2L, 3L }),
            (TagDataType.FloatArray, (object)new[] { 1.1f, 2.2f, 3.3f }),
            (TagDataType.DoubleArray, (object)new[] { 1.1, 2.2, 3.3 }),
            (TagDataType.StringArray, (object)new[] { "hello", "world" }),
            (TagDataType.DecimalArray, (object)new[] { 1.1m, 2.2m, 3.3m }),
            (TagDataType.UShortArray, (object)new ushort[] { 1, 2, 3 }),
            (TagDataType.UIntArray, (object)new uint[] { 1u, 2u, 3u }),
            (TagDataType.ULongArray, (object)new ulong[] { 1ul, 2ul, 3ul }),
            (TagDataType.SByteArray, (object)new sbyte[] { 1, 2, 3 })
        };

        foreach (var (dataType, value) in testCases)
        {
            // Act
            var original = new TagStruct(dataType, value);
            var serialized = TagSerializer.Serialize(original)!;
            var deserialized = TagDeserializer.Deserialize(serialized);

            // Assert
            Assert.NotNull(deserialized);
            Assert.Equal(dataType, deserialized!.Type);
            Assert.Equal(value, deserialized.Value);
        }
    }

    [Fact]
    public void Deserialize_EmptyArrays_ShouldReconstructCorrectly()
    {
        // Arrange
        var testCases = new[]
        {
            (TagDataType.BoolArray, (object)new bool[0]),
            (TagDataType.IntArray, (object)new int[0]),
            (TagDataType.StringArray, (object)new string[0])
        };

        foreach (var (dataType, value) in testCases)
        {
            // Act
            var original = new TagStruct(dataType, value);
            var serialized = TagSerializer.Serialize(original)!;
            var deserialized = TagDeserializer.Deserialize(serialized);

            // Assert
            Assert.NotNull(deserialized);
            Assert.Equal(dataType, deserialized!.Type);
            Assert.Equal(value, deserialized.Value);
        }
    }

    [Fact]
    public void Deserialize_LargeArrays_ShouldReconstructCorrectly()
    {
        // Arrange
        var largeArray = Enumerable.Range(0, 1000).ToArray();
        var original = new TagStruct(TagDataType.IntArray, largeArray);
        var serialized = TagSerializer.Serialize(original)!;

        // Act
        var deserialized = TagDeserializer.Deserialize(serialized);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(TagDataType.IntArray, deserialized!.Type);
        Assert.Equal(largeArray, deserialized.Value);
    }

    [Fact]
    public void Deserialize_LargeData_ShouldReconstructCorrectly()
    {
        // Arrange
        var largeData = new byte[10000];
        for (int i = 0; i < largeData.Length; i++)
        {
            largeData[i] = (byte)(i % 256);
        }
        var original = new TagStruct(TagDataType.Data, largeData);
        var serialized = TagSerializer.Serialize(original)!;

        // Act
        var deserialized = TagDeserializer.Deserialize(serialized);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(TagDataType.Data, deserialized!.Type);
        Assert.Equal(largeData, deserialized.Value);
    }

    [Fact]
    public void Deserialize_NestedStructures_ShouldReconstructCorrectly()
    {
        // Arrange
        var root = new TagStruct(TagDataType.String, "root");
        var child1 = new TagStruct(TagDataType.Int, 1);
        var child2 = new TagStruct(TagDataType.Bool, true);
        var grandchild = new TagStruct(TagDataType.Double, 2.5);
        
        child1.Entities.Add(grandchild);
        root.Entities.Add(child1);
        root.Entities.Add(child2);

        var serialized = TagSerializer.Serialize(root)!;

        // Act
        var deserialized = TagDeserializer.Deserialize(serialized);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal("root", deserialized!.Value);
        Assert.Equal(2, deserialized.Entities.Count);
        
        Assert.Equal(TagDataType.Int, deserialized.Entities[0].Type);
        Assert.Equal(1, deserialized.Entities[0].Value);
        Assert.Single(deserialized.Entities[0].Entities);
        Assert.Equal(2.5, deserialized.Entities[0].Entities[0].Value);
        
        Assert.Equal(TagDataType.Bool, deserialized.Entities[1].Type);
        Assert.True((bool)deserialized.Entities[1].Value!);
    }

    [Fact]
    public void Deserialize_ComplexArrayStructures_ShouldReconstructCorrectly()
    {
        // Arrange
        var root = new TagStruct(TagDataType.String, "root");
        
        var numbers = new TagStruct(TagDataType.IntArray, new[] { 1, 2, 3 });
        var flags = new TagStruct(TagDataType.BoolArray, new[] { true, false, true });
        
        var nested = new TagStruct(TagDataType.DoubleArray, new[] { 1.1, 2.2, 3.3 });
        nested.Entities.Add(numbers);
        
        root.Entities.Add(nested);
        root.Entities.Add(flags);

        var serialized = TagSerializer.Serialize(root)!;

        // Act
        var deserialized = TagDeserializer.Deserialize(serialized);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal("root", deserialized!.Value);
        Assert.Equal(2, deserialized.Entities.Count);
        
        // Check nested array structure
        Assert.Equal(TagDataType.DoubleArray, deserialized.Entities[0].Type);
        Assert.Equal(new[] { 1.1, 2.2, 3.3 }, deserialized.Entities[0].Value);
        Assert.Single(deserialized.Entities[0].Entities);
        Assert.Equal(new[] { 1, 2, 3 }, deserialized.Entities[0].Entities[0].Value);
        
        // Check other array
        Assert.Equal(TagDataType.BoolArray, deserialized.Entities[1].Type);
        Assert.Equal(new[] { true, false, true }, deserialized.Entities[1].Value);
    }

    [Fact]
    public void Deserialize_DataAndByteArray_ShouldReconstructCorrectly()
    {
        // Arrange
        var dataBytes = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };
        var byteArrayBytes = new byte[] { 0x10, 0x20, 0x30, 0x40, 0x50 };
        
        var dataTag = new TagStruct(TagDataType.Data, dataBytes);
        var byteArrayTag = new TagStruct(TagDataType.ByteArray, byteArrayBytes);

        var dataSerialized = TagSerializer.Serialize(dataTag)!;
        var byteArraySerialized = TagSerializer.Serialize(byteArrayTag)!;

        // Act
        var dataDeserialized = TagDeserializer.Deserialize(dataSerialized);
        var byteArrayDeserialized = TagDeserializer.Deserialize(byteArraySerialized);

        // Assert
        Assert.NotNull(dataDeserialized);
        Assert.NotNull(byteArrayDeserialized);
        Assert.Equal(TagDataType.Data, dataDeserialized!.Type);
        Assert.Equal(TagDataType.ByteArray, byteArrayDeserialized!.Type);
        Assert.Equal(dataBytes, dataDeserialized.Value);
        Assert.Equal(byteArrayBytes, byteArrayDeserialized.Value);
    }

    [Fact]
    public void RoundTrip_ShouldPreserveAllData()
    {
        // Arrange
        var original = new TagStruct(TagDataType.StringArray, new[] { "test1", "test2" });
        var child = new TagStruct(TagDataType.IntArray, new[] { 10, 20, 30 });
        original.Entities.Add(child);

        // Act
        var serialized = TagSerializer.Serialize(original)!;
        var deserialized = TagDeserializer.Deserialize(serialized);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(original.Type, deserialized!.Type);
        Assert.Equal(original.Value, deserialized.Value);
        Assert.Equal(original.Entities.Count, deserialized.Entities.Count);
        Assert.Equal(original.Entities[0].Type, deserialized.Entities[0].Type);
        Assert.Equal(original.Entities[0].Value, deserialized.Entities[0].Value);
    }

    private TagStruct CreateNestedTagStruct(int depth)
    {
        if (depth <= 0)
            return new TagStruct(TagDataType.String, "leaf");

        var tag = new TagStruct(TagDataType.Int, depth);
        tag.Entities.Add(CreateNestedTagStruct(depth - 1));
        return tag;
    }

    private byte[] CreateInvalidDataWithWrongMagic()
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        
        writer.Write("WRONG_MAGIC");
        writer.Write("%TAG");
        writer.Write((long)10);
        writer.Write(new byte[10]);
        
        return stream.ToArray();
    }

    private byte[] CreateInvalidDataWithWrongType()
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        
        writer.Write("@HYPER");
        writer.Write("WRONG_TYPE");
        writer.Write((long)10);
        writer.Write(new byte[10]);
        
        return stream.ToArray();
    }
}

using HyperTAG.Exceptions;
using HyperTAG.Models;
using HyperTAG.Structs;
using HyperTAG.Utilities;

namespace TagTests.Utilities;

public class TagSerializerTests
{
    [Fact]
    public void Serialize_ValidTagStruct_ShouldReturnByteArray()
    {
        // Arrange
        var tagStruct = new TagStruct(TagDataType.String, "test value");

        // Act
        var result = TagSerializer.Serialize(tagStruct);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void Serialize_WithCustomMaxRecursionDepth_ShouldWork()
    {
        // Arrange
        var tagStruct = CreateNestedTagStruct(5); // 5 levels deep

        // Act
        var result = TagSerializer.Serialize(tagStruct, maxRecursionDepth: 10);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void Serialize_ExceedingMaxRecursionDepth_ShouldThrowExceptionWhenRequested()
    {
        // Arrange
        var tagStruct = CreateNestedTagStruct(10); // 10 levels deep

        // Act & Assert
        Assert.Throws<TagRecursionException>(() => 
            TagSerializer.Serialize(tagStruct, maxRecursionDepth: 5, throwExceptions: true));
    }

    [Fact]
    public void Serialize_ExceedingMaxRecursionDepth_ShouldReturnNullWhenNotThrowing()
    {
        // Arrange
        var tagStruct = CreateNestedTagStruct(10); // 10 levels deep

        // Act
        var result = TagSerializer.Serialize(tagStruct, maxRecursionDepth: 5, throwExceptions: false);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Serialize_AllSupportedSingleDataTypes_ShouldWork()
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
            var tagStruct = new TagStruct(dataType, value);
            var result = TagSerializer.Serialize(tagStruct);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
    }

    [Fact]
    public void Serialize_AllSupportedArrayDataTypes_ShouldWork()
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
            var tagStruct = new TagStruct(dataType, value);
            var result = TagSerializer.Serialize(tagStruct);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
    }

    [Fact]
    public void Serialize_EmptyArrays_ShouldWork()
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
            var tagStruct = new TagStruct(dataType, value);
            var result = TagSerializer.Serialize(tagStruct);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
    }

    [Fact]
    public void Serialize_LargeArrays_ShouldWork()
    {
        // Arrange
        var largeArray = Enumerable.Range(0, 1000).ToArray();
        var tagStruct = new TagStruct(TagDataType.IntArray, largeArray);

        // Act
        var result = TagSerializer.Serialize(tagStruct);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void Serialize_LargeData_ShouldWork()
    {
        // Arrange
        var largeData = new byte[10000];
        for (int i = 0; i < largeData.Length; i++)
        {
            largeData[i] = (byte)(i % 256);
        }
        var tagStruct = new TagStruct(TagDataType.Data, largeData);

        // Act
        var result = TagSerializer.Serialize(tagStruct);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void Serialize_NullValue_ShouldWork()
    {
        // Arrange
        var tagStruct = new TagStruct(TagDataType.String, null);

        // Act
        var result = TagSerializer.Serialize(tagStruct);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void Serialize_EmptyAndNullTypes_ShouldWork()
    {
        // Arrange
        var emptyTag = new TagStruct(TagDataType.Empty, null);
        var nullTag = new TagStruct(TagDataType.Null, null);

        // Act
        var emptyResult = TagSerializer.Serialize(emptyTag);
        var nullResult = TagSerializer.Serialize(nullTag);

        // Assert
        Assert.NotNull(emptyResult);
        Assert.NotNull(nullResult);
        Assert.NotEmpty(emptyResult);
        Assert.NotEmpty(nullResult);
    }

    [Fact]
    public void Serialize_ComplexNestedStructure_ShouldWork()
    {
        // Arrange
        var root = new TagStruct(TagDataType.String, "root");
        
        var numbers = new TagStruct(TagDataType.IntArray, new[] { 1, 2, 3 });
        var flags = new TagStruct(TagDataType.BoolArray, new[] { true, false });
        
        var nested = new TagStruct(TagDataType.Double, 3.14);
        nested.Entities.Add(numbers);
        
        root.Entities.Add(nested);
        root.Entities.Add(flags);

        // Act
        var result = TagSerializer.Serialize(root);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void Serialize_DataAndByteArray_ShouldBothWork()
    {
        // Arrange
        var dataBytes = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };
        var byteArrayBytes = new byte[] { 0x10, 0x20, 0x30, 0x40, 0x50 };
        
        var dataTag = new TagStruct(TagDataType.Data, dataBytes);
        var byteArrayTag = new TagStruct(TagDataType.ByteArray, byteArrayBytes);

        // Act
        var dataResult = TagSerializer.Serialize(dataTag);
        var byteArrayResult = TagSerializer.Serialize(byteArrayTag);

        // Assert
        Assert.NotNull(dataResult);
        Assert.NotNull(byteArrayResult);
        Assert.NotEmpty(dataResult);
        Assert.NotEmpty(byteArrayResult);
    }

    private TagStruct CreateNestedTagStruct(int depth)
    {
        if (depth <= 0)
            return new TagStruct(TagDataType.String, "leaf");

        var tag = new TagStruct(TagDataType.Int, depth);
        tag.Entities.Add(CreateNestedTagStruct(depth - 1));
        return tag;
    }
}

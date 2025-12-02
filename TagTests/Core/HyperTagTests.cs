using HyperTAG.Core;
using HyperTAG.Models;
using HyperTAG.Structs;

namespace TagTests.Core;

public class HyperTagTests
{
    [Fact]
    public void Constructor_Default_ShouldCreateEmptyTag()
    {
        // Arrange & Act
        var tag = new HyperTag();

        // Assert
        Assert.Equal(TagDataType.Empty, tag.Type);
        Assert.Null(tag.Value);
        Assert.Empty(tag.Entities);
    }

    [Fact]
    public void Constructor_WithTypeAndValue_ShouldCreateTagWithData()
    {
        // Arrange & Act
        var tag = new HyperTag(TagDataType.String, "test value");

        // Assert
        Assert.Equal(TagDataType.String, tag.Type);
        Assert.Equal("test value", tag.Value);
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var tag = new HyperTag();

        // Act
        tag.Type = TagDataType.Bool;
        tag.Value = true;

        // Assert
        Assert.Equal(TagDataType.Bool, tag.Type);
        Assert.True((bool)tag.Value!);
    }

    [Fact]
    public void Entities_ShouldBeMutable()
    {
        // Arrange
        var tag = new HyperTag();
        var subTag = new HyperTag(TagDataType.Int, 42);

        // Act
        tag.Entities.Add(subTag);

        // Assert
        Assert.Single(tag.Entities);
        Assert.Equal(subTag, tag.Entities[0]);
    }

    [Fact]
    public void MultipleEntities_ShouldBeSupported()
    {
        // Arrange
        var tag = new HyperTag();
        var subTag1 = new HyperTag(TagDataType.String, "first");
        var subTag2 = new HyperTag(TagDataType.Int, 2);
        var subTag3 = new HyperTag(TagDataType.Bool, true);

        // Act
        tag.Entities.Add(subTag1);
        tag.Entities.Add(subTag2);
        tag.Entities.Add(subTag3);

        // Assert
        Assert.Equal(3, tag.Entities.Count);
        Assert.Equal("first", tag.Entities[0].Value);
        Assert.Equal(2, tag.Entities[1].Value);
        Assert.True((bool)tag.Entities[2].Value!);
    }

    [Fact]
    public void NestedEntities_ShouldBeSupported()
    {
        // Arrange
        var root = new HyperTag(TagDataType.String, "root");
        var child = new HyperTag(TagDataType.Int, 1);
        var grandchild = new HyperTag(TagDataType.Bool, true);

        // Act
        child.Entities.Add(grandchild);
        root.Entities.Add(child);

        // Assert
        Assert.Single(root.Entities);
        Assert.Single(root.Entities[0].Entities);
        Assert.True((bool)root.Entities[0].Entities[0].Value!);
    }

    [Fact]
    public void ToStruct_ShouldConvertToTagStruct()
    {
        // Arrange
        var tag = new HyperTag(TagDataType.Int, 42);
        var subTag = new HyperTag(TagDataType.String, "sub");
        tag.Entities.Add(subTag);

        // Act
        var tagStruct = tag.ToStruct();

        // Assert
        Assert.Equal(TagDataType.Int, tagStruct.Type);
        Assert.Equal(42, tagStruct.Value);
        Assert.Single(tagStruct.Entities);
        Assert.Equal(TagDataType.String, tagStruct.Entities[0].Type);
        Assert.Equal("sub", tagStruct.Entities[0].Value);
    }

    [Fact]
    public void FromStruct_ShouldConvertFromTagStruct()
    {
        // Arrange
        var tagStruct = new TagStruct(TagDataType.Bool, true);
        var subStruct = new TagStruct(TagDataType.Float, 3.14f);
        tagStruct.Entities.Add(subStruct);

        // Act
        var tag = HyperTag.FromStruct(tagStruct);

        // Assert
        Assert.Equal(TagDataType.Bool, tag.Type);
        Assert.True((bool)tag.Value!);
        Assert.Single(tag.Entities);
        Assert.Equal(TagDataType.Float, tag.Entities[0].Type);
        Assert.Equal(3.14f, tag.Entities[0].Value);
    }

    [Fact]
    public void Clone_ShouldCreateDeepCopy()
    {
        // Arrange
        var original = new HyperTag(TagDataType.String, "original");
        var subTag = new HyperTag(TagDataType.Int, 100);
        original.Entities.Add(subTag);

        // Act
        var clone = original.Clone();

        // Assert
        Assert.Equal(original.Type, clone.Type);
        Assert.Equal(original.Value, clone.Value);
        Assert.NotSame(original.Entities, clone.Entities);
        Assert.Equal(original.Entities.Count, clone.Entities.Count);

        // Modify clone and ensure original is not affected
        clone.Value = "modified";
        clone.Entities[0].Value = 200;

        Assert.Equal("original", original.Value);
        Assert.Equal(100, original.Entities[0].Value);
    }

    [Fact]
    public void Clone_WithArrayTypes_ShouldCreateDeepCopy()
    {
        // Arrange
        var original = new HyperTag(TagDataType.IntArray, new[] { 1, 2, 3 });
        var subTag = new HyperTag(TagDataType.StringArray, new[] { "a", "b", "c" });
        original.Entities.Add(subTag);

        // Act
        var clone = original.Clone();

        // Assert
        Assert.Equal(original.Type, clone.Type);
        Assert.Equal(original.Value, clone.Value);
        Assert.NotSame(original.Value, clone.Value);
        Assert.NotSame(original.Entities, clone.Entities);
        
        // Modify clone array and ensure original is not affected
        var cloneArray = (int[])clone.Value!;
        cloneArray[0] = 999;

        var originalArray = (int[])original.Value!;
        Assert.Equal(1, originalArray[0]); // Original should remain unchanged
    }

    [Fact]
    public void Clone_WithDataTypes_ShouldCreateDeepCopy()
    {
        // Arrange
        var originalData = new byte[] { 1, 2, 3, 4, 5 };
        var original = new HyperTag(TagDataType.Data, originalData);

        // Act
        var clone = original.Clone();

        // Assert
        Assert.Equal(original.Type, clone.Type);
        Assert.Equal(original.Value, clone.Value);
        Assert.NotSame(original.Value, clone.Value);
        
        // Modify clone data and ensure original is not affected
        var cloneData = (byte[])clone.Value!;
        cloneData[0] = 99;

        Assert.Equal(1, originalData[0]); // Original should remain unchanged
    }

    [Fact]
    public void Serialize_ShouldReturnNonNullByteArray()
    {
        // Arrange
        var tag = new HyperTag(TagDataType.String, "test data");

        // Act
        var result = tag.Serialize();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void Serialize_WithCustomRecursionDepth_ShouldWork()
    {
        // Arrange
        var tag = new HyperTag(TagDataType.Int, 123);
        var maxRecursionDepth = 100;

        // Act
        var result = tag.Serialize(maxRecursionDepth);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void Deserialize_ShouldReconstructOriginalTag()
    {
        // Arrange
        var original = new HyperTag(TagDataType.String, "test");
        var subTag = new HyperTag(TagDataType.Int, 42);
        original.Entities.Add(subTag);

        var serialized = original.Serialize();

        // Act
        var deserialized = HyperTag.Deserialize(serialized!);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(original.Type, deserialized!.Type);
        Assert.Equal(original.Value, deserialized.Value);
        Assert.Equal(original.Entities.Count, deserialized.Entities.Count);
        Assert.Equal(original.Entities[0].Type, deserialized.Entities[0].Type);
        Assert.Equal(original.Entities[0].Value, deserialized.Entities[0].Value);
    }

    [Fact]
    public void Deserialize_WithArrayTypes_ShouldReconstructCorrectly()
    {
        // Arrange
        var original = new HyperTag(TagDataType.DoubleArray, new[] { 1.1, 2.2, 3.3 });
        var subTag = new HyperTag(TagDataType.BoolArray, new[] { true, false, true });
        original.Entities.Add(subTag);

        var serialized = original.Serialize();

        // Act
        var deserialized = HyperTag.Deserialize(serialized!);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(TagDataType.DoubleArray, deserialized!.Type);
        Assert.Equal(new[] { 1.1, 2.2, 3.3 }, deserialized.Value);
        Assert.Single(deserialized.Entities);
        Assert.Equal(new[] { true, false, true }, deserialized.Entities[0].Value);
    }

    [Fact]
    public void Deserialize_WithDataTypes_ShouldReconstructCorrectly()
    {
        // Arrange
        var originalData = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };
        var original = new HyperTag(TagDataType.Data, originalData);
        var subTag = new HyperTag(TagDataType.ByteArray, new byte[] { 0x10, 0x20, 0x30 });
        original.Entities.Add(subTag);

        var serialized = original.Serialize();

        // Act
        var deserialized = HyperTag.Deserialize(serialized!);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(TagDataType.Data, deserialized!.Type);
        Assert.Equal(originalData, deserialized.Value);
        Assert.Single(deserialized.Entities);
        Assert.Equal(new byte[] { 0x10, 0x20, 0x30 }, deserialized.Entities[0].Value);
    }

    [Fact]
    public void StaticSerialize_ShouldWorkSameAsInstanceMethod()
    {
        // Arrange
        var tag = new HyperTag(TagDataType.Double, 3.14159);

        // Act
        var instanceResult = tag.Serialize();
        var staticResult = HyperTag.Serialize(tag);

        // Assert
        Assert.NotNull(instanceResult);
        Assert.NotNull(staticResult);
        Assert.Equal(instanceResult, staticResult);
    }

    [Fact]
    public void CloneValue_ShouldDeepCopyByteArray()
    {
        // Arrange
        var originalBytes = new byte[] { 1, 2, 3, 4, 5 };

        // Act
        var clonedBytes = (byte[])HyperTag.CloneValue(originalBytes, TagDataType.ByteArray)!;

        // Assert
        Assert.Equal(originalBytes, clonedBytes);
        Assert.NotSame(originalBytes, clonedBytes);
    }

    [Fact]
    public void CloneValue_ShouldDeepCopyData()
    {
        // Arrange
        var originalData = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };

        // Act
        var clonedData = (byte[])HyperTag.CloneValue(originalData, TagDataType.Data)!;

        // Assert
        Assert.Equal(originalData, clonedData);
        Assert.NotSame(originalData, clonedData);
    }

    [Fact]
    public void CloneValue_ShouldDeepCopyString()
    {
        // Arrange
        var originalString = "test string";

        // Act
        var clonedString = (string)HyperTag.CloneValue(originalString, TagDataType.String)!;

        // Assert
        Assert.Equal(originalString, clonedString);
    }

    [Fact]
    public void CloneValue_ShouldDeepCopyAllArrayTypes()
    {
        // Arrange
        var testCases = new[]
        {
            (TagDataType.BoolArray, (object)new[] { true, false, true }),
            (TagDataType.CharArray, (object)new[] { 'a', 'b', 'c' }),
            (TagDataType.ShortArray, (object)new short[] { 1, 2, 3 }),
            (TagDataType.IntArray, (object)new[] { 1, 2, 3 }),
            (TagDataType.LongArray, (object)new long[] { 1L, 2L, 3L }),
            (TagDataType.FloatArray, (object)new[] { 1.1f, 2.2f, 3.3f }),
            (TagDataType.DoubleArray, (object)new[] { 1.1, 2.2, 3.3 }),
            (TagDataType.StringArray, (object)new[] { "a", "b", "c" }),
            (TagDataType.DecimalArray, (object)new[] { 1.1m, 2.2m, 3.3m }),
            (TagDataType.UShortArray, (object)new ushort[] { 1, 2, 3 }),
            (TagDataType.UIntArray, (object)new uint[] { 1u, 2u, 3u }),
            (TagDataType.ULongArray, (object)new ulong[] { 1ul, 2ul, 3ul }),
            (TagDataType.SByteArray, (object)new sbyte[] { 1, 2, 3 })
        };

        foreach (var (dataType, value) in testCases)
        {
            // Act
            var cloned = HyperTag.CloneValue(value, dataType);

            // Assert
            Assert.NotNull(cloned);
            Assert.Equal(value, cloned);
            Assert.NotSame(value, cloned);
        }
    }

    [Fact]
    public void RoundTrip_ComplexStructure_ShouldPreserveAllData()
    {
        // Arrange
        var root = new HyperTag(TagDataType.String, "root");
        var numbers = new HyperTag(TagDataType.IntArray, new[] { 1, 2, 3, 4, 5 });
        var flags = new HyperTag(TagDataType.BoolArray, new[] { true, false, true });
        var texts = new HyperTag(TagDataType.StringArray, new[] { "hello", "world" });
        var data = new HyperTag(TagDataType.Data, new byte[] { 0x01, 0x02, 0x03 });

        var nested = new HyperTag(TagDataType.Double, 3.14159);
        nested.Entities.Add(numbers);

        root.Entities.Add(nested);
        root.Entities.Add(flags);
        root.Entities.Add(texts);
        root.Entities.Add(data);

        // Act
        var serialized = root.Serialize();
        var deserialized = HyperTag.Deserialize(serialized!);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal("root", deserialized!.Value);
        Assert.Equal(4, deserialized.Entities.Count);

        // Check nested structure
        Assert.Equal(TagDataType.Double, deserialized.Entities[0].Type);
        Assert.Equal(3.14159, deserialized.Entities[0].Value);
        Assert.Single(deserialized.Entities[0].Entities);
        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, deserialized.Entities[0].Entities[0].Value);

        // Check other entities
        Assert.Equal(new[] { true, false, true }, deserialized.Entities[1].Value);
        Assert.Equal(new[] { "hello", "world" }, deserialized.Entities[2].Value);
        Assert.Equal(new byte[] { 0x01, 0x02, 0x03 }, deserialized.Entities[3].Value);
    }
}

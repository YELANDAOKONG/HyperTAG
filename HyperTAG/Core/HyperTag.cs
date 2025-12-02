using HyperTAG.Models;
using HyperTAG.Structs;
using HyperTAG.Utilities;

namespace HyperTAG.Core;

public class HyperTag
{
    private TagStruct _struct;

    public TagDataType Type 
    { 
        get => _struct.Type; 
        set => _struct.Type = value; 
    }

    public object? Value 
    { 
        get => _struct.Value; 
        set => _struct.Value = value; 
    }

    public List<HyperTag> Entities { get; private set; } = new List<HyperTag>();

    public HyperTag()
    {
        _struct = new TagStruct();
    }

    public HyperTag(TagDataType type, object? value = null)
    {
        _struct = new TagStruct(type, value);
    }

    // Class <-> Struct conversions
    public TagStruct ToStruct()
    {
        var result = new TagStruct
        {
            Type = _struct.Type,
            Value = CloneValue(_struct.Value, _struct.Type)
        };

        foreach (var subTag in Entities)
        {
            result.Entities.Add(subTag.ToStruct());
        }

        return result;
    }

    public static HyperTag FromStruct(TagStruct tagStruct)
    {
        var hyperTag = new HyperTag
        {
            Type = tagStruct.Type,
            Value = CloneValue(tagStruct.Value, tagStruct.Type)
        };

        foreach (var subStruct in tagStruct.Entities)
        {
            hyperTag.Entities.Add(FromStruct(subStruct));
        }

        return hyperTag;
    }

    // Clone
    public HyperTag Clone()
    {
        return FromStruct(ToStruct());
    }

    // Binary serialization methods
    public byte[]? Serialize(int maxRecursionDepth = TagSerializer.MaxRecursionDepth)
    {
        return TagSerializer.Serialize(ToStruct(), maxRecursionDepth);
    }

    public static byte[]? Serialize(HyperTag tag, int maxRecursionDepth = TagSerializer.MaxRecursionDepth)
    {
        return tag.Serialize(maxRecursionDepth);
    }

    public static HyperTag? Deserialize(byte[] bytes, int maxRecursionDepth = TagDeserializer.MaxRecursionDepth)
    {
        var tagStruct = TagDeserializer.Deserialize(bytes, maxRecursionDepth);
        return tagStruct != null ? FromStruct(tagStruct) : null;
    }

    // JSON serialization methods
    public string? SerializeToJson(bool throwExceptions = false)
    {
        return TagJsonSerializer.Serialize(ToStruct(), throwExceptions);
    }

    public static string? SerializeToJson(HyperTag tag, bool throwExceptions = false)
    {
        return tag.SerializeToJson(throwExceptions);
    }

    public static HyperTag? DeserializeFromJson(string json, bool throwExceptions = false)
    {
        var tagStruct = TagJsonDeserializer.Deserialize(json, throwExceptions);
        return tagStruct != null ? FromStruct(tagStruct) : null;
    }

    // BSON serialization methods
    public byte[]? SerializeToBson(bool throwExceptions = false)
    {
        return TagBsonSerializer.Serialize(ToStruct(), throwExceptions);
    }

    public static byte[]? SerializeToBson(HyperTag tag, bool throwExceptions = false)
    {
        return tag.SerializeToBson(throwExceptions);
    }

    public static HyperTag? DeserializeFromBson(byte[] bson, bool throwExceptions = false)
    {
        var tagStruct = TagBsonDeserializer.Deserialize(bson, throwExceptions);
        return tagStruct != null ? FromStruct(tagStruct) : null;
    }

    // Helper method for deep copying values
    public static object? CloneValue(object? value, TagDataType type)
    {
        if (value == null) return null;

        return type switch
        {
            // Single value types
            TagDataType.String when value is string stringData => new string(stringData),
            
            // Data and ByteArray types - deep copy byte arrays
            TagDataType.Data when value is byte[] data => data.ToArray(),
            TagDataType.ByteArray when value is byte[] byteArray => byteArray.ToArray(),
            
            // Array types - use Array.Copy for deep cloning
            TagDataType.BoolArray when value is bool[] array => array.ToArray(),
            TagDataType.CharArray when value is char[] array => array.ToArray(),
            TagDataType.ShortArray when value is short[] array => array.ToArray(),
            TagDataType.IntArray when value is int[] array => array.ToArray(),
            TagDataType.LongArray when value is long[] array => array.ToArray(),
            TagDataType.FloatArray when value is float[] array => array.ToArray(),
            TagDataType.DoubleArray when value is double[] array => array.ToArray(),
            TagDataType.StringArray when value is string[] array => array.Select(s => new string(s ?? "")).ToArray(),
            TagDataType.DecimalArray when value is decimal[] array => array.ToArray(),
            TagDataType.UShortArray when value is ushort[] array => array.ToArray(),
            TagDataType.UIntArray when value is uint[] array => array.ToArray(),
            TagDataType.ULongArray when value is ulong[] array => array.ToArray(),
            TagDataType.SByteArray when value is sbyte[] array => array.ToArray(),
            
            _ => value
        };
    }
}

using System.IO.Compression;
using HyperTAG.Exceptions;
using HyperTAG.Models;
using HyperTAG.Structs;

namespace HyperTAG.Utilities;

public static class TagDeserializer
{
    public const int MaxRecursionDepth = 8192;
    public const string MagicString = "@HYPER";
    public const string TypeString = "%TAG";
    
    public static TagStruct? Deserialize(byte[] bytes, int maxRecursionDepth = MaxRecursionDepth, bool throwExceptions = false)
    {
        try
        {
            using var byteStream = new MemoryStream(bytes);
            using var reader = new BinaryReader(byteStream);
            
            // Read header: [@HYPER][%TAG][INT64: compressed data length][DATA...]
            var magicString = reader.ReadString();
            if (magicString != MagicString)
            {
                throw new TagDeserializeException("The magic string is not correct.");
            }

            var tagString = reader.ReadString();
            if (tagString != TypeString)
            {
                throw new TagDeserializeException("This file is not a tag file.");
            }

            // Read compressed data length
            var compressedDataLength = reader.ReadInt64();
            
            // Read the compressed data
            var compressedData = reader.ReadBytes((int)compressedDataLength);
            
            // Decompress and deserialize
            using var compressedStream = new MemoryStream(compressedData);
            using var deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress);
            using var tagReader = new BinaryReader(deflateStream);
            
            return DeserializeTag(tagReader, maxRecursionDepth);
        }
        catch (Exception)
        {
            if (throwExceptions) throw;
            return null;
        }
    }

    private static TagStruct DeserializeTag(BinaryReader reader, int maxRecursionDepth, int depthCounter = 0)
    {
        if (depthCounter >= maxRecursionDepth)
        {
            throw new TagRecursionException(
                $"Recursion depth has exceeded the allowed maximum limit ({depthCounter}/{maxRecursionDepth}).");
        }

        var tag = new TagStruct();
        tag.Type = (TagDataType)reader.ReadInt32();

        var isValueNull = reader.ReadBoolean();
        if (isValueNull)
        {
            tag.Value = null;
        }
        else
        {
            tag.Value = tag.Type switch
            {
                // Single value types
                TagDataType.Empty => null,
                TagDataType.Null => null,
                TagDataType.Data => ReadByteArray(reader),
                TagDataType.Bool => reader.ReadBoolean(),
                TagDataType.Char => reader.ReadChar(),
                TagDataType.Byte => reader.ReadByte(),
                TagDataType.Short => reader.ReadInt16(),
                TagDataType.Int => reader.ReadInt32(),
                TagDataType.Long => reader.ReadInt64(),
                TagDataType.Float => reader.ReadSingle(),
                TagDataType.Double => reader.ReadDouble(),
                TagDataType.String => reader.ReadString(),
                TagDataType.Decimal => reader.ReadDecimal(),
                TagDataType.UShort => reader.ReadUInt16(),
                TagDataType.UInt => reader.ReadUInt32(),
                TagDataType.ULong => reader.ReadUInt64(),
                TagDataType.SByte => reader.ReadSByte(),
                
                // Array types
                TagDataType.BoolArray => ReadArray(reader, r => r.ReadBoolean()),
                TagDataType.CharArray => ReadArray(reader, r => r.ReadChar()),
                TagDataType.ByteArray => ReadArray(reader, r => r.ReadByte()),
                TagDataType.ShortArray => ReadArray(reader, r => r.ReadInt16()),
                TagDataType.IntArray => ReadArray(reader, r => r.ReadInt32()),
                TagDataType.LongArray => ReadArray(reader, r => r.ReadInt64()),
                TagDataType.FloatArray => ReadArray(reader, r => r.ReadSingle()),
                TagDataType.DoubleArray => ReadArray(reader, r => r.ReadDouble()),
                TagDataType.StringArray => ReadArray(reader, r => r.ReadString()),
                TagDataType.DecimalArray => ReadArray(reader, r => r.ReadDecimal()),
                TagDataType.UShortArray => ReadArray(reader, r => r.ReadUInt16()),
                TagDataType.UIntArray => ReadArray(reader, r => r.ReadUInt32()),
                TagDataType.ULongArray => ReadArray(reader, r => r.ReadUInt64()),
                TagDataType.SByteArray => ReadArray(reader, r => r.ReadSByte()),
                
                _ => throw new TagDeserializeException("Unsupported Data Type during deserialization.")
            };
        }

        var subTagCount = reader.ReadInt32();
        for (var i = 0; i < subTagCount; i++)
        {
            var subTag = DeserializeTag(reader, maxRecursionDepth, depthCounter + 1);
            tag.Entities.Add(subTag);
        }

        return tag;
    }
    
    private static byte[] ReadByteArray(BinaryReader reader)
    {
        var length = reader.ReadInt32();
        return reader.ReadBytes(length);
    }

    private static T[] ReadArray<T>(BinaryReader reader, Func<BinaryReader, T> readElement)
    {
        var length = reader.ReadInt32();
        var array = new T[length];
        for (var i = 0; i < length; i++)
        {
            array[i] = readElement(reader);
        }
        return array;
    }
}

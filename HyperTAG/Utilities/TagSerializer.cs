using System.IO.Compression;
using HyperTAG.Exceptions;
using HyperTAG.Models;
using HyperTAG.Structs;

namespace HyperTAG.Utilities;

public static class TagSerializer
{
    public const int MaxRecursionDepth = 8192;
    public const string MagicString = "@HYPER";
    public const string TypeString = "%TAG";
    
    public static byte[]? Serialize(TagStruct tagStruct, int maxRecursionDepth = MaxRecursionDepth, bool throwExceptions = false)
    {
        try
        {
            // First serialize the tag data to a compressed stream
            byte[] compressedData;
            using (var dataStream = new MemoryStream())
            {
                using (var deflateStream = new DeflateStream(dataStream, CompressionLevel.Optimal, true))
                using (var dataWriter = new BinaryWriter(deflateStream))
                {
                    SerializeTag(dataWriter, tagStruct, maxRecursionDepth);
                    dataWriter.Flush();
                }
                compressedData = dataStream.ToArray();
            }

            // Now create the final stream with header and compressed data length
            using var finalStream = new MemoryStream();
            using var finalWriter = new BinaryWriter(finalStream);
            
            // Write header: [@HYPER][%TAG][INT64: compressed data length][DATA...]
            finalWriter.Write(MagicString);
            finalWriter.Write(TypeString);
            finalWriter.Write((long)compressedData.Length); // Write compressed data length as INT64
            finalWriter.Write(compressedData); // Write the actual compressed data
            
            finalWriter.Flush();
            return finalStream.ToArray();
        }
        catch (Exception)
        {
            if (throwExceptions) throw;
            return null;
        }
    }

    private static void SerializeTag(BinaryWriter writer, TagStruct tag, int maxRecursionDepth, int depthCounter = 0)
    {
        if (depthCounter >= maxRecursionDepth)
        {
            throw new TagRecursionException(
                $"Recursion depth has exceeded the allowed maximum limit ({depthCounter}/{maxRecursionDepth}).");
        }

        writer.Write((int)tag.Type);
        
        if (tag.Value == null)
        {
            writer.Write(true);
        }
        else
        {
            writer.Write(false);
            switch (tag.Type)
            {
                // Single value types
                case TagDataType.Empty:
                    writer.Write(false);
                    break;
                case TagDataType.Null:
                    writer.Write(true);
                    break;
                case TagDataType.Data:
                    WriteByteArray(writer, (byte[])tag.Value);
                    break;
                case TagDataType.Bool:
                    writer.Write((bool)tag.Value);
                    break;
                case TagDataType.Char:
                    writer.Write((char)tag.Value);
                    break;
                case TagDataType.Byte:
                    writer.Write((byte)tag.Value);
                    break;
                case TagDataType.Short:
                    writer.Write((short)tag.Value);
                    break;
                case TagDataType.Int:
                    writer.Write((int)tag.Value);
                    break;
                case TagDataType.Long:
                    writer.Write((long)tag.Value);
                    break;
                case TagDataType.Float:
                    writer.Write((float)tag.Value);
                    break;
                case TagDataType.Double:
                    writer.Write((double)tag.Value);
                    break;
                case TagDataType.String:
                    writer.Write((string)tag.Value);
                    break;
                case TagDataType.Decimal:
                    writer.Write((decimal)tag.Value);
                    break;
                case TagDataType.UShort:
                    writer.Write((ushort)tag.Value);
                    break;
                case TagDataType.UInt:
                    writer.Write((uint)tag.Value);
                    break;
                case TagDataType.ULong:
                    writer.Write((ulong)tag.Value);
                    break;
                case TagDataType.SByte:
                    writer.Write((sbyte)tag.Value);
                    break;
                
                // Array types
                case TagDataType.BoolArray:
                    WriteArray(writer, (bool[])tag.Value, (w, v) => w.Write(v));
                    break;
                case TagDataType.CharArray:
                    WriteArray(writer, (char[])tag.Value, (w, v) => w.Write(v));
                    break;
                case TagDataType.ByteArray:
                    WriteArray(writer, (byte[])tag.Value, (w, v) => w.Write(v));
                    break;
                case TagDataType.ShortArray:
                    WriteArray(writer, (short[])tag.Value, (w, v) => w.Write(v));
                    break;
                case TagDataType.IntArray:
                    WriteArray(writer, (int[])tag.Value, (w, v) => w.Write(v));
                    break;
                case TagDataType.LongArray:
                    WriteArray(writer, (long[])tag.Value, (w, v) => w.Write(v));
                    break;
                case TagDataType.FloatArray:
                    WriteArray(writer, (float[])tag.Value, (w, v) => w.Write(v));
                    break;
                case TagDataType.DoubleArray:
                    WriteArray(writer, (double[])tag.Value, (w, v) => w.Write(v));
                    break;
                case TagDataType.StringArray:
                    WriteArray(writer, (string[])tag.Value, (w, v) => w.Write(v ?? ""));
                    break;
                case TagDataType.DecimalArray:
                    WriteArray(writer, (decimal[])tag.Value, (w, v) => w.Write(v));
                    break;
                case TagDataType.UShortArray:
                    WriteArray(writer, (ushort[])tag.Value, (w, v) => w.Write(v));
                    break;
                case TagDataType.UIntArray:
                    WriteArray(writer, (uint[])tag.Value, (w, v) => w.Write(v));
                    break;
                case TagDataType.ULongArray:
                    WriteArray(writer, (ulong[])tag.Value, (w, v) => w.Write(v));
                    break;
                case TagDataType.SByteArray:
                    WriteArray(writer, (sbyte[])tag.Value, (w, v) => w.Write(v));
                    break;
                    
                default:
                    throw new TagSerializeException("Unsupported Data Type");
            }
        }

        writer.Write(tag.Entities.Count);
        foreach (var subTag in tag.Entities)
        {
            SerializeTag(writer, subTag, maxRecursionDepth, depthCounter + 1);
        }
    }

    private static void WriteByteArray(BinaryWriter writer, byte[] data)
    {
        writer.Write(data.Length);
        writer.Write(data);
    }

    private static void WriteArray<T>(BinaryWriter writer, T[] array, Action<BinaryWriter, T> writeElement)
    {
        writer.Write(array.Length);
        foreach (var element in array)
        {
            writeElement(writer, element);
        }
    }
}

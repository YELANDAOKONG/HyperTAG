using System.Reflection;
using HyperTAG.Attributes;
using HyperTAG.Models;

namespace HyperTAG.Utilities;

/// <summary>
/// Internal helper for TagAutoSerializer and TagAutoDeserializer
/// </summary>
public static class TagTypeHelper
{
    private static readonly Dictionary<Type, MemberInfo[]> MemberCache = new();
    
    /// <summary>
    /// Gets all fields and properties marked with TagData attribute for the given type
    /// </summary>
    public static MemberInfo[] GetTagDataMembers(Type type)
    {
        if (MemberCache.TryGetValue(type, out var cached))
        {
            return cached;
        }
        
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(f => f.GetCustomAttribute<TagDataAttribute>() != null)
            .Cast<MemberInfo>();
            
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<TagDataAttribute>() != null)
            .Cast<MemberInfo>();
        
        var members = fields.Concat(properties).ToArray();
        
        MemberCache[type] = members;
        return members;
    }
    
    /// <summary>
    /// Gets the value of a member (field or property)
    /// </summary>
    public static object? GetMemberValue(MemberInfo member, object obj)
    {
        return member switch
        {
            FieldInfo field => field.GetValue(obj),
            PropertyInfo property => property.GetValue(obj),
            _ => throw new InvalidOperationException($"Unsupported member type: {member.GetType().Name}")
        };
    }
    
    /// <summary>
    /// Sets the value of a member (field or property)
    /// </summary>
    public static void SetMemberValue(MemberInfo member, object obj, object? value)
    {
        switch (member)
        {
            case FieldInfo field:
                field.SetValue(obj, value);
                break;
            case PropertyInfo property:
                if (property.CanWrite)
                {
                    property.SetValue(obj, value);
                }
                break;
            default:
                throw new InvalidOperationException($"Unsupported member type: {member.GetType().Name}");
        }
    }
    
    /// <summary>
    /// Gets the type of a member (field or property)
    /// </summary>
    public static Type GetMemberType(MemberInfo member)
    {
        return member switch
        {
            FieldInfo field => field.FieldType,
            PropertyInfo property => property.PropertyType,
            _ => throw new InvalidOperationException($"Unsupported member type: {member.GetType().Name}")
        };
    }
    
    /// <summary>
    /// Maps C# types to TagDataType enum values
    /// </summary>
    public static TagDataType? GetTagDataType(Type type)
    {
        // Single value types
        if (type == typeof(bool)) return TagDataType.Bool;
        if (type == typeof(char)) return TagDataType.Char;
        if (type == typeof(byte)) return TagDataType.Byte;
        if (type == typeof(short)) return TagDataType.Short;
        if (type == typeof(int)) return TagDataType.Int;
        if (type == typeof(long)) return TagDataType.Long;
        if (type == typeof(float)) return TagDataType.Float;
        if (type == typeof(double)) return TagDataType.Double;
        if (type == typeof(string)) return TagDataType.String;
        if (type == typeof(decimal)) return TagDataType.Decimal;
        if (type == typeof(ushort)) return TagDataType.UShort;
        if (type == typeof(uint)) return TagDataType.UInt;
        if (type == typeof(ulong)) return TagDataType.ULong;
        if (type == typeof(sbyte)) return TagDataType.SByte;
        
        // Array types
        if (type == typeof(bool[])) return TagDataType.BoolArray;
        if (type == typeof(char[])) return TagDataType.CharArray;
        if (type == typeof(byte[])) return TagDataType.ByteArray;
        if (type == typeof(short[])) return TagDataType.ShortArray;
        if (type == typeof(int[])) return TagDataType.IntArray;
        if (type == typeof(long[])) return TagDataType.LongArray;
        if (type == typeof(float[])) return TagDataType.FloatArray;
        if (type == typeof(double[])) return TagDataType.DoubleArray;
        if (type == typeof(string[])) return TagDataType.StringArray;
        if (type == typeof(decimal[])) return TagDataType.DecimalArray;
        if (type == typeof(ushort[])) return TagDataType.UShortArray;
        if (type == typeof(uint[])) return TagDataType.UIntArray;
        if (type == typeof(ulong[])) return TagDataType.ULongArray;
        if (type == typeof(sbyte[])) return TagDataType.SByteArray;
        
        return null;
    }
}

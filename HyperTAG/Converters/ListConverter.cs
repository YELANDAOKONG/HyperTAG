using HyperTAG.Core;
using HyperTAG.Exceptions;
using HyperTAG.Interfaces;
using HyperTAG.Models;
using HyperTAG.Utilities;

namespace HyperTAG.Converters;

/// <summary>
/// Converter for List<T> using nested entities structure.
/// Supports any element type that can be serialized (basic types, ITagStruct, [TagStruct], or custom converters).
/// </summary>
/// <typeparam name="T">The element type</typeparam>
public class ListConverter<T> : ITagConverter<List<T>>
{
    private readonly TagConverterRegistry _registry;
    private readonly TagAutoOptions _options;

    public ListConverter() : this(null, null)
    {
    }

    public ListConverter(TagConverterRegistry? registry = null, TagAutoOptions? options = null)
    {
        _registry = registry ?? TagConverterRegistry.Default;
        _options = options ?? TagAutoOptions.Default;
    }

    public HyperTag? ToTag(List<T>? obj)
    {
        if (obj == null)
            return new HyperTag(TagDataType.Null);

        // Use Empty type container with nested entities
        var tag = new HyperTag(TagDataType.Empty);
        
        foreach (var item in obj)
        {
            if (item == null)
            {
                tag.Entities.Add(new HyperTag(TagDataType.Null));
                continue;
            }

            var itemTag = SerializeItem(item);
            tag.Entities.Add(itemTag);
        }

        return tag;
    }

    public List<T>? FromTag(HyperTag? tag)
    {
        if (tag == null || tag.Type == TagDataType.Null)
            return null;

        if (tag.Type != TagDataType.Empty)
        {
            throw new TagAutoDeserializeException(
                $"Expected Empty type container for List<{typeof(T).Name}>, but got {tag.Type}");
        }

        var list = new List<T>();

        foreach (var itemTag in tag.Entities)
        {
            if (itemTag.Type == TagDataType.Null)
            {
                list.Add(default!);
                continue;
            }

            var item = DeserializeItem(itemTag);
            list.Add(item);
        }

        return list;
    }

    private HyperTag SerializeItem(T item)
    {
        var itemType = typeof(T);

        // Check for custom converter first
        var converter = _registry.GetConverter<T>();
        if (converter != null)
        {
            var result = converter.ToTag(item);
            if (result != null)
                return result;
        }

        // Check if it's a basic type
        var basicType = TagTypeHelper.GetTagDataType(itemType);
        if (basicType != null)
        {
            return new HyperTag(basicType.Value, item);
        }

        // Try auto-serialize for complex types
        var serialized = TagAutoSerializer.SerializeObject(item, _registry, _options);
        if (serialized != null)
            return serialized;

        throw new TagAutoSerializeException(
            $"Cannot serialize List item of type '{itemType.FullName}'. " +
            $"Ensure the type is a basic type, implements ITagStruct, is marked with [TagStruct], " +
            $"or has a registered converter.");
    }

    private T DeserializeItem(HyperTag itemTag)
    {
        var itemType = typeof(T);

        // Check for custom converter first
        var converter = _registry.GetConverter<T>();
        if (converter != null)
        {
            var result = converter.FromTag(itemTag);
            if (result != null)
                return result;
        }

        // Check if it's a basic type
        var basicType = TagTypeHelper.GetTagDataType(itemType);
        if (basicType != null)
        {
            if (itemTag.Type != basicType)
            {
                throw new TagAutoDeserializeException(
                    $"Type mismatch for List item: expected {basicType}, got {itemTag.Type}");
            }
            return (T)itemTag.Value!;
        }

        // Try auto-deserialize for complex types
        var deserialized = TagAutoDeserializer.DeserializeObject<T>(itemTag, _registry, _options);
        if (deserialized != null)
            return deserialized;

        throw new TagAutoDeserializeException(
            $"Cannot deserialize List item of type '{itemType.FullName}'");
    }
}

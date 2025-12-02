using HyperTAG.Core;
using HyperTAG.Exceptions;
using HyperTAG.Interfaces;
using HyperTAG.Models;
using HyperTAG.Utilities;

namespace HyperTAG.Converters;

/// <summary>
/// Converter for Dictionary<TKey, TValue> using nested entities structure.
/// IMPORTANT: Keys must be basic types only (no nested types allowed).
/// Values can be any supported type (basic types, ITagStruct, [TagStruct], or custom converters).
/// </summary>
/// <typeparam name="TKey">The key type (must be a basic type)</typeparam>
/// <typeparam name="TValue">The value type (can be any supported type)</typeparam>
public class DictionaryConverter<TKey, TValue> : ITagConverter<Dictionary<TKey, TValue>>
    where TKey : notnull
{
    private readonly TagConverterRegistry _registry;
    private readonly TagAutoOptions _options;

    public DictionaryConverter() : this(null, null)
    {
    }

    public DictionaryConverter(TagConverterRegistry? registry = null, TagAutoOptions? options = null)
    {
        _registry = registry ?? TagConverterRegistry.Default;
        _options = options ?? TagAutoOptions.Default;

        // Validate that TKey is a basic type (nested types not allowed for keys)
        var keyType = typeof(TKey);
        if (TagTypeHelper.GetTagDataType(keyType) == null)
        {
            throw new TagAutoSerializeException(
                $"Dictionary key type '{keyType.FullName}' must be a basic type. " +
                $"Nested types are not allowed as dictionary keys.");
        }
    }

    public HyperTag? ToTag(Dictionary<TKey, TValue>? obj)
    {
        if (obj == null)
            return new HyperTag(TagDataType.Null);

        // Use Empty type container with nested pair entities
        var tag = new HyperTag(TagDataType.Empty);

        foreach (var kvp in obj)
        {
            // Each key-value pair is stored as an Empty container with 2 entities: [key, value]
            var pairTag = new HyperTag(TagDataType.Empty);
            
            // Add key (must be basic type)
            var keyType = TagTypeHelper.GetTagDataType(typeof(TKey))!.Value;
            var keyTag = new HyperTag(keyType, kvp.Key);
            pairTag.Entities.Add(keyTag);

            // Add value (can be any type)
            var valueTag = SerializeValue(kvp.Value);
            pairTag.Entities.Add(valueTag);

            tag.Entities.Add(pairTag);
        }

        return tag;
    }

    public Dictionary<TKey, TValue>? FromTag(HyperTag? tag)
    {
        if (tag == null || tag.Type == TagDataType.Null)
            return null;

        if (tag.Type != TagDataType.Empty)
        {
            throw new TagAutoDeserializeException(
                $"Expected Empty type container for Dictionary<{typeof(TKey).Name}, {typeof(TValue).Name}>, but got {tag.Type}");
        }

        var dictionary = new Dictionary<TKey, TValue>();

        foreach (var pairTag in tag.Entities)
        {
            if (pairTag.Type != TagDataType.Empty)
            {
                throw new TagAutoDeserializeException(
                    "Invalid dictionary entry: expected Empty type container for key-value pair");
            }

            if (pairTag.Entities.Count != 2)
            {
                throw new TagAutoDeserializeException(
                    $"Invalid dictionary entry: expected 2 entities (key and value), got {pairTag.Entities.Count}");
            }

            var keyTag = pairTag.Entities[0];
            var valueTag = pairTag.Entities[1];

            // Deserialize key (must be basic type)
            var expectedKeyType = TagTypeHelper.GetTagDataType(typeof(TKey))!.Value;
            if (keyTag.Type != expectedKeyType)
            {
                throw new TagAutoDeserializeException(
                    $"Dictionary key type mismatch: expected {expectedKeyType}, got {keyTag.Type}");
            }
            var key = (TKey)keyTag.Value!;

            // Deserialize value
            var value = DeserializeValue(valueTag);

            dictionary[key] = value;
        }

        return dictionary;
    }

    private HyperTag SerializeValue(TValue? value)
    {
        if (value == null)
            return new HyperTag(TagDataType.Null);

        var valueType = typeof(TValue);

        // Check for custom converter first
        var converter = _registry.GetConverter<TValue>();
        if (converter != null)
        {
            var result = converter.ToTag(value);
            if (result != null)
                return result;
        }

        // Check if it's a basic type
        var basicType = TagTypeHelper.GetTagDataType(valueType);
        if (basicType != null)
        {
            return new HyperTag(basicType.Value, value);
        }

        // Try auto-serialize for complex types
        var serialized = TagAutoSerializer.SerializeObject(value, _registry, _options);
        if (serialized != null)
            return serialized;

        throw new TagAutoSerializeException(
            $"Cannot serialize Dictionary value of type '{valueType.FullName}'. " +
            $"Ensure the type is a basic type, implements ITagStruct, is marked with [TagStruct], " +
            $"or has a registered converter.");
    }

    private TValue DeserializeValue(HyperTag valueTag)
    {
        if (valueTag.Type == TagDataType.Null)
            return default!;

        var valueType = typeof(TValue);

        // Check for custom converter first
        var converter = _registry.GetConverter<TValue>();
        if (converter != null)
        {
            var result = converter.FromTag(valueTag);
            if (result != null)
                return result;
        }

        // Check if it's a basic type
        var basicType = TagTypeHelper.GetTagDataType(valueType);
        if (basicType != null)
        {
            if (valueTag.Type != basicType)
            {
                throw new TagAutoDeserializeException(
                    $"Type mismatch for Dictionary value: expected {basicType}, got {valueTag.Type}");
            }
            return (TValue)valueTag.Value!;
        }

        // Try auto-deserialize for complex types
        var deserialized = TagAutoDeserializer.DeserializeObject<TValue>(valueTag, _registry, _options);
        if (deserialized != null)
            return deserialized;

        throw new TagAutoDeserializeException(
            $"Cannot deserialize Dictionary value of type '{valueType.FullName}'");
    }
}

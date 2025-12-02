using HyperTAG.Core;
using HyperTAG.Exceptions;
using HyperTAG.Interfaces;
using HyperTAG.Models;
using HyperTAG.Utilities;

namespace HyperTAG.Converters;

/// <summary>
/// Converter for ValueTuple<T1> using nested entities structure.
/// Elements must be basic types, ITagStruct, [TagStruct], or have registered converters.
/// </summary>
public class ValueTupleConverter<T1> : ITagConverter<ValueTuple<T1>>
{
    private readonly TagConverterRegistry _registry;
    private readonly TagAutoOptions _options;

    public ValueTupleConverter() : this(null, null)
    {
    }

    public ValueTupleConverter(TagConverterRegistry? registry = null, TagAutoOptions? options = null)
    {
        _registry = registry ?? TagConverterRegistry.Default;
        _options = options ?? TagAutoOptions.Default;
    }

    public HyperTag? ToTag(ValueTuple<T1> obj)
    {
        var tag = new HyperTag(TagDataType.Empty);
        tag.Entities.Add(SerializeItem(obj.Item1));
        return tag;
    }

    public ValueTuple<T1> FromTag(HyperTag? tag)
    {
        if (tag == null)
            throw new ArgumentNullException(nameof(tag));

        if (tag.Type == TagDataType.Null)
            throw new InvalidOperationException("Cannot convert null tag to non-nullable ValueTuple");

        if (tag.Type != TagDataType.Empty)
        {
            throw new TagAutoDeserializeException(
                $"Expected Empty type container for ValueTuple<{typeof(T1).Name}>, but got {tag.Type}");
        }

        if (tag.Entities.Count != 1)
        {
            throw new TagAutoDeserializeException(
                $"Expected 1 entity for ValueTuple<{typeof(T1).Name}>, but got {tag.Entities.Count}");
        }

        var item1 = DeserializeItem<T1>(tag.Entities[0]);
        return new ValueTuple<T1>(item1);
    }

    private HyperTag SerializeItem<T>(T? item)
    {
        if (item == null)
            return new HyperTag(TagDataType.Null);

        var itemType = typeof(T);
        var converter = _registry.GetConverter<T>();
        if (converter != null)
        {
            var result = converter.ToTag(item);
            if (result != null)
                return result;
        }

        var basicType = TagTypeHelper.GetTagDataType(itemType);
        if (basicType != null)
            return new HyperTag(basicType.Value, item);

        var serialized = TagAutoSerializer.SerializeObject(item, _registry, _options);
        if (serialized != null)
            return serialized;

        throw new TagAutoSerializeException(
            $"Cannot serialize ValueTuple item of type '{itemType.FullName}'. " +
            $"Ensure the type is a basic type, implements ITagStruct, is marked with [TagStruct], " +
            $"or has a registered converter.");
    }

    private T DeserializeItem<T>(HyperTag itemTag)
    {
        if (itemTag.Type == TagDataType.Null)
            return default!;

        var itemType = typeof(T);
        var converter = _registry.GetConverter<T>();
        if (converter != null)
        {
            var result = converter.FromTag(itemTag);
            if (result != null)
                return result;
        }

        var basicType = TagTypeHelper.GetTagDataType(itemType);
        if (basicType != null)
        {
            if (itemTag.Type != basicType)
            {
                throw new TagAutoDeserializeException(
                    $"Type mismatch for ValueTuple item: expected {basicType}, got {itemTag.Type}");
            }
            return (T)itemTag.Value!;
        }

        var deserialized = TagAutoDeserializer.DeserializeObject<T>(itemTag, _registry, _options);
        if (deserialized != null)
            return deserialized;

        throw new TagAutoDeserializeException(
            $"Cannot deserialize ValueTuple item of type '{itemType.FullName}'");
    }
}

/// <summary>
/// Converter for ValueTuple<T1, T2>
/// </summary>
public class ValueTupleConverter<T1, T2> : ITagConverter<(T1, T2)>
{
    private readonly TagConverterRegistry _registry;
    private readonly TagAutoOptions _options;

    public ValueTupleConverter() : this(null, null)
    {
    }

    public ValueTupleConverter(TagConverterRegistry? registry = null, TagAutoOptions? options = null)
    {
        _registry = registry ?? TagConverterRegistry.Default;
        _options = options ?? TagAutoOptions.Default;
    }

    public HyperTag? ToTag((T1, T2) obj)
    {
        var tag = new HyperTag(TagDataType.Empty);
        tag.Entities.Add(SerializeItem(obj.Item1));
        tag.Entities.Add(SerializeItem(obj.Item2));
        return tag;
    }

    public (T1, T2) FromTag(HyperTag? tag)
    {
        if (tag == null)
            throw new ArgumentNullException(nameof(tag));

        if (tag.Type == TagDataType.Null)
            throw new InvalidOperationException("Cannot convert null tag to non-nullable ValueTuple");

        if (tag.Type != TagDataType.Empty)
        {
            throw new TagAutoDeserializeException(
                $"Expected Empty type container for ValueTuple, but got {tag.Type}");
        }

        if (tag.Entities.Count != 2)
        {
            throw new TagAutoDeserializeException(
                $"Expected 2 entities for ValueTuple, but got {tag.Entities.Count}");
        }

        var item1 = DeserializeItem<T1>(tag.Entities[0]);
        var item2 = DeserializeItem<T2>(tag.Entities[1]);
        return (item1, item2);
    }

    private HyperTag SerializeItem<T>(T? item)
    {
        if (item == null)
            return new HyperTag(TagDataType.Null);

        var itemType = typeof(T);
        var converter = _registry.GetConverter<T>();
        if (converter != null)
        {
            var result = converter.ToTag(item);
            if (result != null)
                return result;
        }

        var basicType = TagTypeHelper.GetTagDataType(itemType);
        if (basicType != null)
            return new HyperTag(basicType.Value, item);

        var serialized = TagAutoSerializer.SerializeObject(item, _registry, _options);
        if (serialized != null)
            return serialized;

        throw new TagAutoSerializeException(
            $"Cannot serialize ValueTuple item of type '{itemType.FullName}'. " +
            $"Ensure the type is a basic type, implements ITagStruct, is marked with [TagStruct], " +
            $"or has a registered converter.");
    }

    private T DeserializeItem<T>(HyperTag itemTag)
    {
        if (itemTag.Type == TagDataType.Null)
            return default!;

        var itemType = typeof(T);
        var converter = _registry.GetConverter<T>();
        if (converter != null)
        {
            var result = converter.FromTag(itemTag);
            if (result != null)
                return result;
        }

        var basicType = TagTypeHelper.GetTagDataType(itemType);
        if (basicType != null)
        {
            if (itemTag.Type != basicType)
            {
                throw new TagAutoDeserializeException(
                    $"Type mismatch for ValueTuple item: expected {basicType}, got {itemTag.Type}");
            }
            return (T)itemTag.Value!;
        }

        var deserialized = TagAutoDeserializer.DeserializeObject<T>(itemTag, _registry, _options);
        if (deserialized != null)
            return deserialized;

        throw new TagAutoDeserializeException(
            $"Cannot deserialize ValueTuple item of type '{itemType.FullName}'");
    }
}

/// <summary>
/// Converter for ValueTuple<T1, T2, T3>
/// </summary>
public class ValueTupleConverter<T1, T2, T3> : ITagConverter<(T1, T2, T3)>
{
    private readonly TagConverterRegistry _registry;
    private readonly TagAutoOptions _options;

    public ValueTupleConverter() : this(null, null)
    {
    }

    public ValueTupleConverter(TagConverterRegistry? registry = null, TagAutoOptions? options = null)
    {
        _registry = registry ?? TagConverterRegistry.Default;
        _options = options ?? TagAutoOptions.Default;
    }

    public HyperTag? ToTag((T1, T2, T3) obj)
    {
        var tag = new HyperTag(TagDataType.Empty);
        tag.Entities.Add(SerializeItem(obj.Item1));
        tag.Entities.Add(SerializeItem(obj.Item2));
        tag.Entities.Add(SerializeItem(obj.Item3));
        return tag;
    }

    public (T1, T2, T3) FromTag(HyperTag? tag)
    {
        if (tag == null)
            throw new ArgumentNullException(nameof(tag));

        if (tag.Type == TagDataType.Null)
            throw new InvalidOperationException("Cannot convert null tag to non-nullable ValueTuple");

        if (tag.Type != TagDataType.Empty)
        {
            throw new TagAutoDeserializeException(
                $"Expected Empty type container for ValueTuple, but got {tag.Type}");
        }

        if (tag.Entities.Count != 3)
        {
            throw new TagAutoDeserializeException(
                $"Expected 3 entities for ValueTuple, but got {tag.Entities.Count}");
        }

        var item1 = DeserializeItem<T1>(tag.Entities[0]);
        var item2 = DeserializeItem<T2>(tag.Entities[1]);
        var item3 = DeserializeItem<T3>(tag.Entities[2]);
        return (item1, item2, item3);
    }

    private HyperTag SerializeItem<T>(T? item)
    {
        if (item == null)
            return new HyperTag(TagDataType.Null);

        var itemType = typeof(T);
        var converter = _registry.GetConverter<T>();
        if (converter != null)
        {
            var result = converter.ToTag(item);
            if (result != null)
                return result;
        }

        var basicType = TagTypeHelper.GetTagDataType(itemType);
        if (basicType != null)
            return new HyperTag(basicType.Value, item);

        var serialized = TagAutoSerializer.SerializeObject(item, _registry, _options);
        if (serialized != null)
            return serialized;

        throw new TagAutoSerializeException(
            $"Cannot serialize ValueTuple item of type '{itemType.FullName}'. " +
            $"Ensure the type is a basic type, implements ITagStruct, is marked with [TagStruct], " +
            $"or has a registered converter.");
    }

    private T DeserializeItem<T>(HyperTag itemTag)
    {
        if (itemTag.Type == TagDataType.Null)
            return default!;

        var itemType = typeof(T);
        var converter = _registry.GetConverter<T>();
        if (converter != null)
        {
            var result = converter.FromTag(itemTag);
            if (result != null)
                return result;
        }

        var basicType = TagTypeHelper.GetTagDataType(itemType);
        if (basicType != null)
        {
            if (itemTag.Type != basicType)
            {
                throw new TagAutoDeserializeException(
                    $"Type mismatch for ValueTuple item: expected {basicType}, got {itemTag.Type}");
            }
            return (T)itemTag.Value!;
        }

        var deserialized = TagAutoDeserializer.DeserializeObject<T>(itemTag, _registry, _options);
        if (deserialized != null)
            return deserialized;

        throw new TagAutoDeserializeException(
            $"Cannot deserialize ValueTuple item of type '{itemType.FullName}'");
    }
}

/// <summary>
/// Converter for ValueTuple<T1, T2, T3, T4>
/// </summary>
public class ValueTupleConverter<T1, T2, T3, T4> : ITagConverter<(T1, T2, T3, T4)>
{
    private readonly TagConverterRegistry _registry;
    private readonly TagAutoOptions _options;

    public ValueTupleConverter() : this(null, null)
    {
    }

    public ValueTupleConverter(TagConverterRegistry? registry = null, TagAutoOptions? options = null)
    {
        _registry = registry ?? TagConverterRegistry.Default;
        _options = options ?? TagAutoOptions.Default;
    }

    public HyperTag? ToTag((T1, T2, T3, T4) obj)
    {
        var tag = new HyperTag(TagDataType.Empty);
        tag.Entities.Add(SerializeItem(obj.Item1));
        tag.Entities.Add(SerializeItem(obj.Item2));
        tag.Entities.Add(SerializeItem(obj.Item3));
        tag.Entities.Add(SerializeItem(obj.Item4));
        return tag;
    }

    public (T1, T2, T3, T4) FromTag(HyperTag? tag)
    {
        if (tag == null)
            throw new ArgumentNullException(nameof(tag));

        if (tag.Type == TagDataType.Null)
            throw new InvalidOperationException("Cannot convert null tag to non-nullable ValueTuple");

        if (tag.Type != TagDataType.Empty)
        {
            throw new TagAutoDeserializeException(
                $"Expected Empty type container for ValueTuple, but got {tag.Type}");
        }

        if (tag.Entities.Count != 4)
        {
            throw new TagAutoDeserializeException(
                $"Expected 4 entities for ValueTuple, but got {tag.Entities.Count}");
        }

        var item1 = DeserializeItem<T1>(tag.Entities[0]);
        var item2 = DeserializeItem<T2>(tag.Entities[1]);
        var item3 = DeserializeItem<T3>(tag.Entities[2]);
        var item4 = DeserializeItem<T4>(tag.Entities[3]);
        return (item1, item2, item3, item4);
    }

    private HyperTag SerializeItem<T>(T? item)
    {
        if (item == null)
            return new HyperTag(TagDataType.Null);

        var itemType = typeof(T);
        var converter = _registry.GetConverter<T>();
        if (converter != null)
        {
            var result = converter.ToTag(item);
            if (result != null)
                return result;
        }

        var basicType = TagTypeHelper.GetTagDataType(itemType);
        if (basicType != null)
            return new HyperTag(basicType.Value, item);

        var serialized = TagAutoSerializer.SerializeObject(item, _registry, _options);
        if (serialized != null)
            return serialized;

        throw new TagAutoSerializeException(
            $"Cannot serialize ValueTuple item of type '{itemType.FullName}'. " +
            $"Ensure the type is a basic type, implements ITagStruct, is marked with [TagStruct], " +
            $"or has a registered converter.");
    }

    private T DeserializeItem<T>(HyperTag itemTag)
    {
        if (itemTag.Type == TagDataType.Null)
            return default!;

        var itemType = typeof(T);
        var converter = _registry.GetConverter<T>();
        if (converter != null)
        {
            var result = converter.FromTag(itemTag);
            if (result != null)
                return result;
        }

        var basicType = TagTypeHelper.GetTagDataType(itemType);
        if (basicType != null)
        {
            if (itemTag.Type != basicType)
            {
                throw new TagAutoDeserializeException(
                    $"Type mismatch for ValueTuple item: expected {basicType}, got {itemTag.Type}");
            }
            return (T)itemTag.Value!;
        }

        var deserialized = TagAutoDeserializer.DeserializeObject<T>(itemTag, _registry, _options);
        if (deserialized != null)
            return deserialized;

        throw new TagAutoDeserializeException(
            $"Cannot deserialize ValueTuple item of type '{itemType.FullName}'");
    }
}

/// <summary>
/// Converter for ValueTuple<T1, T2, T3, T4, T5>
/// </summary>
public class ValueTupleConverter<T1, T2, T3, T4, T5> : ITagConverter<(T1, T2, T3, T4, T5)>
{
    private readonly TagConverterRegistry _registry;
    private readonly TagAutoOptions _options;

    public ValueTupleConverter() : this(null, null)
    {
    }

    public ValueTupleConverter(TagConverterRegistry? registry = null, TagAutoOptions? options = null)
    {
        _registry = registry ?? TagConverterRegistry.Default;
        _options = options ?? TagAutoOptions.Default;
    }

    public HyperTag? ToTag((T1, T2, T3, T4, T5) obj)
    {
        var tag = new HyperTag(TagDataType.Empty);
        tag.Entities.Add(SerializeItem(obj.Item1));
        tag.Entities.Add(SerializeItem(obj.Item2));
        tag.Entities.Add(SerializeItem(obj.Item3));
        tag.Entities.Add(SerializeItem(obj.Item4));
        tag.Entities.Add(SerializeItem(obj.Item5));
        return tag;
    }

    public (T1, T2, T3, T4, T5) FromTag(HyperTag? tag)
    {
        if (tag == null)
            throw new ArgumentNullException(nameof(tag));

        if (tag.Type == TagDataType.Null)
            throw new InvalidOperationException("Cannot convert null tag to non-nullable ValueTuple");

        if (tag.Type != TagDataType.Empty)
        {
            throw new TagAutoDeserializeException(
                $"Expected Empty type container for ValueTuple, but got {tag.Type}");
        }

        if (tag.Entities.Count != 5)
        {
            throw new TagAutoDeserializeException(
                $"Expected 5 entities for ValueTuple, but got {tag.Entities.Count}");
        }

        var item1 = DeserializeItem<T1>(tag.Entities[0]);
        var item2 = DeserializeItem<T2>(tag.Entities[1]);
        var item3 = DeserializeItem<T3>(tag.Entities[2]);
        var item4 = DeserializeItem<T4>(tag.Entities[3]);
        var item5 = DeserializeItem<T5>(tag.Entities[4]);
        return (item1, item2, item3, item4, item5);
    }

    private HyperTag SerializeItem<T>(T? item)
    {
        if (item == null)
            return new HyperTag(TagDataType.Null);

        var itemType = typeof(T);
        var converter = _registry.GetConverter<T>();
        if (converter != null)
        {
            var result = converter.ToTag(item);
            if (result != null)
                return result;
        }

        var basicType = TagTypeHelper.GetTagDataType(itemType);
        if (basicType != null)
            return new HyperTag(basicType.Value, item);

        var serialized = TagAutoSerializer.SerializeObject(item, _registry, _options);
        if (serialized != null)
            return serialized;

        throw new TagAutoSerializeException(
            $"Cannot serialize ValueTuple item of type '{itemType.FullName}'. " +
            $"Ensure the type is a basic type, implements ITagStruct, is marked with [TagStruct], " +
            $"or has a registered converter.");
    }

    private T DeserializeItem<T>(HyperTag itemTag)
    {
        if (itemTag.Type == TagDataType.Null)
            return default!;

        var itemType = typeof(T);
        var converter = _registry.GetConverter<T>();
        if (converter != null)
        {
            var result = converter.FromTag(itemTag);
            if (result != null)
                return result;
        }

        var basicType = TagTypeHelper.GetTagDataType(itemType);
        if (basicType != null)
        {
            if (itemTag.Type != basicType)
            {
                throw new TagAutoDeserializeException(
                    $"Type mismatch for ValueTuple item: expected {basicType}, got {itemTag.Type}");
            }
            return (T)itemTag.Value!;
        }

        var deserialized = TagAutoDeserializer.DeserializeObject<T>(itemTag, _registry, _options);
        if (deserialized != null)
            return deserialized;

        throw new TagAutoDeserializeException(
            $"Cannot deserialize ValueTuple item of type '{itemType.FullName}'");
    }
}

/// <summary>
/// Converter for ValueTuple<T1, T2, T3, T4, T5, T6>
/// </summary>
public class ValueTupleConverter<T1, T2, T3, T4, T5, T6> : ITagConverter<(T1, T2, T3, T4, T5, T6)>
{
    private readonly TagConverterRegistry _registry;
    private readonly TagAutoOptions _options;

    public ValueTupleConverter() : this(null, null)
    {
    }

    public ValueTupleConverter(TagConverterRegistry? registry = null, TagAutoOptions? options = null)
    {
        _registry = registry ?? TagConverterRegistry.Default;
        _options = options ?? TagAutoOptions.Default;
    }

    public HyperTag? ToTag((T1, T2, T3, T4, T5, T6) obj)
    {
        var tag = new HyperTag(TagDataType.Empty);
        tag.Entities.Add(SerializeItem(obj.Item1));
        tag.Entities.Add(SerializeItem(obj.Item2));
        tag.Entities.Add(SerializeItem(obj.Item3));
        tag.Entities.Add(SerializeItem(obj.Item4));
        tag.Entities.Add(SerializeItem(obj.Item5));
        tag.Entities.Add(SerializeItem(obj.Item6));
        return tag;
    }

    public (T1, T2, T3, T4, T5, T6) FromTag(HyperTag? tag)
    {
        if (tag == null)
            throw new ArgumentNullException(nameof(tag));

        if (tag.Type == TagDataType.Null)
            throw new InvalidOperationException("Cannot convert null tag to non-nullable ValueTuple");

        if (tag.Type != TagDataType.Empty)
        {
            throw new TagAutoDeserializeException(
                $"Expected Empty type container for ValueTuple, but got {tag.Type}");
        }

        if (tag.Entities.Count != 6)
        {
            throw new TagAutoDeserializeException(
                $"Expected 6 entities for ValueTuple, but got {tag.Entities.Count}");
        }

        var item1 = DeserializeItem<T1>(tag.Entities[0]);
        var item2 = DeserializeItem<T2>(tag.Entities[1]);
        var item3 = DeserializeItem<T3>(tag.Entities[2]);
        var item4 = DeserializeItem<T4>(tag.Entities[3]);
        var item5 = DeserializeItem<T5>(tag.Entities[4]);
        var item6 = DeserializeItem<T6>(tag.Entities[5]);
        return (item1, item2, item3, item4, item5, item6);
    }

    private HyperTag SerializeItem<T>(T? item)
    {
        if (item == null)
            return new HyperTag(TagDataType.Null);

        var itemType = typeof(T);
        var converter = _registry.GetConverter<T>();
        if (converter != null)
        {
            var result = converter.ToTag(item);
            if (result != null)
                return result;
        }

        var basicType = TagTypeHelper.GetTagDataType(itemType);
        if (basicType != null)
            return new HyperTag(basicType.Value, item);

        var serialized = TagAutoSerializer.SerializeObject(item, _registry, _options);
        if (serialized != null)
            return serialized;

        throw new TagAutoSerializeException(
            $"Cannot serialize ValueTuple item of type '{itemType.FullName}'. " +
            $"Ensure the type is a basic type, implements ITagStruct, is marked with [TagStruct], " +
            $"or has a registered converter.");
    }

    private T DeserializeItem<T>(HyperTag itemTag)
    {
        if (itemTag.Type == TagDataType.Null)
            return default!;

        var itemType = typeof(T);
        var converter = _registry.GetConverter<T>();
        if (converter != null)
        {
            var result = converter.FromTag(itemTag);
            if (result != null)
                return result;
        }

        var basicType = TagTypeHelper.GetTagDataType(itemType);
        if (basicType != null)
        {
            if (itemTag.Type != basicType)
            {
                throw new TagAutoDeserializeException(
                    $"Type mismatch for ValueTuple item: expected {basicType}, got {itemTag.Type}");
            }
            return (T)itemTag.Value!;
        }

        var deserialized = TagAutoDeserializer.DeserializeObject<T>(itemTag, _registry, _options);
        if (deserialized != null)
            return deserialized;

        throw new TagAutoDeserializeException(
            $"Cannot deserialize ValueTuple item of type '{itemType.FullName}'");
    }
}

/// <summary>
/// Converter for ValueTuple<T1, T2, T3, T4, T5, T6, T7>
/// </summary>
public class ValueTupleConverter<T1, T2, T3, T4, T5, T6, T7> : ITagConverter<(T1, T2, T3, T4, T5, T6, T7)>
{
    private readonly TagConverterRegistry _registry;
    private readonly TagAutoOptions _options;

    public ValueTupleConverter() : this(null, null)
    {
    }

    public ValueTupleConverter(TagConverterRegistry? registry = null, TagAutoOptions? options = null)
    {
        _registry = registry ?? TagConverterRegistry.Default;
        _options = options ?? TagAutoOptions.Default;
    }

    public HyperTag? ToTag((T1, T2, T3, T4, T5, T6, T7) obj)
    {
        var tag = new HyperTag(TagDataType.Empty);
        tag.Entities.Add(SerializeItem(obj.Item1));
        tag.Entities.Add(SerializeItem(obj.Item2));
        tag.Entities.Add(SerializeItem(obj.Item3));
        tag.Entities.Add(SerializeItem(obj.Item4));
        tag.Entities.Add(SerializeItem(obj.Item5));
        tag.Entities.Add(SerializeItem(obj.Item6));
        tag.Entities.Add(SerializeItem(obj.Item7));
        return tag;
    }

    public (T1, T2, T3, T4, T5, T6, T7) FromTag(HyperTag? tag)
    {
        if (tag == null)
            throw new ArgumentNullException(nameof(tag));

        if (tag.Type == TagDataType.Null)
            throw new InvalidOperationException("Cannot convert null tag to non-nullable ValueTuple");

        if (tag.Type != TagDataType.Empty)
        {
            throw new TagAutoDeserializeException(
                $"Expected Empty type container for ValueTuple, but got {tag.Type}");
        }

        if (tag.Entities.Count != 7)
        {
            throw new TagAutoDeserializeException(
                $"Expected 7 entities for ValueTuple, but got {tag.Entities.Count}");
        }

        var item1 = DeserializeItem<T1>(tag.Entities[0]);
        var item2 = DeserializeItem<T2>(tag.Entities[1]);
        var item3 = DeserializeItem<T3>(tag.Entities[2]);
        var item4 = DeserializeItem<T4>(tag.Entities[3]);
        var item5 = DeserializeItem<T5>(tag.Entities[4]);
        var item6 = DeserializeItem<T6>(tag.Entities[5]);
        var item7 = DeserializeItem<T7>(tag.Entities[6]);
        return (item1, item2, item3, item4, item5, item6, item7);
    }

    private HyperTag SerializeItem<T>(T? item)
    {
        if (item == null)
            return new HyperTag(TagDataType.Null);

        var itemType = typeof(T);
        var converter = _registry.GetConverter<T>();
        if (converter != null)
        {
            var result = converter.ToTag(item);
            if (result != null)
                return result;
        }

        var basicType = TagTypeHelper.GetTagDataType(itemType);
        if (basicType != null)
            return new HyperTag(basicType.Value, item);

        var serialized = TagAutoSerializer.SerializeObject(item, _registry, _options);
        if (serialized != null)
            return serialized;

        throw new TagAutoSerializeException(
            $"Cannot serialize ValueTuple item of type '{itemType.FullName}'. " +
            $"Ensure the type is a basic type, implements ITagStruct, is marked with [TagStruct], " +
            $"or has a registered converter.");
    }

    private T DeserializeItem<T>(HyperTag itemTag)
    {
        if (itemTag.Type == TagDataType.Null)
            return default!;

        var itemType = typeof(T);
        var converter = _registry.GetConverter<T>();
        if (converter != null)
        {
            var result = converter.FromTag(itemTag);
            if (result != null)
                return result;
        }

        var basicType = TagTypeHelper.GetTagDataType(itemType);
        if (basicType != null)
        {
            if (itemTag.Type != basicType)
            {
                throw new TagAutoDeserializeException(
                    $"Type mismatch for ValueTuple item: expected {basicType}, got {itemTag.Type}");
            }
            return (T)itemTag.Value!;
        }

        var deserialized = TagAutoDeserializer.DeserializeObject<T>(itemTag, _registry, _options);
        if (deserialized != null)
            return deserialized;

        throw new TagAutoDeserializeException(
            $"Cannot deserialize ValueTuple item of type '{itemType.FullName}'");
    }
}

/// <summary>
/// Converter for ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>
/// TRest is typically another ValueTuple for larger tuple sizes (8+ elements)
/// </summary>
public class ValueTupleConverter<T1, T2, T3, T4, T5, T6, T7, TRest> : ITagConverter<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>>
    where TRest : struct
{
    private readonly TagConverterRegistry _registry;
    private readonly TagAutoOptions _options;

    public ValueTupleConverter() : this(null, null)
    {
    }

    public ValueTupleConverter(TagConverterRegistry? registry = null, TagAutoOptions? options = null)
    {
        _registry = registry ?? TagConverterRegistry.Default;
        _options = options ?? TagAutoOptions.Default;
    }

    public HyperTag? ToTag(ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> obj)
    {
        var tag = new HyperTag(TagDataType.Empty);
        tag.Entities.Add(SerializeItem(obj.Item1));
        tag.Entities.Add(SerializeItem(obj.Item2));
        tag.Entities.Add(SerializeItem(obj.Item3));
        tag.Entities.Add(SerializeItem(obj.Item4));
        tag.Entities.Add(SerializeItem(obj.Item5));
        tag.Entities.Add(SerializeItem(obj.Item6));
        tag.Entities.Add(SerializeItem(obj.Item7));
        tag.Entities.Add(SerializeItem(obj.Rest));
        return tag;
    }

    public ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> FromTag(HyperTag? tag)
    {
        if (tag == null)
            throw new ArgumentNullException(nameof(tag));

        if (tag.Type == TagDataType.Null)
            throw new InvalidOperationException("Cannot convert null tag to non-nullable ValueTuple");

        if (tag.Type != TagDataType.Empty)
        {
            throw new TagAutoDeserializeException(
                $"Expected Empty type container for ValueTuple, but got {tag.Type}");
        }

        if (tag.Entities.Count != 8)
        {
            throw new TagAutoDeserializeException(
                $"Expected 8 entities for ValueTuple, but got {tag.Entities.Count}");
        }

        var item1 = DeserializeItem<T1>(tag.Entities[0]);
        var item2 = DeserializeItem<T2>(tag.Entities[1]);
        var item3 = DeserializeItem<T3>(tag.Entities[2]);
        var item4 = DeserializeItem<T4>(tag.Entities[3]);
        var item5 = DeserializeItem<T5>(tag.Entities[4]);
        var item6 = DeserializeItem<T6>(tag.Entities[5]);
        var item7 = DeserializeItem<T7>(tag.Entities[6]);
        var rest = DeserializeItem<TRest>(tag.Entities[7]);
        return new ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(item1, item2, item3, item4, item5, item6, item7, rest);
    }

    private HyperTag SerializeItem<T>(T? item)
    {
        if (item == null)
            return new HyperTag(TagDataType.Null);

        var itemType = typeof(T);
        var converter = _registry.GetConverter<T>();
        if (converter != null)
        {
            var result = converter.ToTag(item);
            if (result != null)
                return result;
        }

        var basicType = TagTypeHelper.GetTagDataType(itemType);
        if (basicType != null)
            return new HyperTag(basicType.Value, item);

        var serialized = TagAutoSerializer.SerializeObject(item, _registry, _options);
        if (serialized != null)
            return serialized;

        throw new TagAutoSerializeException(
            $"Cannot serialize ValueTuple item of type '{itemType.FullName}'. " +
            $"Ensure the type is a basic type, implements ITagStruct, is marked with [TagStruct], " +
            $"or has a registered converter.");
    }

    private T DeserializeItem<T>(HyperTag itemTag)
    {
        if (itemTag.Type == TagDataType.Null)
            return default!;

        var itemType = typeof(T);
        var converter = _registry.GetConverter<T>();
        if (converter != null)
        {
            var result = converter.FromTag(itemTag);
            if (result != null)
                return result;
        }

        var basicType = TagTypeHelper.GetTagDataType(itemType);
        if (basicType != null)
        {
            if (itemTag.Type != basicType)
            {
                throw new TagAutoDeserializeException(
                    $"Type mismatch for ValueTuple item: expected {basicType}, got {itemTag.Type}");
            }
            return (T)itemTag.Value!;
        }

        var deserialized = TagAutoDeserializer.DeserializeObject<T>(itemTag, _registry, _options);
        if (deserialized != null)
            return deserialized;

        throw new TagAutoDeserializeException(
            $"Cannot deserialize ValueTuple item of type '{itemType.FullName}'");
    }
}

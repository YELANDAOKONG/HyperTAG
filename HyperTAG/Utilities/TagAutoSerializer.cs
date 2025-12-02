using System.Reflection;
using HyperTAG.Attributes;
using HyperTAG.Core;
using HyperTAG.Exceptions;
using HyperTAG.Interfaces;
using HyperTAG.Models;

namespace HyperTAG.Utilities;

public static class TagAutoSerializer
{
    /// <summary>
    /// Serializes an object to HyperTag using default options and registry
    /// </summary>
    public static HyperTag? SerializeObject(object? obj) =>
        SerializeObject(obj, TagConverterRegistry.Default, TagAutoOptions.Default);

    /// <summary>
    /// Serializes an object to HyperTag using custom registry
    /// </summary>
    public static HyperTag? SerializeObject(object? obj, TagConverterRegistry? registry) =>
        SerializeObject(obj, registry, TagAutoOptions.Default);

    /// <summary>
    /// Serializes an object to HyperTag using custom options
    /// </summary>
    public static HyperTag? SerializeObject(object? obj, TagAutoOptions? options) =>
        SerializeObject(obj, TagConverterRegistry.Default, options);

    /// <summary>
    /// Serializes an object to HyperTag using custom registry and options
    /// </summary>
    public static HyperTag? SerializeObject(object? obj, TagConverterRegistry? registry, TagAutoOptions? options)
    {
        if (obj == null) return null;
        
        registry ??= TagConverterRegistry.Default;
        options ??= TagAutoOptions.Default;
        
        try
        {
            return SerializeObjectInternal(obj, registry, options);
        }
        catch (Exception)
        {
            if (options.ThrowExceptions) throw;
            return null;
        }
    }
    
    private static HyperTag SerializeObjectInternal(object obj, TagConverterRegistry registry, TagAutoOptions options)
    {
        var type = obj.GetType();
        
        // Check for custom converter first
        var converter = registry.GetConverter(type);
        if (converter != null)
        {
            var method = converter.GetType().GetMethod("ToTag");
            if (method != null)
            {
                var result = method.Invoke(converter, new[] { obj }) as HyperTag;
                if (result != null)
                    return result;
            }
        }
        
        // Check if directly implements ITagStruct
        if (obj is ITagStruct tagStruct)
        {
            var result = tagStruct.ToTag();
            if (result != null)
                return result;
        }
        
        // Check if has TagStruct attribute
        if (type.GetCustomAttribute<TagStructAttribute>() == null)
        {
            throw new TagAutoSerializeException(
                $"Type '{type.FullName}' does not implement ITagStruct, is not marked with [TagStruct] attribute, " +
                $"and has no registered converter.");
        }
        
        // Auto-serialize
        // Auto-serialize
        var rootTag = new HyperTag(TagDataType.Empty);
        var members = TagTypeHelper.GetTagDataMembers(type);

        foreach (var member in members)
        {
            var tagDataAttr = member.GetCustomAttribute<TagDataAttribute>();
            var memberName = tagDataAttr?.CustomName ?? member.Name;
            var memberValue = TagTypeHelper.GetMemberValue(member, obj);
    
            var nameTag = new HyperTag(TagDataType.String, memberName);
            rootTag.Entities.Add(nameTag);
    
            var memberType = TagTypeHelper.GetMemberType(member);
            var valueTag = SerializeField(memberType, memberValue, registry, options);
            nameTag.Entities.Add(valueTag);
        }

        
        return rootTag;
    }
    
    private static HyperTag SerializeField(Type fieldType, object? fieldValue, TagConverterRegistry registry, TagAutoOptions options)
    {
        if (fieldValue == null)
        {
            return new HyperTag(TagDataType.Null);
        }
        
        // Check for custom converter first
        var converter = registry.GetConverter(fieldType);
        if (converter != null)
        {
            var method = converter.GetType().GetMethod("ToTag");
            if (method != null)
            {
                var tag = method.Invoke(converter, new[] { fieldValue }) as HyperTag;
                if (tag != null)
                {
                    if (options.NestedTags)
                    {
                        return tag;
                    }
                    else
                    {
                        var data = tag.Serialize(options.RecursionDepth);
                        return new HyperTag(TagDataType.Data, data ?? Array.Empty<byte>());
                    }
                }
            }
        }
        
        // Check if it's an array type
        if (fieldType.IsArray)
        {
            return SerializeArray(fieldType, fieldValue, registry, options);
        }
        
        // Handle basic types
        var basicTagDataType = TagTypeHelper.GetTagDataType(fieldType);
        if (basicTagDataType != null)
        {
            return new HyperTag(basicTagDataType.Value, fieldValue);
        }
        
        // Handle nested objects
        return SerializeNestedObject(fieldType, fieldValue, registry, options);
    }
    
    private static HyperTag SerializeArray(Type fieldType, object fieldValue, TagConverterRegistry registry, TagAutoOptions options)
    {
        var elementType = fieldType.GetElementType();
        if (elementType == null)
        {
            throw new TagAutoSerializeException(
                $"Cannot determine element type for array type '{fieldType.FullName}'");
        }
        
        // Check if it's a basic type array
        var tagDataType = TagTypeHelper.GetTagDataType(fieldType);
        if (tagDataType != null)
        {
            return new HyperTag(tagDataType.Value, fieldValue);
        }
        
        // Object array
        var array = (Array)fieldValue;
        var containerTag = new HyperTag(TagDataType.Empty);
        
        var elementConverter = registry.GetConverter(elementType);
        var elementImplementsITagStruct = typeof(ITagStruct).IsAssignableFrom(elementType);
        var elementHasTagStruct = elementType.GetCustomAttribute<TagStructAttribute>() != null;
        
        if (elementConverter == null && !elementImplementsITagStruct && !elementHasTagStruct)
        {
            throw new TagAutoSerializeException(
                $"Array element type '{elementType.FullName}' does not implement ITagStruct, " +
                $"is not marked with [TagStruct] attribute, and has no registered converter.");
        }
        
        foreach (var item in array)
        {
            if (item == null)
            {
                containerTag.Entities.Add(new HyperTag(TagDataType.Null));
                continue;
            }
            
            HyperTag? itemTag = null;
            
            if (elementConverter != null)
            {
                var method = elementConverter.GetType().GetMethod("ToTag");
                if (method != null)
                {
                    itemTag = method.Invoke(elementConverter, new[] { item }) as HyperTag;
                }
            }
            else if (item is ITagStruct tagStruct)
            {
                itemTag = tagStruct.ToTag();
            }
            else if (elementHasTagStruct)
            {
                itemTag = SerializeObjectInternal(item, registry, options);
            }
            
            if (itemTag == null)
            {
                containerTag.Entities.Add(new HyperTag(TagDataType.Null));
            }
            else if (options.NestedTags)
            {
                containerTag.Entities.Add(itemTag);
            }
            else
            {
                var data = itemTag.Serialize(options.RecursionDepth);
                containerTag.Entities.Add(new HyperTag(TagDataType.Data, data ?? Array.Empty<byte>()));
            }
        }
        
        return containerTag;
    }
    
    private static HyperTag SerializeNestedObject(Type fieldType, object fieldValue, TagConverterRegistry registry, TagAutoOptions options)
    {
        // Handle ITagStruct objects
        if (fieldValue is ITagStruct tagStructObj)
        {
            var nestedTag = tagStructObj.ToTag();
            if (nestedTag == null)
            {
                return new HyperTag(TagDataType.Null);
            }
            
            if (options.NestedTags)
            {
                return nestedTag;
            }
            else
            {
                var data = nestedTag.Serialize(options.RecursionDepth);
                return new HyperTag(TagDataType.Data, data ?? Array.Empty<byte>());
            }
        }
        
        // Handle TagStruct attribute marked objects
        if (fieldType.GetCustomAttribute<TagStructAttribute>() != null)
        {
            var nestedTag = SerializeObjectInternal(fieldValue, registry, options);
            
            if (options.NestedTags)
            {
                return nestedTag;
            }
            else
            {
                var data = nestedTag.Serialize(options.RecursionDepth);
                return new HyperTag(TagDataType.Data, data ?? Array.Empty<byte>());
            }
        }
        
        throw new TagAutoSerializeException(
            $"Field type '{fieldType.FullName}' is not supported for auto-serialization. " +
            $"Make sure the type is either a basic type, implements ITagStruct, is marked with [TagStruct], " +
            $"or has a registered converter.");
    }
}

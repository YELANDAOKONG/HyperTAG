using System.Reflection;
using HyperTAG.Attributes;
using HyperTAG.Core;
using HyperTAG.Exceptions;
using HyperTAG.Interfaces;
using HyperTAG.Models;

namespace HyperTAG.Utilities;

public static class TagAutoDeserializer
{
    /// <summary>
    /// Deserializes a HyperTag to an object using default options and registry
    /// </summary>
    public static T? DeserializeObject<T>(HyperTag? tag) =>
        DeserializeObject<T>(tag, TagConverterRegistry.Default, TagAutoOptions.Default);

    /// <summary>
    /// Deserializes a HyperTag to an object using custom registry
    /// </summary>
    public static T? DeserializeObject<T>(HyperTag? tag, TagConverterRegistry? registry) =>
        DeserializeObject<T>(tag, registry, TagAutoOptions.Default);

    /// <summary>
    /// Deserializes a HyperTag to an object using custom options
    /// </summary>
    public static T? DeserializeObject<T>(HyperTag? tag, TagAutoOptions? options) =>
        DeserializeObject<T>(tag, TagConverterRegistry.Default, options);

    /// <summary>
    /// Deserializes a HyperTag to an object using custom registry and options
    /// </summary>
    public static T? DeserializeObject<T>(HyperTag? tag, TagConverterRegistry? registry, TagAutoOptions? options)
    {
        if (tag == null) return default;
        
        registry ??= TagConverterRegistry.Default;
        options ??= TagAutoOptions.Default;
        
        try
        {
            return DeserializeObjectInternal<T>(tag, registry, options);
        }
        catch (Exception)
        {
            if (options.ThrowExceptions) throw;
            return default;
        }
    }
    
    private static T? DeserializeObjectInternal<T>(HyperTag tag, TagConverterRegistry registry, TagAutoOptions options)
    {
        var type = typeof(T);
        
        // Check for custom converter first
        var converter = registry.GetConverter<T>();
        if (converter != null)
        {
            return converter.FromTag(tag);
        }
        
        // Create instance
        var instance = Activator.CreateInstance(type);
        if (instance == null)
        {
            throw new TagAutoDeserializeException($"Cannot create instance of type '{type.FullName}'");
        }
        
        // Check if implements ITagStruct
        if (instance is ITagStruct tagStruct)
        {
            tagStruct.FromTag(tag);
            return (T)instance;
        }
        
        // Check if has TagStruct attribute
        if (type.GetCustomAttribute<TagStructAttribute>() == null)
        {
            throw new TagAutoDeserializeException(
                $"Type '{type.FullName}' does not implement ITagStruct, is not marked with [TagStruct] attribute, " +
                $"and has no registered converter.");
        }
        
        // Auto-deserialize
        var members = TagTypeHelper.GetTagDataMembers(type);
        var memberMap = members.ToDictionary(
            m => m.GetCustomAttribute<TagDataAttribute>()?.CustomName ?? m.Name,
            m => m
        );

        foreach (var nameTag in tag.Entities)
        {
            if (nameTag.Type != TagDataType.String || nameTag.Value == null)
                continue;
        
            var memberName = (string)nameTag.Value;
            if (!memberMap.TryGetValue(memberName, out var member))
                continue;
        
            if (nameTag.Entities.Count == 0)
                continue;
        
            var valueTag = nameTag.Entities[0];
            var memberType = TagTypeHelper.GetMemberType(member);
            var value = DeserializeField(memberType, valueTag, registry, options);
            TagTypeHelper.SetMemberValue(member, instance, value);
        }
        
        return (T)instance;
    }
    
    private static object? DeserializeField(Type fieldType, HyperTag valueTag, TagConverterRegistry registry, TagAutoOptions options)
    {
        if (valueTag.Type == TagDataType.Null)
        {
            return null;
        }
        
        // Check for custom converter first
        var converter = registry.GetConverter(fieldType);
        if (converter != null)
        {
            if (options.NestedTags)
            {
                // Directly use the nested tag
                var method = converter.GetType().GetMethod("FromTag");
                if (method != null)
                {
                    return method.Invoke(converter, new object?[] { valueTag });
                }
            }
            else if (valueTag.Type == TagDataType.Data && valueTag.Value is byte[] converterData)
            {
                // Deserialize from Data bytes
                var nestedTag = HyperTag.Deserialize(converterData, options.RecursionDepth);
                if (nestedTag != null)
                {
                    var method = converter.GetType().GetMethod("FromTag");
                    if (method != null)
                    {
                        return method.Invoke(converter, new object?[] { nestedTag });
                    }
                }
            }
        }
        
        // Check if it's an array type
        if (fieldType.IsArray)
        {
            return DeserializeArray(fieldType, valueTag, registry, options);
        }
        
        // Handle basic types
        var expectedBasicType = TagTypeHelper.GetTagDataType(fieldType);
        if (expectedBasicType != null)
        {
            if (valueTag.Type != expectedBasicType)
            {
                throw new TagAutoDeserializeException(
                    $"Type mismatch: expected {expectedBasicType} for type {fieldType.Name}, but got {valueTag.Type}");
            }
            return valueTag.Value;
        }
        
        // Handle nested objects
        return DeserializeNestedObject(fieldType, valueTag, registry, options);
    }
    
    private static object? DeserializeArray(Type fieldType, HyperTag valueTag, TagConverterRegistry registry, TagAutoOptions options)
    {
        var elementType = fieldType.GetElementType();
        if (elementType == null)
        {
            throw new TagAutoDeserializeException(
                $"Cannot determine element type for array type '{fieldType.FullName}'");
        }
        
        // Check if it's a basic type array
        var expectedType = TagTypeHelper.GetTagDataType(fieldType);
        if (expectedType != null)
        {
            if (valueTag.Type != expectedType)
            {
                throw new TagAutoDeserializeException(
                    $"Type mismatch: expected {expectedType} for type {fieldType.Name}, but got {valueTag.Type}");
            }
            return valueTag.Value;
        }
        
        // Object array
        if (valueTag.Type != TagDataType.Empty)
        {
            throw new TagAutoDeserializeException(
                $"Expected Empty type container for object array, but got {valueTag.Type}");
        }
        
        var elementConverter = registry.GetConverter(elementType);
        var elementImplementsITagStruct = typeof(ITagStruct).IsAssignableFrom(elementType);
        var elementHasTagStruct = elementType.GetCustomAttribute<TagStructAttribute>() != null;
        
        if (elementConverter == null && !elementImplementsITagStruct && !elementHasTagStruct)
        {
            throw new TagAutoDeserializeException(
                $"Array element type '{elementType.FullName}' does not implement ITagStruct, " +
                $"is not marked with [TagStruct] attribute, and has no registered converter.");
        }
        
        var list = new List<object?>();
        foreach (var itemTag in valueTag.Entities)
        {
            if (itemTag.Type == TagDataType.Null)
            {
                list.Add(null);
                continue;
            }
            
            object? item = null;
            
            if (options.NestedTags)
            {
                // Directly deserialize nested tag
                if (elementConverter != null)
                {
                    var method = elementConverter.GetType().GetMethod("FromTag");
                    if (method != null)
                    {
                        item = method.Invoke(elementConverter, new object?[] { itemTag });
                    }
                }
                else if (elementImplementsITagStruct)
                {
                    var instance = Activator.CreateInstance(elementType);
                    if (instance == null)
                    {
                        throw new TagAutoDeserializeException(
                            $"Cannot create instance of type '{elementType.FullName}'");
                    }
                    ((ITagStruct)instance).FromTag(itemTag);
                    item = instance;
                }
                else if (elementHasTagStruct)
                {
                    item = DeserializeObjectByType(elementType, itemTag, registry, options);
                }
            }
            else
            {
                // Deserialize from Data bytes
                if (itemTag.Type != TagDataType.Data || itemTag.Value is not byte[] data)
                {
                    throw new TagAutoDeserializeException(
                        $"Expected Data type for array element, but got {itemTag.Type}");
                }
                
                var nestedTag = HyperTag.Deserialize(data, options.RecursionDepth);
                if (nestedTag == null)
                {
                    list.Add(null);
                    continue;
                }
                
                if (elementConverter != null)
                {
                    var method = elementConverter.GetType().GetMethod("FromTag");
                    if (method != null)
                    {
                        item = method.Invoke(elementConverter, new object?[] { nestedTag });
                    }
                }
                else if (elementImplementsITagStruct)
                {
                    var instance = Activator.CreateInstance(elementType);
                    if (instance == null)
                    {
                        throw new TagAutoDeserializeException(
                            $"Cannot create instance of type '{elementType.FullName}'");
                    }
                    ((ITagStruct)instance).FromTag(nestedTag);
                    item = instance;
                }
                else if (elementHasTagStruct)
                {
                    item = DeserializeObjectByType(elementType, nestedTag, registry, options);
                }
            }
            
            list.Add(item);
        }
        
        var array = Array.CreateInstance(elementType, list.Count);
        for (int i = 0; i < list.Count; i++)
        {
            array.SetValue(list[i], i);
        }
        return array;
    }
    
    private static object? DeserializeNestedObject(Type fieldType, HyperTag valueTag, TagConverterRegistry registry, TagAutoOptions options)
    {
        if (options.NestedTags)
        {
            // Directly deserialize nested tag
            if (typeof(ITagStruct).IsAssignableFrom(fieldType))
            {
                var instance = Activator.CreateInstance(fieldType);
                if (instance == null)
                {
                    throw new TagAutoDeserializeException(
                        $"Cannot create instance of type '{fieldType.FullName}'");
                }
                ((ITagStruct)instance).FromTag(valueTag);
                return instance;
            }
            
            if (fieldType.GetCustomAttribute<TagStructAttribute>() != null)
            {
                return DeserializeObjectByType(fieldType, valueTag, registry, options);
            }
        }
        else
        {
            // Deserialize from Data bytes
            if (valueTag.Type == TagDataType.Data && valueTag.Value is byte[] objData)
            {
                var nestedTag = HyperTag.Deserialize(objData, options.RecursionDepth);
                if (nestedTag == null)
                {
                    return null;
                }
                
                if (typeof(ITagStruct).IsAssignableFrom(fieldType))
                {
                    var instance = Activator.CreateInstance(fieldType);
                    if (instance == null)
                    {
                        throw new TagAutoDeserializeException(
                            $"Cannot create instance of type '{fieldType.FullName}'");
                    }
                    ((ITagStruct)instance).FromTag(nestedTag);
                    return instance;
                }
                
                if (fieldType.GetCustomAttribute<TagStructAttribute>() != null)
                {
                    return DeserializeObjectByType(fieldType, nestedTag, registry, options);
                }
            }
        }
        
        throw new TagAutoDeserializeException(
            $"Cannot deserialize field of type '{fieldType.FullName}'. " +
            $"Make sure the type implements ITagStruct, is marked with [TagStruct], or has a registered converter.");
    }
    
    private static object? DeserializeObjectByType(Type type, HyperTag? tag, TagConverterRegistry registry, TagAutoOptions options)
    {
        var method = typeof(TagAutoDeserializer)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .FirstOrDefault(m => 
                m.Name == nameof(DeserializeObjectInternal) && 
                m.IsGenericMethodDefinition)
            ?.MakeGenericMethod(type);
        
        return method?.Invoke(null, new object?[] { tag, registry, options });
    }
}

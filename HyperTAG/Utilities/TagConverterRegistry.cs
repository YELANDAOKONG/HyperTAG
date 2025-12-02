using System.Collections.Concurrent;
using HyperTAG.Interfaces;

namespace HyperTAG.Utilities;

/// <summary>
/// Registry for custom tag converters. Supports both global default instance and custom instances.
/// </summary>
public class TagConverterRegistry
{
    private readonly ConcurrentDictionary<Type, object> _converters = new();

    /// <summary>
    /// Gets the global default converter registry instance
    /// </summary>
    public static TagConverterRegistry Default { get; } = new();

    /// <summary>
    /// Registers a converter for a specific type
    /// </summary>
    public void Register<T>(ITagConverter<T> converter)
    {
        if (converter == null)
            throw new ArgumentNullException(nameof(converter));
            
        _converters[typeof(T)] = converter;
    }

    /// <summary>
    /// Unregisters a converter for a specific type
    /// </summary>
    public void Unregister<T>()
    {
        _converters.TryRemove(typeof(T), out _);
    }

    /// <summary>
    /// Unregisters a converter for a specific type
    /// </summary>
    public void Unregister(Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));
            
        _converters.TryRemove(type, out _);
    }

    /// <summary>
    /// Gets a converter for a specific type
    /// </summary>
    public ITagConverter<T>? GetConverter<T>()
    {
        if (_converters.TryGetValue(typeof(T), out var converter))
        {
            return converter as ITagConverter<T>;
        }
        return null;
    }

    /// <summary>
    /// Checks if a converter is registered for a specific type
    /// </summary>
    public bool HasConverter<T>()
    {
        return _converters.ContainsKey(typeof(T));
    }

    /// <summary>
    /// Checks if a converter is registered for a specific type
    /// </summary>
    public bool HasConverter(Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));
            
        return _converters.ContainsKey(type);
    }

    /// <summary>
    /// Gets a converter for a specific type (non-generic)
    /// </summary>
    internal object? GetConverter(Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));
            
        _converters.TryGetValue(type, out var converter);
        return converter;
    }

    /// <summary>
    /// Clears all registered converters in this registry
    /// </summary>
    public void Clear()
    {
        _converters.Clear();
    }

    /// <summary>
    /// Gets the count of registered converters
    /// </summary>
    public int Count => _converters.Count;
}

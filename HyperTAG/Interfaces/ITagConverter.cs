using HyperTAG.Core;

namespace HyperTAG.Interfaces;

/// <summary>
/// Interface for custom type converters
/// </summary>
/// <typeparam name="T">The type to convert</typeparam>
public interface ITagConverter<T>
{
    /// <summary>
    /// Converts an object to HyperTag
    /// </summary>
    HyperTag? ToTag(T? obj);
    
    /// <summary>
    /// Converts a HyperTag to an object
    /// </summary>
    T? FromTag(HyperTag? tag);
}
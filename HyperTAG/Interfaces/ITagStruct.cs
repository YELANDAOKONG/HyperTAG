using HyperTAG.Core;

namespace HyperTAG.Interfaces;

/// <summary>
/// Interface for types that can be converted to/from HyperTag
/// </summary>
public interface ITagStruct
{
    /// <summary>
    /// Converts this object to a HyperTag
    /// </summary>
    HyperTag? ToTag();
    
    /// <summary>
    /// Populates this object from a HyperTag
    /// </summary>
    void FromTag(HyperTag? tag);
}
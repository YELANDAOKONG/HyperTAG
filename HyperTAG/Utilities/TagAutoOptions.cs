namespace HyperTAG.Utilities;

/// <summary>
/// Configuration options for TagAutoSerializer and TagAutoDeserializer
/// </summary>
public class TagAutoOptions
{
    /// <summary>
    /// Gets the default options instance with recommended settings
    /// </summary>
    public static TagAutoOptions Default { get; } = new();

    /// <summary>
    /// When true, nested objects are embedded directly as child entities.
    /// When false, nested objects are serialized to Data type.
    /// Default: true (nested tags for better structure and performance)
    /// </summary>
    public bool NestedTags { get; set; } = true;

    /// <summary>
    /// Maximum recursion depth for serialization/deserialization
    /// </summary>
    public int RecursionDepth { get; set; } = 8192;
    
    public bool ThrowExceptions { get; set; } = false;
}
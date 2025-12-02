namespace HyperTAG.Attributes;

/// <summary>
/// Marks a field or property to be included in Tag serialization
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class TagDataAttribute : Attribute
{
    /// <summary>
    /// Custom name for the member in the serialized Tag. If null, uses the member name.
    /// </summary>
    public string? CustomName { get; }

    public TagDataAttribute()
    {
    }

    public TagDataAttribute(string customName)
    {
        CustomName = customName;
    }
}
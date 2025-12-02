using HyperTAG.Core;
using HyperTAG.Interfaces;
using HyperTAG.Models;

namespace HyperTAG.Converters;

/// <summary>
/// Converter for DateTime. Uses ToBinary/FromBinary to preserve Kind information.
/// </summary>
public class DateTimeConverter : ITagConverter<DateTime>
{
    public HyperTag? ToTag(DateTime obj)
    {
        // Use ToBinary() to preserve DateTimeKind (Utc/Local/Unspecified)
        return new HyperTag(TagDataType.Long, obj.ToBinary());
    }

    public DateTime FromTag(HyperTag? tag)
    {
        if (tag == null)
            throw new ArgumentNullException(nameof(tag));
            
        if (tag.Type == TagDataType.Null)
            throw new InvalidOperationException("Cannot convert null tag to non-nullable DateTime");
            
        if (tag.Type != TagDataType.Long)
            throw new InvalidOperationException($"Expected Long type, but got {tag.Type}");

        if (tag.Value is not long binary)
            throw new InvalidOperationException("Invalid DateTime data");

        return DateTime.FromBinary(binary);
    }
}
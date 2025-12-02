using HyperTAG.Core;
using HyperTAG.Interfaces;
using HyperTAG.Models;

namespace HyperTAG.Converters;

/// <summary>
/// Converter for TimeOnly (available in .NET 6+). Stores as ticks (long).
/// </summary>
public class TimeOnlyConverter : ITagConverter<TimeOnly>
{
    public HyperTag? ToTag(TimeOnly obj)
    {
        return new HyperTag(TagDataType.Long, obj.Ticks);
    }

    public TimeOnly FromTag(HyperTag? tag)
    {
        if (tag == null)
            throw new ArgumentNullException(nameof(tag));
            
        if (tag.Type == TagDataType.Null)
            throw new InvalidOperationException("Cannot convert null tag to non-nullable TimeOnly");
            
        if (tag.Type != TagDataType.Long)
            throw new InvalidOperationException($"Expected Long type, but got {tag.Type}");

        if (tag.Value is not long ticks)
            throw new InvalidOperationException("Invalid TimeOnly data");

        return new TimeOnly(ticks);
    }
}

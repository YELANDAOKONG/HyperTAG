using HyperTAG.Core;
using HyperTAG.Interfaces;
using HyperTAG.Models;

namespace HyperTAG.Converters;

/// <summary>
/// Converter for TimeSpan. Stores as ticks (long).
/// </summary>
public class TimeSpanConverter : ITagConverter<TimeSpan>
{
    public HyperTag? ToTag(TimeSpan obj)
    {
        return new HyperTag(TagDataType.Long, obj.Ticks);
    }

    public TimeSpan FromTag(HyperTag? tag)
    {
        if (tag == null)
            throw new ArgumentNullException(nameof(tag));
            
        if (tag.Type == TagDataType.Null)
            throw new InvalidOperationException("Cannot convert null tag to non-nullable TimeSpan");
            
        if (tag.Type != TagDataType.Long)
            throw new InvalidOperationException($"Expected Long type, but got {tag.Type}");

        if (tag.Value is not long ticks)
            throw new InvalidOperationException("Invalid TimeSpan data");

        return TimeSpan.FromTicks(ticks);
    }
}
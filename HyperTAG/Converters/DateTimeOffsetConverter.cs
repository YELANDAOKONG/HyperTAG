using HyperTAG.Core;
using HyperTAG.Interfaces;
using HyperTAG.Models;

namespace HyperTAG.Converters;

/// <summary>
/// Converter for DateTimeOffset. Stores both ticks and offset separately.
/// </summary>
public class DateTimeOffsetConverter : ITagConverter<DateTimeOffset>
{
    public HyperTag? ToTag(DateTimeOffset obj)
    {
        var root = new HyperTag(TagDataType.Empty);
        
        // Ticks
        var ticksNameTag = new HyperTag(TagDataType.String, "Ticks");
        var ticksValueTag = new HyperTag(TagDataType.Long, obj.Ticks);
        ticksNameTag.Entities.Add(ticksValueTag);
        root.Entities.Add(ticksNameTag);
        
        // Offset in ticks
        var offsetNameTag = new HyperTag(TagDataType.String, "Offset");
        var offsetValueTag = new HyperTag(TagDataType.Long, obj.Offset.Ticks);
        offsetNameTag.Entities.Add(offsetValueTag);
        root.Entities.Add(offsetNameTag);
        
        return root;
    }

    public DateTimeOffset FromTag(HyperTag? tag)
    {
        if (tag == null)
            throw new ArgumentNullException(nameof(tag));
            
        if (tag.Type == TagDataType.Null)
            throw new InvalidOperationException("Cannot convert null tag to non-nullable DateTimeOffset");
            
        if (tag.Type != TagDataType.Empty)
            throw new InvalidOperationException($"Expected Empty type container, but got {tag.Type}");

        long? ticks = null;
        long? offsetTicks = null;

        foreach (var nameTag in tag.Entities)
        {
            if (nameTag.Type != TagDataType.String || nameTag.Value == null)
                continue;

            var fieldName = (string)nameTag.Value;
            if (nameTag.Entities.Count == 0)
                continue;

            var valueTag = nameTag.Entities[0];

            switch (fieldName)
            {
                case "Ticks":
                    if (valueTag.Type == TagDataType.Long && valueTag.Value != null)
                        ticks = (long)valueTag.Value;
                    break;

                case "Offset":
                    if (valueTag.Type == TagDataType.Long && valueTag.Value != null)
                        offsetTicks = (long)valueTag.Value;
                    break;
            }
        }

        if (!ticks.HasValue || !offsetTicks.HasValue)
            throw new InvalidOperationException("Invalid DateTimeOffset data: missing required fields");

        return new DateTimeOffset(ticks.Value, TimeSpan.FromTicks(offsetTicks.Value));
    }
}

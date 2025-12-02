using HyperTAG.Core;
using HyperTAG.Interfaces;
using HyperTAG.Models;

namespace HyperTAG.Converters;

/// <summary>
/// Converter for DateOnly (available in .NET 6+). Stores as DayNumber (int).
/// </summary>
public class DateOnlyConverter : ITagConverter<DateOnly>
{
    public HyperTag? ToTag(DateOnly obj)
    {
        return new HyperTag(TagDataType.Int, obj.DayNumber);
    }

    public DateOnly FromTag(HyperTag? tag)
    {
        if (tag == null)
            throw new ArgumentNullException(nameof(tag));
            
        if (tag.Type == TagDataType.Null)
            throw new InvalidOperationException("Cannot convert null tag to non-nullable DateOnly");
            
        if (tag.Type != TagDataType.Int)
            throw new InvalidOperationException($"Expected Int type, but got {tag.Type}");

        if (tag.Value is not int dayNumber)
            throw new InvalidOperationException("Invalid DateOnly data");

        return DateOnly.FromDayNumber(dayNumber);
    }
}

using HyperTAG.Core;
using HyperTAG.Interfaces;
using HyperTAG.Models;

namespace HyperTAG.Converters;

/// <summary>
/// Generic converter for Nullable<T> types. Wraps an inner converter for the non-nullable type.
/// </summary>
public class NullableConverter<T> : ITagConverter<T?> where T : struct
{
    private readonly ITagConverter<T> _innerConverter;

    public NullableConverter(ITagConverter<T> innerConverter)
    {
        _innerConverter = innerConverter ?? throw new ArgumentNullException(nameof(innerConverter));
    }

    public HyperTag? ToTag(T? obj)
    {
        if (obj == null)
            return null;
            
        return _innerConverter.ToTag(obj.Value);
    }

    public T? FromTag(HyperTag? tag)
    {
        if (tag == null)
            return null;
            
        if (tag.Type == TagDataType.Null)
            return null;
            
        return _innerConverter.FromTag(tag);
    }
}
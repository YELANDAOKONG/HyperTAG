using HyperTAG.Core;
using HyperTAG.Interfaces;
using HyperTAG.Models;

namespace HyperTAG.Converters;

public class GuidConverter : ITagConverter<Guid>
{
    public HyperTag? ToTag(Guid obj)
    {
        var bytes = obj.ToByteArray();
        return new HyperTag(TagDataType.Data, bytes);
    }

    public Guid FromTag(HyperTag? tag)
    {
        if (tag == null)
            throw new ArgumentNullException(nameof(tag));
            
        if (tag.Type == TagDataType.Null)
            throw new InvalidOperationException("Cannot convert null tag to non-nullable Guid");
            
        if (tag.Type != TagDataType.Data)
            throw new InvalidOperationException($"Expected Data type, but got {tag.Type}");

        if (tag.Value is not byte[] bytes || bytes.Length != 16)
            throw new InvalidOperationException("Invalid Guid data: expected 16 bytes");

        return new Guid(bytes);
    }
}
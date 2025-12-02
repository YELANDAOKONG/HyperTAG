using HyperTAG.Core;
using HyperTAG.Interfaces;
using HyperTAG.Models;

namespace HyperTAG.Converters;

public class HyperTagConverter : ITagConverter<HyperTag>
{
    public HyperTag? ToTag(HyperTag? obj)
    {
        if (obj == null)
            return null;
            
        var bytes = obj.Serialize();
        if (bytes == null)
            return null;
            
        return new HyperTag(TagDataType.Data, bytes);
    }

    public HyperTag? FromTag(HyperTag? tag)
    {
        if (tag == null)
            return null;
            
        if (tag.Type == TagDataType.Null)
            return null;
            
        if (tag.Type != TagDataType.Data)
            return null;

        if (tag.Value is not byte[] bytes)
            return null;

        return HyperTag.Deserialize(bytes);
    }
}
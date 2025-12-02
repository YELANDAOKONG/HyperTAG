using HyperTAG.Models;
using Newtonsoft.Json;

namespace HyperTAG.Structs;

[Serializable, JsonObject]
public class TagStruct
{
    public TagDataType Type { get; set; } = TagDataType.Empty;
    public object? Value { get; set; } = null;
    public List<TagStruct> Entities { get; set; } = new List<TagStruct>();

    public TagStruct() { }

    public TagStruct(TagDataType type, object? value)
    {
        Type = type;
        Value = value;
    }
}
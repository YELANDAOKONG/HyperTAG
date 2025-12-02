using HyperTAG.Models;
using HyperTAG.Structs;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace HyperTAG.Utilities;

/// <summary>
/// Provides BSON deserialization for TagStruct
/// </summary>
public static class TagBsonDeserializer
{
    public static TagStruct? Deserialize(byte[] bson, bool throwExceptions = false)
    {
        try
        {
            if (bson == null)
            {
                throw new ArgumentNullException(nameof(bson), "BSON data cannot be null");
            }
            
            if (bson.Length == 0)
            {
                return new TagStruct(TagDataType.Empty, null);
            }
            
            using var stream = new MemoryStream(bson);
            using var reader = new BsonDataReader(stream);
            
            var serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include
            };
            
            return serializer.Deserialize<TagStruct>(reader);
        }
        catch (Exception)
        {
            if (throwExceptions) throw;
            return null;
        }
    }
}
using HyperTAG.Structs;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace HyperTAG.Utilities;

/// <summary>
/// Provides BSON serialization for TagStruct
/// </summary>
public static class TagBsonSerializer
{
    public static byte[]? Serialize(TagStruct tagStruct, bool throwExceptions = false)
    {
        try
        {
            using var stream = new MemoryStream();
            using var writer = new BsonDataWriter(stream);
            
            var serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include
            };
            
            serializer.Serialize(writer, tagStruct);
            return stream.ToArray();
        }
        catch (Exception)
        {
            if (throwExceptions) throw;
            return null;
        }
    }
}
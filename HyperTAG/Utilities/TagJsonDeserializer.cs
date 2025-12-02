using HyperTAG.Structs;
using Newtonsoft.Json;

namespace HyperTAG.Utilities;

/// <summary>
/// Provides JSON deserialization for TagStruct
/// </summary>
public static class TagJsonDeserializer
{
    public static TagStruct? Deserialize(string json, bool throwExceptions = false)
    {
        try
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include
            };
            
            return JsonConvert.DeserializeObject<TagStruct>(json, settings);
        }
        catch (Exception)
        {
            if (throwExceptions) throw;
            return null;
        }
    }
}
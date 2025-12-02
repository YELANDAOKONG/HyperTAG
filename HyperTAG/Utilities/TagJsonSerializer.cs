using HyperTAG.Structs;
using Newtonsoft.Json;

namespace HyperTAG.Utilities;

/// <summary>
/// Provides JSON serialization for TagStruct
/// </summary>
public static class TagJsonSerializer
{
    public static string? Serialize(TagStruct tagStruct, bool throwExceptions = false)
    {
        try
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include
            };
            
            return JsonConvert.SerializeObject(tagStruct, settings);
        }
        catch (Exception)
        {
            if (throwExceptions) throw;
            return null;
        }
    }
}
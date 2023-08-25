using System.Text.Json;

namespace LockerService.Application.Common.Utils;

public class JsonSerializerUtils
{
    public static JsonSerializerOptions GetGlobalJsonSerializerOptions()
    {
        return new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(), },
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
    }
   
}
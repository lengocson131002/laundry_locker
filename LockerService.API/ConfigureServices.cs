using System.Text.Json.Serialization;
using LockerService.API.Filters;

namespace LockerService.API;

public static class ConfigureServices
{
    public static IServiceCollection ConfigureApiServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers(opt => opt.Filters.Add(typeof(TrimPropertiesActionFilter)))
            .ConfigureApiBehaviorOptions(opt => opt.SuppressModelStateInvalidFilter = true)
            .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });
        
        return services;
    }
}
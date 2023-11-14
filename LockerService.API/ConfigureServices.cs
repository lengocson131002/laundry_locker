using System.Text.Json.Serialization;
using LockerService.API.Filters;

namespace LockerService.API;

/// <summary>
/// 
/// </summary>
public static class ConfigureServices
{
    /// <summary>
    /// Configure services
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureApiServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers()
            .ConfigureApiBehaviorOptions(opt => opt.SuppressModelStateInvalidFilter = true)
            .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });
        
        return services;
    }
}
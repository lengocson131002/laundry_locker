using System.Reflection;
using LockerService.Application.Common.Behaviours;
using LockerService.Application.Common.Mappings;
using Microsoft.Extensions.DependencyInjection;

namespace LockerService.Application;

public static class ConfigureServices
{
    public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Auto mapper
        services.AddAutoMapper(typeof(MappingProfiles));
        services.AddAutoMapper(typeof(RabbitMqEventMappings));
        
        // FluentAPI validation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        // MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationErrorBehaviour<,>));
        });

        return services;
    }
}
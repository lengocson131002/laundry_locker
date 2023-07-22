using System.Reflection;
using LockerService.Application.Common.Behaviours;
using LockerService.Application.Common.Mappings;
using LockerService.Application.EventBus.RabbitMq.Consumers.Lockers;
using LockerService.Application.EventBus.RabbitMq.Consumers.Orders;
using LockerService.Application.EventBus.RabbitMq.Events.Lockers;
using MassTransit;
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
        
        
        // Rabbit MQ
        services.AddMassTransit(config =>
        {
            // Define RabbitMQ consumer
            // Order events consumers
            config.AddConsumer<OrderCreatedConsumer>();
            config.AddConsumer<OrderReturnedConsumer>();
            
            // Locker events consumers
            config.AddConsumer<LockerConnectedConsumer>();
            config.AddConsumer<LockerDisconnectedConsumer>();
            config.AddConsumer<LockerOverloadedConsumer>();
            config.AddConsumer<LockerUpdatedInfoConsumer>();
            config.AddConsumer<LockerUpdatedStatusConsumer>();
            
            config.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(
                        configuration["RabbitMQ:Host"],
                        ushort.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
                        "/",
                        h =>
                        {
                            h.Username(configuration["RabbitMQ:Username"]);
                            h.Password(configuration["RabbitMQ:Password"]);
                        }
                    );
                cfg.ConfigureEndpoints(ctx);
            });
            
            config.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("locker-service", false));
        });
        
        return services;
    }
}
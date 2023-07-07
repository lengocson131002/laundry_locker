using System.Reflection;
using LockerService.Application.Common.Behaviours;
using LockerService.Application.Common.Mappings;
using LockerService.Application.EventBus.RabbitMq.Consumers;
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
            config.AddConsumer<OrderCreatedConsumer>();
            config.AddConsumer<OrderReturnedConsumer>();
            
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
                
                cfg.ReceiveEndpoint("order.created.locker-service", e =>
                {
                    e.PrefetchCount = 20;
                    e.ConfigureConsumer<OrderCreatedConsumer>(ctx);
                });
                
                cfg.ReceiveEndpoint("order.returned.locker-service", e =>
                {
                    e.PrefetchCount = 20;
                    e.ConfigureConsumer<OrderReturnedConsumer>(ctx);
                });
            });
        });
        
        return services;
    }
}
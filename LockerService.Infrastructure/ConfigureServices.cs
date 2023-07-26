using System.Text;
using LockerService.Application.Common.Security;
using LockerService.Application.Common.Services;
using LockerService.Application.Common.Services.Notification;
using LockerService.Application.EventBus.RabbitMq;
using LockerService.Infrastructure.Common.Constants;
using LockerService.Infrastructure.EventBus.Mqtt;
using LockerService.Infrastructure.EventBus.RabbitMq;
using LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Lockers;
using LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Orders;
using LockerService.Infrastructure.Persistence;
using LockerService.Infrastructure.Repositories;
using LockerService.Infrastructure.Services;
using LockerService.Infrastructure.Services.Notification;
using LockerService.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Quartz;

namespace LockerService.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddHttpContextAccessor();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICurrentPrincipalService, CurrentPrincipalService>();
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
        });

        // Initialize database and seeding
        services.AddScoped<ApplicationDbInitializer>();
        using var scope = services.BuildServiceProvider().CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbInitializer>();
        
        if (IsDevelopment())
        {
            initializer.InitializeAsync().Wait();
            initializer.SeedAsync().Wait();
        }
        
        // Caching
        services.AddOptions<RedisSettings>()
            .BindConfiguration(RedisSettings.ConfigSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<RedisSettings>>().Value);
            
        services.AddSingleton<ICacheService, CacheService>();
        services.AddStackExchangeRedisCache(redisOptions =>
        {
            using var sc = services.BuildServiceProvider().CreateScope();
            var settings = sc.ServiceProvider.GetRequiredService<RedisSettings>();
            var connection = $"{settings.Host}:{settings.Port},password={settings.Password}";
            redisOptions.Configuration = connection;
        });
            
        // Storage
        services.AddOptions<AwsS3Settings>()
            .BindConfiguration(AwsS3Settings.ConfigSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<AwsS3Settings>>().Value);

        services.AddScoped<IStorageService, StorageService>();
        
        // Jwt service
        services.AddOptions<JwtSettings>()
            .BindConfiguration(JwtSettings.ConfigSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);
            
        services.AddScoped<IJwtService, JwtService>();
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = TokenCookieConstants.AccessTokenCookie;
            })
            .AddJwtBearer(options =>
            {
                using var sc = services.BuildServiceProvider().CreateScope();
                var settings = sc.ServiceProvider.GetRequiredService<JwtSettings>();
                // Validate JWT Token
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = settings.Issuer,
                    ValidAudience = settings.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key))
                };
                options.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies[TokenCookieConstants.AccessTokenCookie];
                        return Task.CompletedTask;
                    }
                };
            });
        // ApiKey 
        services.AddOptions<ApiKeySettings>()
            .BindConfiguration(ApiKeySettings.ConfigSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton(sp => sp.GetRequiredService<IOptions<ApiKeySettings>>().Value);
        
        // Security
        services.AddScoped<ISecurityService, AesSecurityService>();
        
        // Mqtt
        services.AddOptions<MqttSettings>()
            .BindConfiguration(MqttSettings.ConfigSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton(sp => sp.GetRequiredService<IOptions<MqttSettings>>().Value);
        
        services.AddSingleton<MqttClientService>();
        services.AddScoped<IMqttBus, MqttBus>();
        services.AddHostedService<MqttClientService>(sp => sp.GetRequiredService<MqttClientService>());
        services.AddSingleton<MqttClientServiceProvider>(sp =>
        {
            var mqttClientService = sp.GetRequiredService<MqttClientService>();
            var mqttClientServiceProvider = new MqttClientServiceProvider(mqttClientService);
            return mqttClientServiceProvider;
        });
        
        // Rabbit MQ
        services.AddOptions<RabbitMqSettings>()
            .BindConfiguration(RabbitMqSettings.ConfigSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton(sp => sp.GetRequiredService<IOptions<RabbitMqSettings>>().Value);
        services.AddMassTransit(config =>
        {
            // Define RabbitMQ consumer
            // Order events consumers
            config.AddConsumer<OrderCreatedConsumer>();
            config.AddConsumer<OrderProcessingConsumer>();
            config.AddConsumer<OrderConfirmedConsumer>();
            config.AddConsumer<OrderReturnedConsumer>();
            config.AddConsumer<OrderCompletedConsumer>();
            config.AddConsumer<OrderCanceledConsumer>();
            
            // Locker events consumers
            config.AddConsumer<LockerConnectedConsumer>();
            config.AddConsumer<LockerDisconnectedConsumer>();
            config.AddConsumer<LockerOverloadedConsumer>();
            config.AddConsumer<LockerUpdatedInfoConsumer>();
            config.AddConsumer<LockerUpdatedStatusConsumer>();
            
            using var sc = services.BuildServiceProvider().CreateScope();
            var settings = sc.ServiceProvider.GetRequiredService<RabbitMqSettings>();
            config.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(
                        settings.Host,
                        settings.Port,
                        "/",
                        h =>
                        {
                            h.Username(settings.Username);
                            h.Password(settings.Password);
                        }
                    );
                cfg.ConfigureEndpoints(ctx);
            });
            
            config.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(settings.EndpointNamePrefix, false));
            services.AddScoped<IRabbitMqBus, RabbitMqBus>();
        });
        
        // Quartz
        services.AddOptions<QuartzSettings>()
            .BindConfiguration(QuartzSettings.ConfigSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton(sp => sp.GetRequiredService<IOptions<QuartzSettings>>().Value);
        
        services.AddScoped<IOrderTimeoutService, OrderTimeoutService>();
        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();
            q.UsePersistentStore(options =>
            {
                using var sc = services.BuildServiceProvider().CreateScope();
                var settings = sc.ServiceProvider.GetRequiredService<QuartzSettings>();
                // options.UseProperties = true;
                options.UsePostgres(settings.ConnectionString);
                
                options.UseJsonSerializer();
            });
        });

        services.AddQuartzServer(options =>
        {
            // options.WaitForJobsToComplete = true;
        });
        
        // Fee
        services.AddScoped<IFeeService, FeeService>();
        
        // Address
        services.AddHostedService<ImportAddressService>();
        
        // Notification
        services.AddScoped<ISmsNotificationService, TwilioNotificationService>();
        
        // Payment
        services.AddScoped<IPaymentService, PaymentService>();
        
        // Notification
        services.AddOptions<TwilioSettings>()
            .BindConfiguration(TwilioSettings.ConfigSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<TwilioSettings>>().Value);
       
        return services;
    }

    private static bool IsDevelopment()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        return environment == Environments.Development;
    }
}
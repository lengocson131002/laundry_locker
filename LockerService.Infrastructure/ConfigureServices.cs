using System.Text;
using System.Text.Json.Serialization;
using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Common.Security;
using LockerService.Application.Common.Services.Payments;
using LockerService.Application.EventBus.RabbitMq;
using LockerService.Infrastructure.EventBus.Mqtt;
using LockerService.Infrastructure.EventBus.RabbitMq;
using LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Accounts;
using LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Lockers;
using LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Orders;
using LockerService.Infrastructure.Persistence.Contexts;
using LockerService.Infrastructure.Persistence.Data;
using LockerService.Infrastructure.Persistence.Interceptors;
using LockerService.Infrastructure.Persistence.Repositories;
using LockerService.Infrastructure.Scheduler;
using LockerService.Infrastructure.Services;
using LockerService.Infrastructure.Services.Notifications;
using LockerService.Infrastructure.Services.Notifications.Mobile.Firebase;
using LockerService.Infrastructure.Services.Notifications.Sms.ZaloZns;
using LockerService.Infrastructure.Services.Notifications.Website.SignalR;
using LockerService.Infrastructure.Services.Payments;
using LockerService.Infrastructure.Services.Payments.Momo;
using LockerService.Infrastructure.Services.Payments.VnPay;
using LockerService.Infrastructure.Settings;
using LockerService.Infrastructure.SignalR;
using LockerService.Infrastructure.SignalR.Notifications;
using LockerService.Shared.Utils;
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
            });
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICurrentPrincipalService, CurrentPrincipalService>();
        services.AddScoped<ICurrentAccountService, CurrentAccountService>();
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
        services.AddScoped<SoftDeleteInterceptor>();
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
            options.UseProjectables();
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

        services.AddScoped<ISettingService, SettingService>();
        
        // Storage
        services.AddOptions<AwsS3Settings>()
            .BindConfiguration(AwsS3Settings.ConfigSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<AwsS3Settings>>().Value);

        services.AddScoped<IStorageService, StorageService>();
        
        
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
            config.AddConsumer<OrderUpdatedStatusConsumer>();
            config.AddConsumer<OrderInitializedConsumer>();
            config.AddConsumer<OrderConfirmedConsumer>();
            config.AddConsumer<OrderCollectedConsumer>();
            config.AddConsumer<OrderProcessedConsumer>();
            config.AddConsumer<OrderReturnedConsumer>();
            config.AddConsumer<OrderCompletedConsumer>();
            config.AddConsumer<OrderOvertimeConsumer>();
            config.AddConsumer<OrderCanceledConsumer>();
            
            
            // Locker events consumers
            config.AddConsumer<LockerConnectedConsumer>();
            config.AddConsumer<LockerDisconnectedConsumer>();
            config.AddConsumer<LockerOverloadedConsumer>();
            config.AddConsumer<LockerUpdatedInfoConsumer>();
            config.AddConsumer<LockerUpdatedStatusConsumer>();
            config.AddConsumer<LockerAddBoxConsumer>();
            config.AddConsumer<LockerResetBoxesConsumer>();
            config.AddConsumer<LockerRemoveBoxConsumer>();
            config.AddConsumer<OrderReservedConsumer>();
            
            // Account event consumers
            config.AddConsumer<OtpCreatedConsumer>();
            config.AddConsumer<StaffCreatedConsumer>();
            
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
                cfg.UseRawJsonSerializer();
                cfg.ConfigureJsonSerializerOptions(options => JsonSerializerUtils.GetGlobalJsonSerializerOptions());
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
            
            // Cron jobs for order overtime jobs every hours 
            var orderOvertimeJobKey = new JobKey(OrderOvertimeJob.OrderOvertimeJobKey);
            q.AddJob<OrderOvertimeJob>(options => options.WithIdentity(orderOvertimeJobKey));

            q.AddTrigger(options =>
                options.ForJob(orderOvertimeJobKey)
                    .WithIdentity($"{OrderOvertimeJob.OrderOvertimeJobKey}-trigger")
                    .WithCronSchedule("0 0/5 * ? * * *", x => x.InTimeZone(TimeZoneInfo.Utc))
            );
        });

        services.AddQuartzServer(options =>
        {
            // options.WaitForJobsToComplete = true;
        });
        
        // Fee
        services.AddScoped<IOrderService, OrderService>();
        
        // Address
        services.AddHostedService<ImportAddressService>();
        
        // Payment
        services.AddOptions<MomoSettings>()
            .BindConfiguration(MomoSettings.ConfigSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<VnPaySettings>()
            .BindConfiguration(VnPaySettings.ConfigSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();
              
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<VnPaySettings>>().Value);
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<MomoSettings>>().Value);

        services.AddScoped<IMomoPaymentService, MomoPaymentService>();
        services.AddScoped<IVnPayPaymentService, VnPayPaymentService>();
        services.AddScoped<IPaymentService, PaymentService>();
        
        // Notification
        services.AddOptions<FcmSettings>()
            .BindConfiguration(FcmSettings.ConfigSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<FcmSettings>(sp => sp.GetRequiredService<IOptions<FcmSettings>>().Value);
        
        services.AddSignalR()
            .AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.Converters
                    .Add(new JsonStringEnumConverter());
            });
        services.AddOptions<TwilioSettings>()
            .BindConfiguration(TwilioSettings.ConfigSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<TwilioSettings>>().Value);
        services.AddSingleton<INotificationAdapter, NotificationAdapter>();
        
        services.AddOptions<ZaloZnsSettings>()
            .BindConfiguration(ZaloZnsSettings.ConfigSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<ZaloZnsSettings>>().Value);
        services.AddSingleton<ZaloAuthService>();
        
        services.AddSingleton<ISmsNotificationService, ZnsNotificationService>();
        services.AddSingleton<IWebNotificationService, WebNotificationService>();
        services.AddSingleton<IMobileNotificationService, FirebaseNotificationService>();
        services.AddSingleton<INotificationProvider, NotificationProvider>();
        services.AddSingleton<INotifier, Notifier>();
        services.AddSingleton<NotificationConnectionManager>();
        services.AddSingleton<ConnectionManagerServiceResolver>(serviceProvider => type =>
        {
            return type switch
            {  
                Type _ when type == typeof(NotificationConnectionManager) 
                    => serviceProvider.GetRequiredService<NotificationConnectionManager>(),
                _ => throw new KeyNotFoundException()
            };
        });
        
        // Server 
        services.AddOptions<ServerSettings>()
            .BindConfiguration(ServerSettings.ConfigSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton(sp => sp.GetRequiredService<IOptions<ServerSettings>>().Value);

        // Token Service
        services.AddScoped<ITokenService, TokenService>();
        
        // LockerService
        services.AddScoped<ILockersService, LockersService>();
        
        return services;
    }

    private static bool IsDevelopment()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        return environment == Environments.Development;
    }
}
using System.Text;
using LockerService.Application.Common.Persistence;
using LockerService.Application.Common.Security;
using LockerService.Application.Common.Services;
using LockerService.Application.Common.Services.Notification;
using LockerService.Application.Common.Services.Notification.Data;
using LockerService.Application.EventBus.Mqtt;
using LockerService.Infrastructure.EventBus.Mqtt;
using LockerService.Infrastructure.Persistence;
using LockerService.Infrastructure.Repositories;
using LockerService.Infrastructure.Services;
using LockerService.Infrastructure.Services.Notification;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using IUnitOfWork = LockerService.Infrastructure.Repositories.IUnitOfWork;

namespace LockerService.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddHttpContextAccessor();
        services.AddScoped<Application.Common.Persistence.IUnitOfWork, IUnitOfWork>();
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

        initializer.InitializeAsync().Wait();
        initializer.SeedAsync().Wait();
        
        // Storage
        services.AddScoped<IStorageService, StorageService>();
        
        // Jwt service
        services.AddScoped<IJwtService, JwtService>();

        // Authentication, Authorization
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // Validate JWT Token
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new ArgumentException("Jwt:Key is required")))
                };
            });
        
        // Security
        services.AddScoped<ISecurityService, AesSecurityService>();
        
        // Mqtt
        services.AddSingleton<MqttClientService>();
        services.AddScoped<IMqttBus, MqttBus>();
        services.AddHostedService<MqttClientService>(sp => sp.GetRequiredService<MqttClientService>());
        services.AddSingleton<MqttClientServiceProvider>(sp =>
        {
            var mqttClientService = sp.GetRequiredService<MqttClientService>();
            var mqttClientServiceProvider = new MqttClientServiceProvider(mqttClientService);
            return mqttClientServiceProvider;
        });
        
        // Quartz
        services.AddScoped<IOrderTimeoutService, OrderTimeoutService>();
        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();
            q.UsePersistentStore(options =>
            {
                // options.UseProperties = true;
                options.UsePostgres(configuration["Quartz:ConnectionString"] ?? "");
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
        
        return services;
    }
}
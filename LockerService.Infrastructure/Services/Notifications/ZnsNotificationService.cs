using System.Text;
using System.Text.Json.Serialization;
using LockerService.Application.Common.Extensions;
using LockerService.Application.Common.Services;
using LockerService.Application.Common.Services.Notifications;
using LockerService.Domain.Entities.Settings;
using LockerService.Domain.Enums;
using LockerService.Infrastructure.HttpClients;
using LockerService.Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace LockerService.Infrastructure.Services.Notifications;

public class ZnsNotificationService : ISmsNotificationService
{
    private readonly ZaloZnsSettings _znsSettings;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<ZnsNotificationService> _logger;
    private const int InvalidAccessTokenErrorCode = -124; 

    public ZnsNotificationService(
        ZaloZnsSettings znsSettings, 
        ILogger<ZnsNotificationService> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _znsSettings = znsSettings;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task NotifyAsync(Notification notification)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var zaloAuthService = serviceProvider.GetRequiredService<ZaloAuthService>();
        var settingService = serviceProvider.GetRequiredService<ISettingService>();
        var zaloAuthSettings = await settingService.GetSettings<ZaloAuthSettings>();

        using var httpClient = new HttpClient(new RetryHandler(new HttpClientHandler()));
        httpClient.Timeout = TimeSpan.FromSeconds(30);
        httpClient.DefaultRequestHeaders.Add("access_token", zaloAuthSettings.AccessToken);
        
        var requestData = GetRequestContent(notification);
        var content = new StringContent(requestData.ToString() ?? string.Empty, Encoding.UTF8, "application/json");
        
        while (true)
        {
            using var response = await httpClient.PostAsync(_znsSettings.ZnsUrl, content);
            var jsonString = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<BaseZaloZnsResponse>(jsonString);
            if (!response.IsSuccessStatusCode || responseData == null)
            {
                return;
            }
            
            if (!responseData.IsError)
            {
                _logger.LogInformation("[Zalo ZNS] Send message successfully: {0}", requestData);
                return;
            }

            // Recreate access token when expired
            if (responseData.Error == InvalidAccessTokenErrorCode)
            {
                _logger.LogInformation("[Zalo ZNS] Invalid access token. Recreate access token");
                var authToken = await zaloAuthService.GetAccessToken(zaloAuthSettings.RefreshToken);
                if (authToken == null)
                {
                    return;
                }
                
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("access_token", authToken.AccessToken);

                await settingService.UpdateSettings(new ZaloAuthSettings()
                {
                    AccessToken = authToken.AccessToken,
                    RefreshToken = authToken.RefreshToken
                });
            }
            else
            {
                _logger.LogError("[Zalo ZNS] Send message failed: {0}", responseData?.Message);
                return;
            }
        }
    }

    private object GetRequestContent(Notification notification)
    {
        switch (notification.Type)
        {
            case NotificationType.AccountOtpCreated:
                return new
                {
                    phone = notification.Account.PhoneNumber.ToVietnamesePhoneNumber() ?? throw new Exception("Phone number is required"),
                    template_id = _znsSettings.Templates.Otp,
                    template_data = new
                    {
                        otp = notification.Data
                    }
                };
        }
        throw new Exception("[Zalo ZNS] Invalid notification type");
    }

    public Task SendSmsAsync(string phoneNumber, string content)
    {
        throw new NotImplementedException();
    }

}

public class BaseZaloZnsResponse
{
    [JsonPropertyName(("error"))]
    public int Error { get; set; }

    [JsonPropertyName("message")] 
    public string Message { get; set; } = default!;

    [JsonPropertyName("data")]
    public object Data { get; set; } = default!;

    public bool IsError => Error != 0;
}

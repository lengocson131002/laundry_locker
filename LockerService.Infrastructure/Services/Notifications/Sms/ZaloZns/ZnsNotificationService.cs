using System.Text;
using LockerService.Domain.Entities.Settings;
using LockerService.Infrastructure.HttpClients;
using LockerService.Infrastructure.Settings;
using LockerService.Shared.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace LockerService.Infrastructure.Services.Notifications.Sms.ZaloZns;

public class ZnsNotificationService : ISmsNotificationService
{
    private readonly ZaloZnsSettings _znsSettings;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<ZnsNotificationService> _logger;

    public ZnsNotificationService(
        ZaloZnsSettings znsSettings, 
        ILogger<ZnsNotificationService> logger, 
        IServiceScopeFactory serviceScopeFactory)
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
        var znsNotificationAdaptor = serviceProvider.GetRequiredService<ZnsNotificationAdaptor>();
        var settingService = serviceProvider.GetRequiredService<ISettingService>();
        var zaloAuthSettings = await settingService.GetSettings<ZaloAuthSettings>();

        using var httpClient = new HttpClient(new ZaloRequestHandler(
                zaloAuthService,
                _logger,
                settingService
            ));
        httpClient.Timeout = TimeSpan.FromSeconds(30);
        
        var requestData = await znsNotificationAdaptor.GetRequestContent(notification);
        var content = new StringContent(
            JsonSerializer.Serialize(requestData, JsonSerializerUtils.GetGlobalJsonSerializerOptions()), 
            Encoding.UTF8, 
            "application/json");
        
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, _znsSettings.ZnsUrl)
        {
            Content = content,
        };
        httpRequest.Headers.Add("access_token", zaloAuthSettings.AccessToken);
        
        using var response = await httpClient.SendAsync(httpRequest);
        var jsonString = await response.Content.ReadAsStringAsync();
        var responseData = JsonSerializer.Deserialize<BaseZaloZnsResponse>(jsonString);
        if (response.IsSuccessStatusCode && responseData != null && !responseData.IsError)
        {
            _logger.LogInformation("[Zalo ZNS] Send message successfully: {0}", requestData);
            return;
        }
        _logger.LogError("[Zalo ZNS] Send message failed. {0}", responseData?.Message);
    }
    
    public Task SendSmsAsync(string phoneNumber, string content)
    {
        throw new NotImplementedException();
    }

}

public class ZaloRequestHandler : DelegatingHandler
{
    private readonly ZaloAuthService _zaloAuthService;

    private readonly ISettingService _settingService;
    
    private readonly ILogger _logger;
    
    private const int InvalidAccessTokenErrorCode = -124;
        
    public ZaloRequestHandler(
        ZaloAuthService zaloAuthService, 
        ILogger logger, 
        ISettingService settingService) : base(new RetryHandler())
    {
        _zaloAuthService = zaloAuthService;
        _logger = logger;
        _settingService = settingService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var zaloAuthSettings = await _settingService.GetSettings<ZaloAuthSettings>(cancellationToken);
        HttpResponseMessage response = null;
        while (true)
        {
            response = await base.SendAsync(request, cancellationToken);
            var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);
            var responseData = JsonSerializer.Deserialize<BaseZaloZnsResponse>(jsonString);
            
            // Recreate access token when expired
            if (response.IsSuccessStatusCode && responseData != null && responseData.Error == InvalidAccessTokenErrorCode)
            {
                _logger.LogInformation("[Zalo ZNS] Invalid access token. Recreate access token");
                var authToken = await _zaloAuthService.GetAccessToken(zaloAuthSettings.RefreshToken);
                if (authToken == null)
                {
                    break;
                }
                
                request.Headers.Clear();
                request.Headers.Add("access_token", authToken.AccessToken);

                await _settingService.UpdateSettings(new ZaloAuthSettings()
                {
                    AccessToken = authToken.AccessToken,
                    RefreshToken = authToken.RefreshToken
                }, cancellationToken);
            }
            else
            {
                break;
            }
        }
        return response;
    }
}
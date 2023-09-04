using System.Text;
using System.Text.Json.Serialization;
using LockerService.Application.Common.Extensions;
using LockerService.Application.Common.Services;
using LockerService.Application.Common.Services.Notifications;
using LockerService.Application.Common.Utils;
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

        using var httpClient = new HttpClient(new ZaloRequestHandler(
                zaloAuthService,
                _logger,
                settingService
            ));
        httpClient.Timeout = TimeSpan.FromSeconds(30);
        
        var requestData = await GetRequestContent(notification);
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

    private async Task<BaseZaloZnsRequest> GetRequestContent(Notification notification)
    {

        using var scope = _serviceScopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var settingService = serviceProvider.GetRequiredService<ISettingService>();
        var orderService = serviceProvider.GetRequiredService<IOrderService>();
            
        var account = notification.Account;
        if (account == null)
        {
            throw new Exception("Notified account is not found");
        }

        var toPhoneNumber = account.PhoneNumber.ToVietnamesePhoneNumber();
        switch (notification.Type)
        {
            case NotificationType.AccountOtpCreated:
                return new BaseZaloZnsRequest()
                {
                    Phone = toPhoneNumber,
                    TemplateId = _znsSettings.Templates.Otp,
                    TemplatedData = new
                    {
                        otp = notification.Data
                    }
                };
            
            case NotificationType.AccountStaffCreated:
                return new BaseZaloZnsRequest()
                {
                    Phone = toPhoneNumber,
                    TemplateId = _znsSettings.Templates.StaffAccountCreated,
                    TemplatedData = new
                    {
                        staff_name = account.FullName,
                        username = account.Username,
                        password = account.Password,
                        role = account.Role.ToString(),
                    }
                };
            
            case NotificationType.OrderCreated:
                var createdOrder = JsonSerializer.Deserialize<Order>(
                    notification.Data ?? string.Empty, 
                    JsonSerializerUtils.GetGlobalJsonSerializerOptions());
                    
                if (createdOrder == null)
                {
                    throw new Exception("[Zalo ZNS] Notification's data is required");
                }
                
                var sender = createdOrder.Sender 
                             ?? throw new Exception("[Zalo ZNS] Order sender is required");
                
                var locker = createdOrder.Locker 
                             ?? throw new Exception("[Zalo ZNS] Order locker is required");
                
                var details = createdOrder.Details 
                              ?? throw new Exception("[Zalo ZNS] Order detail is required");
                
                var orderDetailDescription = Equals(createdOrder.Type, OrderType.Laundry)
                    ? string.Join(", ", details.Select(item => item.Service.Name))
                    : "None";

                return new BaseZaloZnsRequest()
                {
                    Phone = toPhoneNumber,
                    TemplateId = _znsSettings.Templates.OrderCreated,
                    TemplatedData = new
                    {
                        customer_name = account.FullName ?? account.PhoneNumber,
                        order_id = createdOrder.Id.ToString(),
                        order_type = createdOrder.Type.GetDescription(),
                        pin_code = createdOrder.PinCode,
                        sender_name = sender.FullName ?? sender.PhoneNumber,
                        locker_name = locker.Name,
                        locker_address = locker.Location.ToString(),
                        order_status = createdOrder.Status.GetDescription(),
                        order_detail = orderDetailDescription
                    }
                };
            
            case  NotificationType.OrderCanceled:
                var canceledOrder = JsonSerializer.Deserialize<Order>(
                    notification.Data ?? string.Empty, 
                    JsonSerializerUtils.GetGlobalJsonSerializerOptions());
                    
                if (canceledOrder == null)
                {
                    throw new Exception("[Zalo ZNS] Notification's data is required");
                }
                
                var canceledOrderLocker = canceledOrder.Locker 
                                          ?? throw new Exception("[Zalo ZNS] Order locker is required");

                return new BaseZaloZnsRequest()
                {
                    Phone = toPhoneNumber,
                    TemplateId = _znsSettings.Templates.OrderCanceled,
                    TemplatedData = new
                    {
                        customer_name = account.FullName ?? account.PhoneNumber,
                        order_id = canceledOrder.Id.ToString(),
                        order_type = canceledOrder.Type.GetDescription(),
                        order_status = canceledOrder.Status.GetDescription(),
                        locker_name = canceledOrderLocker.Name,
                        locker_address = canceledOrderLocker.Location.ToString(),
                        cancel_reason = canceledOrder.CancelReason != null 
                            ? canceledOrder.CancelReason.Value.GetDescription()
                            : "None"
                    }
                };
            
            case  NotificationType.OrderReturned:
                var returnedOrder = JsonSerializer.Deserialize<Order>(
                    notification.Data ?? string.Empty,
                    JsonSerializerUtils.GetGlobalJsonSerializerOptions());

                if (returnedOrder == null)
                {
                    throw new Exception("[Zalo ZNS] Notification's data is required");
                }

                var returnedOrderLocker = returnedOrder.Locker 
                                          ?? throw new Exception("[Zalo ZNS] Order locker is required");
                
                var returnedOrderDetails = returnedOrder.Details 
                              ?? throw new Exception("[Zalo ZNS] Order detail is required");

                var returnOrderDetailDescription = Equals(returnedOrder.Type, OrderType.Laundry)
                    ? string.Join(", ", returnedOrderDetails.Select(item => item.Service.Name))
                    : "None";
                
                var orderSettings = await settingService.GetSettings<OrderSettings>();
                var timeSettings = await settingService.GetSettings<TimeSettings>();
                var extraAt = returnedOrder.CreatedAt.AddHours(orderSettings.MaxTimeInHours);

                await orderService.CalculateFree(returnedOrder);
                
                return new BaseZaloZnsRequest()
                {
                    Phone = toPhoneNumber,
                    TemplateId = _znsSettings.Templates.OrderReturned,
                    TemplatedData = new
                    {
                        customer_name = account.FullName ?? account.PhoneNumber,
                        order_id = returnedOrder.Id.ToString(),
                        order_type = returnedOrder.Type.GetDescription(),
                        order_status = returnedOrder.Status.GetDescription(),
                        pin_code = returnedOrder.PinCode,
                        order_price = returnedOrder.Price 
                                      ?? throw new Exception("[Zalo ZNS] Order price is required "),
                        order_detail = returnOrderDetailDescription,
                        extra_at = extraAt.ToString(timeSettings.TimeZone, DateTimeConstants.DateTimeFormat),
                        extra_fee = orderSettings.ExtraFee,
                        locker_name = returnedOrderLocker.Name,
                        locker_address = returnedOrderLocker.Location.ToString()
                    }
                };
            
            case NotificationType.OrderOverTime:
                var overtimeOrder = JsonSerializer.Deserialize<Order>(
                    notification.Data ?? string.Empty,
                    JsonSerializerUtils.GetGlobalJsonSerializerOptions());
                
                if (overtimeOrder == null)
                {
                    throw new Exception("[Zalo ZNS] Notification's data is required");
                }

                var overtimeOrderLocker = overtimeOrder.Locker 
                                          ?? throw new Exception("[Zalo ZNS] Order locker is required");
                
                var overtimeOrderDetails = Equals(overtimeOrder.Type, OrderType.Laundry)
                    ? string.Join(", ", overtimeOrder.Details.Select(item => item.Service.Name))
                    : "None";
                
                await orderService.CalculateFree(overtimeOrder);
                
                return new BaseZaloZnsRequest()
                {
                    Phone = toPhoneNumber,
                    TemplateId = _znsSettings.Templates.OrderOvertime,
                    TemplatedData = new
                    {
                        customer_name = account.FullName ?? account.PhoneNumber,
                        order_id = overtimeOrder.Id.ToString(),
                        order_type = overtimeOrder.Type.GetDescription(),
                        order_status = overtimeOrder.Status.GetDescription(),
                        pin_code = overtimeOrder.PinCode,
                        sender_name = overtimeOrder.Sender.FullName ?? overtimeOrder.Sender.PhoneNumber,
                        order_detail = overtimeOrderDetails,
                        locker_name = overtimeOrderLocker.Name,
                        locker_address = overtimeOrderLocker.Location.ToString()
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


public class BaseZaloZnsRequest
{
    [JsonPropertyName("phone")]
    public string Phone { get; set; } = default!;

    [JsonPropertyName("template_id")]
    public string TemplateId { get; set; } = default!;

    [JsonPropertyName("template_data")]
    public object TemplatedData { get; set; } = default!;


}
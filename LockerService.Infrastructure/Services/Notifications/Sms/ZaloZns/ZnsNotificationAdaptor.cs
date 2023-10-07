using System.Text.Json.Serialization;
using LockerService.Domain.Entities.Settings;
using LockerService.Domain.Enums;
using LockerService.Infrastructure.Settings;
using LockerService.Shared.Constants;
using LockerService.Shared.Extensions;
using LockerService.Shared.Utils;

namespace LockerService.Infrastructure.Services.Notifications.Sms.ZaloZns;

public class ZnsNotificationAdaptor
{
    private readonly ZaloZnsSettings _znsSettings;
    private readonly ILogger<ZnsNotificationAdaptor> _logger;
    private readonly ISettingService _settingService;

    public ZnsNotificationAdaptor(ILogger<ZnsNotificationAdaptor> logger, ZaloZnsSettings znsSettings, ISettingService settingService)
    {
        _logger = logger;
        _znsSettings = znsSettings;
        _settingService = settingService;
    }

    public async Task<BaseZaloZnsRequest> GetRequestContent(Notification notification)
    {

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
            
            case NotificationType.SystemStaffCreated:
                return new BaseZaloZnsRequest()
                {
                    Phone = toPhoneNumber,
                    TemplateId = _znsSettings.Templates.StaffAccountCreated,
                    TemplatedData = new
                    {
                        staff_name = account.FullName,
                        username = account.Username,
                        password = account.Password,
                        role = account.Role.GetDescription(),
                    }
                };
            
            case NotificationType.CustomerOrderCreated:
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
            
            case  NotificationType.CustomerOrderCanceled:
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
            
            case  NotificationType.CustomerOrderReturned:
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
                
                var timeSettings = await _settingService.GetSettings<TimeSettings>();
    
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
                        order_price = returnedOrder.Price,
                        order_detail = returnOrderDetailDescription,
                        extra_at = returnedOrder.IntendedOvertime != null 
                            ? returnedOrder.IntendedOvertime.Value.ToString(timeSettings.TimeZone, DateTimeConstants.DateTimeFormat)
                            : string.Empty,
                        extra_fee = returnedOrder.ExtraFee,
                        locker_name = returnedOrderLocker.Name,
                        locker_address = returnedOrderLocker.Location.ToString()
                    }
                };
            
            case NotificationType.CustomerOrderOverTime:
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
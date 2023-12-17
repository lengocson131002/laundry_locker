using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Common.Services.Notifications.Models;
using LockerService.Domain.Entities.Settings;
using LockerService.Domain.Enums;
using LockerService.Infrastructure.Settings;
using LockerService.Shared.Constants;
using LockerService.Shared.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LockerService.Infrastructure.Services.Notifications;

public class NotificationAdapter : INotificationAdapter
{
    private readonly ZaloZnsSettings _znsSettings;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public NotificationAdapter(ZaloZnsSettings znsSettings, IServiceScopeFactory serviceScopeFactory)
    {
        _znsSettings = znsSettings;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<WebNotification> ToWebNotification(Notification notification, string connectionId)
    {
        var webNotification = new WebNotification()
        {
            Id = notification.Id,
            AccountId = notification.AccountId,
            Type = notification.Type,
            EntityType = notification.EntityType,
            ReferenceId = notification.ReferenceId,
            Content = notification.Content,
            Data = notification.Data,
            ReadAt = notification.ReadAt,
            CreatedAt = notification.CreatedAt,
            Level = notification.Level,
            Title = notification.Title
        };

        return await Task.FromResult(webNotification);
    }

    public async Task<ZaloZnsNotification> ToZaloZnsNotification(Notification notification)
    {
        var scope = _serviceScopeFactory.CreateScope();
        var settingService = scope.ServiceProvider.GetRequiredService<ISettingService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            
        var account = notification.Account;
        if (account == null)
        {
            throw new Exception("Notified account is not found");
        }

        var toPhoneNumber = account.PhoneNumber.ToVietnamesePhoneNumber();
        switch (notification.Type)
        {
            case NotificationType.AccountOtpCreated:
                return new ZaloZnsNotification()
                {
                    Phone = toPhoneNumber,
                    TemplateId = _znsSettings.Templates.Otp,
                    TemplatedData = new
                    {
                        otp = notification.Data
                    }
                };
            
            case NotificationType.SystemStaffCreated:
                return new ZaloZnsNotification()
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
                var createdOrder = notification.ReferenceId != null 
                    ? await unitOfWork.OrderRepository
                        .GetOrderInformation(long.Parse(notification.ReferenceId))
                        .FirstOrDefaultAsync()
                    : null;
                    
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

                return new ZaloZnsNotification()
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
                var canceledOrder = notification.ReferenceId != null 
                    ? await unitOfWork.OrderRepository
                        .GetOrderInformation(long.Parse(notification.ReferenceId))
                        .FirstOrDefaultAsync()
                    : null;
                    
                if (canceledOrder == null)
                {
                    throw new Exception("[Zalo ZNS] Notification's data is required");
                }
                
                var canceledOrderLocker = canceledOrder.Locker 
                                          ?? throw new Exception("[Zalo ZNS] Order locker is required");

                return new ZaloZnsNotification()
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
                var returnedOrder = notification.ReferenceId != null 
                    ? await unitOfWork.OrderRepository
                        .GetOrderInformation(long.Parse(notification.ReferenceId))
                        .FirstOrDefaultAsync()
                    : null;
                
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
                
                var timeSettings = await settingService.GetSettings<TimeSettings>();
    
                return new ZaloZnsNotification()
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
                        // extra_fee = returnedOrder.ExtraFee,
                        locker_name = returnedOrderLocker.Name,
                        locker_address = returnedOrderLocker.Location.ToString()
                    }
                };
            
            case NotificationType.CustomerOrderOverTime:
                var overtimeOrder = notification.ReferenceId != null 
                    ? await unitOfWork.OrderRepository
                        .GetOrderInformation(long.Parse(notification.ReferenceId))
                        .FirstOrDefaultAsync()
                    : null;
                
                if (overtimeOrder == null)
                {
                    throw new Exception("[Zalo ZNS] Notification's data is required");
                }

                var overtimeOrderLocker = overtimeOrder.Locker 
                                          ?? throw new Exception("[Zalo ZNS] Order locker is required");
                
                var overtimeOrderDetails = Equals(overtimeOrder.Type, OrderType.Laundry)
                    ? string.Join(", ", overtimeOrder.Details.Select(item => item.Service.Name))
                    : "None";
                
                return new ZaloZnsNotification()
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

    public async Task<FirebaseNotification> ToFirebaseNotification(Notification notification, string deviceToken, DeviceType? deviceType = null)
    {
        var firebaseNotification = new FirebaseNotification()
        {
            Message = new()
            {
                Token = deviceToken,
                Notification = new ()
                {
                    Title = notification.Title,
                    Body = notification.Content
                },
                Data = new {
                    Type = notification.Type.ToString(),
                    EntityType = notification.EntityType.ToString(),
                    ReferenceId = notification.ReferenceId
                },
            }
        };

        return await Task.FromResult(firebaseNotification);
    }
}
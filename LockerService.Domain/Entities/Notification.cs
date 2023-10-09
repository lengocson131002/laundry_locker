using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EntityFrameworkCore.Projectables;
using LockerService.Domain.Enums;
using LockerService.Shared.Extensions;

namespace LockerService.Domain.Entities;

[Table(("Notification"))]
public class Notification : BaseAuditableEntity
{
    [Key]
    public long Id { get; set; }
    
    public long AccountId { get; set; }

    public Account Account { get; set; } = default!;
    
    public NotificationType Type { get; set; }
    
    public EntityType EntityType { get; set; }
    
    public string? ReferenceId { get; set; }

    public string Title { get; set; } = default!;
    
    public string Content { get; set; } = default!;
    
    [Column(TypeName = "jsonb")]
    public string? Data { get; set; }

    public DateTimeOffset? ReadAt { get; set; }

    [Projectable]
    public bool IsRead => ReadAt != null || Deleted;

    [NotMapped] 
    public bool Saved { get; set; } = true;

    public NotificationLevel Level { get; set; } = NotificationLevel.Information;
    
    public Notification()
    {
    }

    public Notification(Account account, NotificationType type, EntityType entityType, object? data = null, bool? saved = true)
    {
        Type = type;
        Account = account;
        EntityType = entityType;
        
        // Save notification into database
        Saved = saved ?? true;

        switch (Type)
        {
            case NotificationType.AccountOtpCreated:
            {
                Title = "OTP mới được khởi tạo";
                Content = "OTP mới được khởi tạo";
                Level = NotificationLevel.Information;
                
                // OTP is data
                Data = data.ToString();
                break;
                
            }

            case NotificationType.SystemStaffCreated:
            {
                Title = "Tài khoản mới";
                Content = "Tài khoản nhân viên mới được khởi tạo";
                Level = NotificationLevel.Information;
                break;
            }

            case NotificationType.SystemLockerConnected:
            {
                if (data == null)
                {
                    throw new Exception($"[Notification] Data is required. Type: {Type}");
                }
                
                var locker = (Locker) data;
                Title = "Locker kết nối đến hệ thống";
                Content = $"Locker {locker.Code} đã được kết nối đến hệ thống.";
                ReferenceId = locker.Id.ToString();
                Level = NotificationLevel.Information;
                break;
            }

            case NotificationType.SystemLockerDisconnected:
            {
                if (data == null)
                {
                    throw new Exception($"[Notification] Data is required. Type: {Type}");
                }
                
                var locker = (Locker) data;
                Title = "Locker đã bị ngắt kết nối khỏi hệ thống";
                Content = $"Locker {locker.Code} đã ngắt kết nối khỏi hệ thống.";
                ReferenceId = locker.Id.ToString();
                Level = NotificationLevel.Critical;
                break;
            }

            case NotificationType.SystemLockerBoxWarning:
            {
                if (data == null)
                {
                    throw new Exception($"[Notification] Data is required. Type: {Type}");
                }
                
                var locker = (Locker) data;
                
                Title = "Locker sắp quá tải";
                Content = "Locker sắp quá tải. Vui long đến nhận đồ";
                ReferenceId = locker.Id.ToString();
                Level = NotificationLevel.Warning;
                break;
            }

            case NotificationType.SystemLockerBoxOverloaded:
            {
                if (data == null)
                {
                    throw new Exception($"[Notification] Data is required. Type: {Type}");
                }
                
                var locker = (Locker) data;
                
                Title = "Locker đã bị quá tải";
                Content = "Locker đã bị quá tải";
                ReferenceId = locker.Id.ToString();
                Level = NotificationLevel.Critical;
                break;
            }

            case NotificationType.SystemOrderCreated:
            {
                if (data == null)
                {
                    throw new Exception($"[Notification] Data is required. Type: {Type}");
                }
                
                var order = (Order) data;
                
                Title = "Đơn hàng mới vừa được khởi tạo";
                Content = "Đơn hàng mới vừa được khởi tạo";
                ReferenceId = order.Id.ToString();
                Level = NotificationLevel.Information;
                break;
            }

            case NotificationType.SystemOrderOverTime:
            {
                if (data == null)
                {
                    throw new Exception($"[Notification] Data is required. Type: {Type}");
                }
                
                var order = (Order) data;
                Title = "Đơn hàng quá hạn";
                Content = "Đơn hàng tại Locker đã quá hạn. Vui lòng liên hệ khách hàng hoặc đến xử lý";
                Level = NotificationLevel.Information;
                ReferenceId = order.Id.ToString();
                break;
            }  
            
            case NotificationType.CustomerOrderCreated:
            {
                if (data == null)
                {
                    throw new Exception($"[Notification] Data is required. Type: {Type}");
                }

                var order = (Order)data;
                Title = "Đơn hàng mới được khởi tạo";
                Content = $"Đơn hàng mới được khởi tạo. Pin code: {order.PinCode}";
                Level = NotificationLevel.Information;
                ReferenceId = order.Id.ToString();
                break;
            }  
            
            case NotificationType.CustomerOrderReturned:
            {
                if (data == null)
                {
                    throw new Exception($"[Notification] Data is required. Type: {Type}");
                }

                var order = (Order)data;
                
                Title = "Đơn hàng đã được xử lý";
                Content = $"Đơn hàng của bạn đã được xử lý. Vui lòng đến Locker để nhận. Pin Code: {order.PinCode}";
                Level = NotificationLevel.Information;
                ReferenceId = order.Id.ToString();
                break;
            }   
            
            case NotificationType.CustomerOrderCanceled:
            {
                if (data == null)
                {
                    throw new Exception($"[Notification] Data is required. Type: {Type}");
                }

                var order = (Order)data;
                var cancelReason = order.CancelReason != null
                    ? order.CancelReason.Value.GetDescription()
                    : string.Empty;
                
                Title = "Đơn hàng đã hủy";
                Content = $"Đơn hàng của bạn đã bị hủy. Lý do hủy: {cancelReason}";
                Level = NotificationLevel.Information;
                ReferenceId = order.Id.ToString();
                break;
            }
            case NotificationType.CustomerOrderCompleted:
            {
                if (data == null)
                {
                    throw new Exception($"[Notification] Data is required. Type: {Type}");
                }

                var order = (Order)data;
                
                Title = "Đơn hàng đã hoàn thành";
                Content = "Đơn hàng đã hoàn thành. Xin cảm ơn";
                Level = NotificationLevel.Information;
                ReferenceId = order.Id.ToString();
                
                break;
            }
            
            case NotificationType.CustomerOrderOverTime:
            {
                if (data == null)
                {
                    throw new Exception($"[Notification] Data is required. Type: {Type}");
                }

                var order = (Order)data;
                Title = "Đơn hàng quá hạn";
                Content = "Đơn hàng quá hạn. Vui lòng đến nhận";
                Level = NotificationLevel.Critical;
                ReferenceId = order.Id.ToString();
                break;
            }
            
            default:
                Title = type.ToString();
                Content = "Bạn có thông báo mới. Click để xem chi tiết";
                break;
        }

    }
}
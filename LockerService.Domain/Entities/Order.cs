using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EntityFrameworkCore.Projectables;
using LockerService.Domain.Enums;

namespace LockerService.Domain.Entities;

[Table("Order")]
public class Order : BaseAuditableEntity
{
    [Key] 
    public long Id { get; set; }

    public string? ReferenceId { get; set; }

    public OrderType Type { get; set; }

    public string? PinCode { get; set; }

    public DateTimeOffset? PinCodeIssuedAt { get; set; }

    // Sender info
    public long SendBoxId { get; set; }

    public Box SendBox { get; set; } = default!;

    public long SenderId { get; set; }

    public Account Sender { get; set; } = default!;

    // Receiver info
    public long? ReceiveBoxId { get; set; }

    public Box? ReceiveBox { get; set; } = default!;

    public long? ReceiverId { get; set; }

    public Account? Receiver { get; set; }

    // Thời gian nhận dự kiến
    public DateTimeOffset? IntendedReceiveAt { get; set; }

    // Thời gian quá hạn dự kiến
    public DateTimeOffset? IntendedOvertime { get; set; }

    // Thời gian nhận thật sự
    public DateTimeOffset? ReceiveAt { get; set; }
    
    public OrderStatus Status { get; set; } = OrderStatus.Initialized;

    public OrderCancelReason? CancelReason { get; set; }

    public long LockerId { get; set; }

    public Locker Locker { get; set; } = default!;

    public long? StaffId { get; set; }

    public Account? Staff { get; set; }

    // Phí quá giờ / 1 giờ đối với dịch vụ giặt sấy
    public decimal ExtraFee { get; set; }

    // Phí nếu là dịch vụ gửi đồ
    public decimal StoragePrice { get; set; }

    // Giảm giá
    public decimal Discount { get; set; } // Giảm giá

    // Phí đặt cọc trước
    public decimal ReservationFee { get; set; }

    // Thời gian quá giờ đối với dịch vụ giặt sấy
    public float ExtraCount
    {
        get
        {
            var calculateAt = IsFinished 
                ? ReceiveAt ?? DateTimeOffset.UtcNow 
                : DateTimeOffset.UtcNow;

            if (IsStorage || calculateAt < IntendedOvertime || IntendedOvertime == null)
            {
                return 0;
            }

            var extraTimespan = calculateAt - IntendedOvertime.Value;
            var extraInHours = (float)extraTimespan.TotalMinutes / 60;
            return (float)Math.Round(extraInHours, 2, MidpointRounding.AwayFromZero);
        }
    }

    public decimal TotalExtraFee => ExtraFee * (decimal) ExtraCount;

    // Tổng chi phí dịch vụ
    public decimal Price
    {
        get
        {
            if (IsStorage)
            {
                var calculateAt = IsFinished 
                    ? ReceiveAt ?? DateTimeOffset.UtcNow 
                    : DateTimeOffset.UtcNow;
                
                var orderDuration = calculateAt - CreatedAt;
                var durationInHours = (float)Math.Round(orderDuration.TotalMinutes / 60.0, 2, MidpointRounding.AwayFromZero);;
                return (decimal) Math.Max(1, durationInHours) * StoragePrice;
            }
         
            return Details.Sum(item => item.Quantity != null 
                ? item.Price * (decimal) item.Quantity.Value 
                : 0);
        }
    }
    
    public decimal ShippingFee { get; set; }
    
    public decimal TotalPrice { get; set; }

    public string? CustomerNote { get; set; }
    
    public string? StaffNote { get; set; }
    
    public string? Description { get; set; }

    public long? DeliveryAddressId { get; set; }

    // Địa chỉ giao trả đồ nếu có
    public Location? DeliveryAddress { get; set; }

    public IList<OrderTimeline> Timelines { get; set; } = new List<OrderTimeline>();

    public IList<OrderDetail> Details { get; set; } = new List<OrderDetail>();

    public IList<Payment> Payments { get; set; } = new List<Payment>();

    [Projectable] 
    public bool IsFinished => IsCanceled || IsCompleted;

    [Projectable] 
    public bool IsCompleted => OrderStatus.Completed.Equals(Status);

    [Projectable] 
    public bool IsInitialized => OrderStatus.Initialized.Equals(Status);

    [Projectable] 
    public bool IsReserved => OrderStatus.Reserved.Equals(Status);

    [Projectable] 
    public bool IsWaiting => OrderStatus.Waiting.Equals(Status);

    [Projectable] 
    public bool IsCollected => OrderStatus.Collected.Equals(Status);

    [Projectable] 
    public bool IsProcessing => OrderStatus.Processing.Equals(Status);
    
    [Projectable] 
    public bool IsReturned => OrderStatus.Returned.Equals(Status);

    [Projectable] 
    public bool IsOvertime => OrderStatus.Overtime.Equals(Status);

    [Projectable]
    public bool IsCanceled => OrderStatus.Canceled.Equals(Status);

    [Projectable]
    public bool IsProcessed => OrderStatus.Processed.Equals(Status);
    
    [Projectable]
    public bool IsUpdating => OrderStatus.Updating.Equals(Status);

    [Projectable]
    public bool IsOvertimeProcessing => OrderStatus.OvertimeProcessing.Equals(Status);
    
    public bool UpdatedInfo => OrderType.Storage.Equals(Type) || (Details.Any() && Details.All(detail => detail.Quantity != null));

    /**
     * Order status, which has not finished yet
     */
    [Projectable]
    public bool IsActive => !IsFinished;

    /**
     * Order status, which takes a box in the locker
     */
    [Projectable]
    public bool IsBusyOrder => IsReserved || IsWaiting || IsReturned || IsOvertime;
    
    [Projectable] 
    public bool DeliverySupported => DeliveryAddressId != null;

    [NotMapped] 
    [Projectable]
    public bool IsLaundry => Equals(OrderType.Laundry, Type);

    [Projectable]
    public bool NotifyStaff => Equals(OrderType.Laundry, Type);
    

    [NotMapped] 
    [Projectable]
    public bool IsStorage => Equals(OrderType.Storage, Type);

    public bool CanUpdateStatus(OrderStatus status)
    {
        if (Equals(status, OrderStatus.Initialized))
        {
            return false;
        }
        
        switch (status)
        {
            case OrderStatus.Waiting:
                return IsInitialized || IsUpdating;
        
            case OrderStatus.Collected:
                return IsLaundry && IsWaiting;
        
            case OrderStatus.Processing:
                return IsLaundry && IsCollected;
            
            case OrderStatus.Processed:
                return IsLaundry && (IsProcessing || IsCollected) && UpdatedInfo;
        
            case OrderStatus.Returned:
                return IsLaundry && IsProcessed;
        
            case OrderStatus.Completed:
                return (OrderType.Storage.Equals(Type) && IsWaiting)
                       || (OrderType.Laundry.Equals(Type) && IsReturned)
                       || IsOvertime
                       || IsOvertimeProcessing;
            
            case OrderStatus.Canceled:
                return IsReserved;
            
            case OrderStatus.Updating:
                return IsWaiting;
            
            case OrderStatus.OvertimeProcessing:
                return IsOvertime;
            
            default:
                return false;
        }
    }

    public Order()
    {
        ReferenceId = Guid.NewGuid().ToString();
    }

    public decimal CalculateTotalPrice()
    {
        return Price
               + (decimal) ExtraCount * ExtraFee
               + ShippingFee
               - Discount;
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EntityFrameworkCore.Projectables;
using LockerService.Domain.Entities;
using LockerService.Domain.Enums;

namespace LockerService.Domain;

[Table("Payment")]
public class Payment : BaseAuditableEntity
{
    [Key]
    public long Id { get; set; }
    
    public string? ReferenceId { get; set; }
    
    public decimal Amount { get; set; }

    public PaymentMethod Method { get; set; }
    
    public string? Content { get; set; }
    
    public string? ReferenceTransactionId { get; set; }
    
    public string? Qr { get; set; }
    
    public string? Url { get; set; }
    
    public string? Deeplink { get; set; }
    
    public long? OrderId { get; set; }

    public Order? Order { get; set; } = default!;
    
    public long? StoreId { get; set; }

    public Store? Store { get; set; } = default!;
    
    public long CustomerId { get; set; }

    public Account Customer { get; set; } = default!;
    
    public string? Description { get; set; }
    
    public PaymentStatus Status { get; set; }

    public PaymentType Type { get; set; }
    
    public Payment()
    {
        Status = PaymentStatus.Created;
        ReferenceId = Guid.NewGuid().ToString();
    }
    
    public Payment(Order order, PaymentMethod method, PaymentStatus? status = PaymentStatus.Created)
    {
        Status = status ?? PaymentStatus.Created;
        ReferenceId = Guid.NewGuid().ToString();
        Amount = order.CalculateTotalPrice();
        Method = method;
        Content = PaymentContent(order.Type);
        OrderId = order.Id;
        CustomerId = order.ReceiverId ?? order.SenderId;
        StoreId = order.Locker.StoreId;
    }

    public static string PaymentContent(OrderType type)
    {
        return type switch
        {
            OrderType.Laundry => "Thanh toán dịch vụ giặt sấy",
            OrderType.Storage => "Thanh toán dịch vụ gửi đồ",
            _ => string.Empty
        };
    }

    [Projectable]
    public bool Completed => Equals(Status, PaymentStatus.Completed);

    [Projectable]
    public bool Created => Equals(Status, PaymentStatus.Created);
}
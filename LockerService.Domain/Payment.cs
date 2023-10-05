using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LockerService.Domain.Entities;
using LockerService.Domain.Enums;

namespace LockerService.Domain;

[Table("Payment")]
public class Payment : BaseAuditableEntity
{
    [Key]
    public long Id { get; set; }
    
    public decimal Amount { get; set; }

    public PaymentMethod Method { get; set; }
    
    public string? Content { get; set; }
    
    public string? ReferenceTransactionId { get; set; }
    
    public string? Qr { get; set; }
    
    public string? Url { get; set; }
    
    public long OrderId { get; set; }

    public Order Order { get; set; } = default!;
    
    public long CustomerId { get; set; }

    public Account Customer { get; set; } = default!;
    
    public PaymentStatus Status { get; set; }

    public Payment()
    {
        Status = PaymentStatus.Created;
    }

    public Payment(Order order, PaymentMethod method)
    {
        Status = PaymentStatus.Created;
        Amount = order.Price + order.TotalExtraFee + order.ShippingFee - order.Discount - order.ReservationFee;
        Method = method;
        Content = PaymentContent(order.Type);
        OrderId = order.Id;
        CustomerId = order.ReceiverId ?? order.SenderId;
    }

    private string PaymentContent(OrderType type)
    {
        return type switch
        {
            OrderType.Laundry => "Thanh toán dịch vụ giặt sấy",
            OrderType.Storage => "Thanh toán dịch vụ gửi đồ",
            _ => string.Empty
        };
    }
}
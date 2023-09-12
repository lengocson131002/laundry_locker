using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LockerService.Domain.Common.Constants;
using LockerService.Domain.Enums;

namespace LockerService.Domain.Entities;

[Table("Bill")]
public class Bill : BaseAuditableEntity
{
    [Key]
    public long Id { get; set; }
    
    public long ReferenceOrderId { get; set; }
    
    public decimal Amount { get; set; }

    public decimal Prepaid { get; set; }
    
    public PaymentMethod Method { get; set; }
    
    public string? Content { get; set; }
    
    public string? ReferenceTransactionId { get; set; }
    
    public string? Qr { get; set; }
    
    public string? CheckoutUrl { get; set; }


    public static Bill CreateBill(Order order, PaymentMethod method)
    {
        return new Bill()
        {
            ReferenceOrderId = order.Id,
            Prepaid = order.ReservationFee,
            Amount = order.Price
                     + (decimal)order.ExtraCount * order.ExtraFee
                     - order.Discount,
            Method = method,
            Content = Equals(order.Type, OrderType.Storage) 
                ? BillConstants.BillContentStorageOrder 
                : BillConstants.BillContentLaundryOrder,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }
}
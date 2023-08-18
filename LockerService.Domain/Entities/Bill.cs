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

    public PaymentMethod Method { get; set; }
    
    public string? Content { get; set; }
    
    public string? ReferenceTransactionId { get; set; }

    public static Bill CreateBill(Order order, PaymentMethod method)
    {
        if (!order.IsCompleted || order.TotalPrice == null)
        {
            throw new Exception("Order was not completed");
        }
        return new Bill()
        {
            ReferenceOrderId = order.Id,
            Amount = order.TotalPrice.Value,
            Method = method,
            Content = Equals(order.Type, OrderType.Storage) 
                ? BillConstants.BillContentStorageOrder 
                : BillConstants.BillContentLaundryOrder
        };
    }
}
namespace LockerService.Application.Bills.Models;

public class BillResponse : BaseAuditableEntityResponse
{
    public long Id { get; set; }
    
    public long ReferenceOrderId { get; set; }
    
    public decimal Amount { get; set; }

    public PaymentMethod Method { get; set; }
    
    public string? Content { get; set; }
    
    public string? ReferenceTransactionId { get; set; }

}
namespace LockerService.Application.Features.Payments.Commands;

public class TestPaymentCommand : IRequest<StatusResponse>
{
    public decimal Amount { get; set; }
    
    public long OrderId { get; set; }
    
    public long CustomerId { get; set; }
    
    public PaymentMethod Method { get; set; }
}
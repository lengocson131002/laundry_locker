namespace LockerService.Application.Orders.Commands;

public class UpdateOrderValidation : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderValidation()
    {
        RuleFor(request => request.Amount)
            .Must(amount => amount == null || amount > 0)
            .WithMessage("Amount must > 0");
        
        RuleFor(request => request.Fee)
            .Must(fee => fee == null || fee > 0)
            .WithMessage("Fee must > 0");
    }
}

public class UpdateOrderCommand : IRequest<OrderResponse>
{
    [JsonIgnore]
    public int Id { get; set; }
    public double? Amount { get; set; }
    
    public double? Fee { get; set; }
    
    public string? Description { get; set; }
}
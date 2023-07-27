namespace LockerService.Application.Customers.Commands;

public class UpdateCustomerStatusCommandValidator: AbstractValidator<UpdateCustomerStatusCommand> 
{
    public UpdateCustomerStatusCommandValidator()
    {
        RuleFor(model => model.Status)
            .IsInEnum()
            .NotNull();
    }
}
public class UpdateCustomerStatusCommand : IRequest
{
    [JsonIgnore]
    public long Id { get; set; }
    
    public AccountStatus Status { get; set; }
}
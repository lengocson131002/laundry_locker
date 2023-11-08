namespace LockerService.Application.Features.Customers.Commands;

public class UpdateCustomerCommandValidator: AbstractValidator<UpdateCustomerCommand> 
{
    public UpdateCustomerCommandValidator()
    {
    }
}
public class UpdateCustomerCommand : IRequest
{
    [JsonIgnore]
    public long Id { get; set; }
    
    public AccountStatus? Status { get; set; }
}
namespace LockerService.Application.Lockers.Commands;

public class AddBoxCommandValidator : AbstractValidator<AddBoxCommand>
{
    public AddBoxCommandValidator()
    {
        RuleFor(model => model.BoxNumber)
            .GreaterThan(0);
    }
}
public class AddBoxCommand : IRequest<StatusResponse>
{
    [JsonIgnore]
    public long LockerId { get; set; }
    
    public int BoxNumber { get; set; }
}
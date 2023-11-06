namespace LockerService.Application.Features.Lockers.Commands;


public class OpenBoxCommandValidator : AbstractValidator<OpenBoxCommand>
{
    public OpenBoxCommandValidator()
    {
        RuleFor(model => model.LockerId)
            .GreaterThan(0);
        
        RuleFor(model => model.BoxNumber)
            .GreaterThan(0);

        RuleFor(model => model.Token)
            .NotEmpty();
    }
}


public class OpenBoxCommand : IRequest<StatusResponse>
{
    [JsonIgnore]
    [BindNever]
    public long LockerId { get; set; }
    
    public int BoxNumber { get; set; }

    public string Token { get; set; } = default!;
}
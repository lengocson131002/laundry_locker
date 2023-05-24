using LockerService.Application.Common.Extensions;

namespace LockerService.Application.Orders.Commands;

public class CreateOrderValidation : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidation()
    {
        RuleFor(req => req.LockerId).NotEmpty();
        RuleFor(req => req.ServiceId).NotEmpty();
        RuleFor(req => req.OrderPhone)
            .NotNull()
            .Must(x => x.IsValidPhoneNumber())
            .WithMessage("Invalid order phone number");

        RuleFor(req => req.ReceivePhone)
            .Must(x => string.IsNullOrWhiteSpace(x) || x.IsValidPhoneNumber())
            .WithMessage("Invalid receive phone number");

        RuleFor(req => req.ReceiveTime)
            .Must(x => x == null || ((DateTimeOffset)x).UtcDateTime > DateTimeOffset.UtcNow)
            .WithMessage("Received time must be later than current now");
    }
}

public class CreateOrderCommand : IRequest<OrderResponse>
{
    public int LockerId { get; set; }
    
    public int ServiceId { get; set; }

    private string _oPhone = default!;
    
    public string OrderPhone
    {
        get => this._oPhone;
        set => this._oPhone = value.Trim();
    }

    private string? _rPhone;
    
    public string? ReceivePhone
    {
        get => this._rPhone;
        set => this._rPhone = value?.Trim();
    }

    private DateTimeOffset? _receiveTime;
    
    public DateTimeOffset? ReceiveTime
    {
        get => this._receiveTime;
        set => this._receiveTime = value?.ToUniversalTime();
    }
}
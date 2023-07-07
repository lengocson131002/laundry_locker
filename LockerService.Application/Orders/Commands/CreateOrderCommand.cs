using LockerService.Application.Common.Extensions;

namespace LockerService.Application.Orders.Commands;

public class CreateOrderValidation : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidation()
    {
        RuleFor(req => req.LockerId)
            .NotNull();
        RuleFor(req => req.Type)
            .NotNull();
        RuleFor(req => req.OrderPhone)
            .NotNull()
            .Must(x => x.IsValidPhoneNumber())
            .WithMessage("Invalid order phone number");

        RuleFor(req => req.ReceivePhone)
            .Must(x => x == null || x.IsValidPhoneNumber())
            .WithMessage("Invalid receive phone number");

        RuleFor(model => model.ServiceIds)
            .Must(serviceIds => serviceIds.Any())
            .When(order => OrderType.Laundry.Equals(order.Type))
            .WithMessage("Services is required for landry type");
    }
}

public class CreateOrderCommand : IRequest<OrderResponse>
{
    public int LockerId { get; set; }
    
    public OrderType Type { get; set; }
    
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

    public IList<int> ServiceIds { get; set; } = new List<int>();
}
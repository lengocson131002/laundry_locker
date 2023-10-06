using LockerService.Application.Features.Orders.Models;

namespace LockerService.Application.Features.Orders.Commands;

public class TakeReservationCommandValidator : AbstractValidator<TakeReservationCommand>
{
    public TakeReservationCommandValidator()
    {
        RuleFor(model => model.Id)
            .NotNull();
    }
}

public class TakeReservationCommand : IRequest<OrderResponse>
{
    public long Id { get; set; } = default!;
}
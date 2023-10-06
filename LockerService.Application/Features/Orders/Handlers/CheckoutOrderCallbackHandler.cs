using LockerService.Application.Features.Orders.Commands;

namespace LockerService.Application.Features.Orders.Handlers;

public class CheckoutOrderCallbackHandler : IRequestHandler<CheckoutOrderCallbackCommand>
{
    public Task Handle(CheckoutOrderCallbackCommand request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
namespace LockerService.Application.Orders.Handlers;

public class CheckoutOrderCallbackHandler : IRequestHandler<CheckoutOrderCallbackCommand>
{
    public Task Handle(CheckoutOrderCallbackCommand request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
using LockerService.Application.Customers.Commands;

namespace LockerService.Application.Customers.Handlers;

public class UpdateCustomerStatusHandler : IRequestHandler<UpdateCustomerStatusCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerStatusHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateCustomerStatusCommand request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.AccountRepository
            .Get(predicate: cus => cus.Id == request.Id && Equals(cus.Role, Role.Customer))
            .FirstOrDefaultAsync(cancellationToken);
        
        if (customer is null)
        {
            throw new ApiException(ResponseCode.AuthErrorAccountNotFound);
        }

        if (Equals(customer.Status, request.Status) || Equals(customer.Status, AccountStatus.Verifying))
        {
            throw new ApiException(ResponseCode.AccountErrorInvalidStatus);
        }

        customer.Status = request.Status;
        await _unitOfWork.AccountRepository.UpdateAsync(customer);

        // Save changes
        await _unitOfWork.SaveChangesAsync();
    }
}
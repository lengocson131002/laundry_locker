using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Customers.Commands;

namespace LockerService.Application.Features.Customers.Handlers;

public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.AccountRepository
            .Get(predicate: cus => cus.Id == request.Id && Equals(cus.Role, Role.Customer))
            .FirstOrDefaultAsync(cancellationToken);
        
        if (customer == null)
        {
            throw new ApiException(ResponseCode.AuthErrorAccountNotFound);
        }

        if (request.Status != null)
        {
            if (!customer.CanUpdateStatus(request.Status.Value))
            {
                throw new ApiException(ResponseCode.AccountErrorInvalidStatus);
            }

            customer.Status = request.Status.Value;
        }
        
        await _unitOfWork.AccountRepository.UpdateAsync(customer);

        // Save changes
        await _unitOfWork.SaveChangesAsync();
    }
}
using LockerService.Application.Services.Commands;

namespace LockerService.Application.Services.Handlers;

public class UpdateServiceStatusHandler : IRequestHandler<UpdateServiceStatusCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateServiceStatusHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateServiceStatusCommand request, CancellationToken cancellationToken)
    {
        var service = await _unitOfWork.ServiceRepository
            .GetStoreService(request.StoreId, request.ServiceId);
        
        if (service is null)
        {
            throw new ApiException(ResponseCode.ServiceErrorNotFound);
        }

        if (request.Status.Equals(service.Status))
        {
            throw new ApiException(ResponseCode.ServiceErrorInvalidStatus);
        }

        service.Status = request.Status;
        await _unitOfWork.ServiceRepository.UpdateAsync(service);
        await _unitOfWork.SaveChangesAsync();
    }
}
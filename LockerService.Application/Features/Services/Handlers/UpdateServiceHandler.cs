using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Services.Commands;

namespace LockerService.Application.Features.Services.Handlers;

public class UpdateServiceHandler : IRequestHandler<UpdateServiceCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateServiceHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }


    public async Task Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await _unitOfWork.ServiceRepository
            .GetStoreService(request.StoreId, request.ServiceId);
        
        if (service is null)
        {
            throw new ApiException(ResponseCode.ServiceErrorNotFound);
        }

        if (request.Name != null)
        {
            service.Name = request.Name;
        }

        if (request.Price != null)
        {
            service.Price = request.Price.Value;
        }

        if (request.Description != null)
        {
            service.Description = request.Description;
        }

        if (request.Image != null)
        {
            service.Image = request.Image;
        }

        if (request.Unit != null)
        {
            service.Unit = request.Unit;
        }
        
        await _unitOfWork.ServiceRepository.UpdateAsync(service);
        
        // Save changes
        await _unitOfWork.SaveChangesAsync();
    }
}
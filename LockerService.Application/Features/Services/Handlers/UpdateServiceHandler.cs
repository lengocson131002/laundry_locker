using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Services.Commands;
using LockerService.Application.Features.Services.Models;
using LockerService.Application.Features.Stores.Models;

namespace LockerService.Application.Features.Services.Handlers;

public class UpdateServiceHandler : IRequestHandler<UpdateServiceCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;
    
    public UpdateServiceHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }


    public async Task Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await _unitOfWork.ServiceRepository.GetByIdAsync(request.ServiceId);
        
        if (service == null || !service.IsStandard)
        {
            throw new ApiException(ResponseCode.ServiceErrorNotFound);
        }
        
        service.Name = request.Name ?? service.Name;
        service.Price = request.Price ?? service.Price;
        service.Description = request.Description ?? service.Description;
        service.Image = request.Image ?? service.Image;
        service.Unit = request.Unit ?? service.Unit;
        service.Status = request.Status ?? service.Status;
        
        await _unitOfWork.ServiceRepository.UpdateAsync(service);
        await _unitOfWork.SaveChangesAsync();

    }
}
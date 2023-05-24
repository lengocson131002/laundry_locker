using LockerService.Application.Services.Commands;

namespace LockerService.Application.Services.Handlers;

public class UpdateServiceHandler : IRequestHandler<UpdateServiceCommand>
{
    private readonly ILogger<UpdateServiceHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateServiceHandler(
        ILogger<UpdateServiceHandler> logger,
        IUnitOfWork unitOfWork, 
        IMapper mapper)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }


    public async Task Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        if (request.ServiceId is null)
        {
            throw new ApiException(ResponseCode.ServiceErrorNotFound);
        }

        var service = await _unitOfWork.ServiceRepository.GetByIdAsync(request.ServiceId);
        if (service is null || !Equals(service.LockerId, request.LockerId))
        {
            throw new ApiException(ResponseCode.ServiceErrorNotFound);
        }

        if (!string.IsNullOrEmpty(request.Name))
        {
            service.Name = request.Name;
        }

        service.Fee = request.Fee;

        if (request.FeeType is not null)
        {
            service.FeeType = (FeeType)request.FeeType;
        }

        if (!string.IsNullOrEmpty(request.Description))
        {
            service.Description = request.Description;
        }

        if (!string.IsNullOrEmpty(request.Image))
        {
            service.Image = request.Image;
        }
        
        if (!FeeType.ByInputPrice.Equals(service.FeeType) && service.Fee == null)
        {
            throw new ApiException(ResponseCode.ServiceErrorMissingFee);
        }
        
        await _unitOfWork.ServiceRepository.UpdateAsync(service);
        
        // Save changes
        await _unitOfWork.SaveChangesAsync();
    }
}
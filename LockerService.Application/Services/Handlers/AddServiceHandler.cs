using LockerService.Application.Services.Commands;

namespace LockerService.Application.Services.Handlers;

public class AddServiceHandler : IRequestHandler<AddServiceCommand, ServiceResponse>
{
    private readonly ILogger<AddServiceHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public AddServiceHandler(
        ILogger<AddServiceHandler> logger,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }


    public async Task<ServiceResponse> Handle(AddServiceCommand request, CancellationToken cancellationToken)
    {
        var locker = await _unitOfWork.LockerRepository.GetByIdAsync(request.LockerId);
        if (locker is null)
        {
            throw new ApiException(ResponseCode.LockerErrorNotFound);
        }

        var service = _mapper.Map<Service>(request);
        service.Locker = locker;
        service.LockerId = request.LockerId;
        service = await _unitOfWork.ServiceRepository.AddAsync(service);
        
        // Save changes 
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ServiceResponse>(service);
    }
}
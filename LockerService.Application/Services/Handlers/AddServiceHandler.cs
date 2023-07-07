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
        var service = _mapper.Map<Service>(request);
        
        // check exist name
        var query = await _unitOfWork.ServiceRepository.GetAsync(
            predicate: ser => ser.Name.Equals(service.Name, StringComparison.OrdinalIgnoreCase)
        );

        if (query.Any())
        {
            throw new ApiException(ResponseCode.ServiceErrorExistedName);
        }
        
        service = await _unitOfWork.ServiceRepository.AddAsync(service);
        
        // Save changes 
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ServiceResponse>(service);
    }
}
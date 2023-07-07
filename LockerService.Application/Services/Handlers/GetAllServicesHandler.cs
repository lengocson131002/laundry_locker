using LockerService.Application.Services.Queries;

namespace LockerService.Application.Services.Handlers;

public class GetAllServicesHandler : IRequestHandler<GetAllServicesQuery, PaginationResponse<Service, ServiceResponse>>
{
    private readonly ILogger<AddServiceHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetAllServicesHandler(
        ILogger<AddServiceHandler> logger,
        IMapper mapper, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }


    public async Task<PaginationResponse<Service, ServiceResponse>> Handle(GetAllServicesQuery request,
        CancellationToken cancellationToken)
    {
        var service = await _unitOfWork.ServiceRepository.GetAsync(
            predicate: request.GetExpressions(),
            orderBy: request.GetOrder()
        );

        return new PaginationResponse<Service, ServiceResponse>(
            service,
            request.PageNumber,
            request.PageSize,
            entity => _mapper.Map<ServiceResponse>(entity));
    }
}
using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Services.Models;
using LockerService.Application.Features.Services.Queries;

namespace LockerService.Application.Features.Services.Handlers;

public class GetAllServicesHandler : IRequestHandler<GetAllServicesQuery, PaginationResponse<Service, ServiceResponse>>
{
    private readonly ILogger<AddServiceHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentAccountService _currentAccountService;

    public GetAllServicesHandler(
        ILogger<AddServiceHandler> logger,
        IMapper mapper, IUnitOfWork unitOfWork, 
        ICurrentAccountService currentAccountService)
    {
        _logger = logger;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _currentAccountService = currentAccountService;
    }


    public async Task<PaginationResponse<Service, ServiceResponse>> Handle(GetAllServicesQuery request,
        CancellationToken cancellationToken)
    {
        var lockerId = request.LockerId;
        
        /*
         * Check current logged in user
         * if Store Staff, get only their store's locker
         */
        var currentLoggedInAccount = await _currentAccountService.GetCurrentAccount();
        if (currentLoggedInAccount != null && currentLoggedInAccount.IsStoreStaff)
        {
            request.StoreId = currentLoggedInAccount.StoreId;
        }
        
        if (lockerId != null)
        {
            var locker = await _unitOfWork.LockerRepository.GetByIdAsync(lockerId);
            if (locker == null)
            {
                throw new ApiException(ResponseCode.LockerErrorNotFound);
            }
            request.StoreId = locker.StoreId;
        }
        
        var service = await _unitOfWork.ServiceRepository.GetAsync(
            predicate: request.GetExpressions(),
            orderBy: request.GetOrder(),
            disableTracking: true
        );

        return new PaginationResponse<Service, ServiceResponse>(
            service,
            request.PageNumber,
            request.PageSize,
            entity => _mapper.Map<ServiceResponse>(entity));
    }
}
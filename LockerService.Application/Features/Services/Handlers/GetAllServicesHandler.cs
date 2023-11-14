using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Services.Models;
using LockerService.Application.Features.Services.Queries;

namespace LockerService.Application.Features.Services.Handlers;

public class GetAllServicesHandler : IRequestHandler<GetAllServicesQuery, PaginationResponse<Service, ServiceResponse>>
{
    private readonly ICurrentAccountService _currentAccountService;
    private readonly ILogger<AddServiceHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

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


    public async Task<PaginationResponse<Service, ServiceResponse>> Handle(GetAllServicesQuery request, CancellationToken cancellationToken)
    {
        var lockerId = request.LockerId;
        if (lockerId != null)
        {
            var locker = await _unitOfWork.LockerRepository.GetByIdAsync(lockerId);
            if (locker == null) throw new ApiException(ResponseCode.LockerErrorNotFound);
            request.StoreId = locker.StoreId;
        }

        var serviceQuery = await _unitOfWork.ServiceRepository.GetAsync(
            predicate: request.GetExpressions(), 
            orderBy: request.GetOrder());

        if (request.StoreId != null)
        {
            /*
             * Join between StoreService and Service table to get store service's prices
            */

            var storeServices = await _unitOfWork.StoreServiceRepository.GetAsync();

            serviceQuery = (from service in serviceQuery
                join storeService in storeServices on new { ServiceId = service.Id, StoreId = request.StoreId.Value }
                    equals new { storeService.ServiceId, storeService.StoreId } into configs
                from config in configs.DefaultIfEmpty()
                select new Service()
                {  
                    Id = service.Id,
                    Name = service.Name,
                    Image = service.Image,
                    Unit = service.Unit,
                    Description = service.Description,
                    Status = service.Status,
                    StoreId = service.StoreId,
                    Store = service.Store,
                    CreatedAt = service.CreatedAt,
                    CreatedBy = service.CreatedBy,
                    CreatedByUsername = service.CreatedByUsername,
                    UpdatedAt = service.UpdatedAt,
                    UpdatedBy = service.UpdatedBy,
                    UpdatedByUsername = service.UpdatedByUsername,
                    DeletedAt = service.DeletedAt,
                    DeletedBy = service.DeletedBy,
                    DeletedByUsername = service.DeletedByUsername,
                    Price = config != null ? config.Price : service.Price,
                }).AsNoTracking();
        }

        return new PaginationResponse<Service, ServiceResponse>(
            serviceQuery,
            request.PageNumber,
            request.PageSize,
            entity => _mapper.Map<ServiceResponse>(entity));
    }
}
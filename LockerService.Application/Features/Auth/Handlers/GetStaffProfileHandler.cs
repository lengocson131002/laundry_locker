using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Auth.Queries;
using LockerService.Application.Features.Staffs.Models;

namespace LockerService.Application.Features.Auth.Handlers;

public class GetStaffProfileHandler : IRequestHandler<GetStaffProfileQuery, StaffResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<GetStaffProfileHandler> _logger;
    private readonly ICurrentPrincipalService _currentPrincipalService;
    private readonly IMapper _mapper;

    public GetStaffProfileHandler(
        IUnitOfWork unitOfWork, 
        ILogger<GetStaffProfileHandler> logger,
        IJwtService jwtService, 
        IMapper mapper, 
        ICurrentPrincipalService currentPrincipalService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _jwtService = jwtService;
        _mapper = mapper;
        _currentPrincipalService = currentPrincipalService;
    }

    public async Task<StaffResponse> Handle(GetStaffProfileQuery request, CancellationToken cancellationToken)
    {
        var accountId = _currentPrincipalService.CurrentSubjectId;
        if (accountId == null)
        {
            throw new ApiException(ResponseCode.Unauthorized);
        }

        var account = await _unitOfWork.AccountRepository
            .Get(acc => acc.Id == accountId)
            .Include(acc => acc.Store)
            .Include(acc => acc.Store.Location)
            .Include(acc => acc.Store.Location.Province)
            .Include(acc => acc.Store.Location.District)
            .Include(acc => acc.Store.Location.Ward)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (account == null)
        {
            throw new ApiException(ResponseCode.Unauthorized);

        }

        return  _mapper.Map<StaffResponse>(account);
    }
}
using LockerService.Application.Auth.Queries;

namespace LockerService.Application.Auth.Handlers;

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
            .FirstOrDefaultAsync(cancellationToken);
        
        if (account == null)
        {
            throw new ApiException(ResponseCode.Unauthorized);

        }

        return  _mapper.Map<StaffResponse>(account);
    }
}
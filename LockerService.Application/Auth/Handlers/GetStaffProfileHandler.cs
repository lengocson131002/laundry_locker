using LockerService.Application.Auth.Queries;

namespace LockerService.Application.Auth.Handlers;

public class GetStaffProfileHandler : IRequestHandler<GetStaffProfileQuery, StaffResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<GetStaffProfileHandler> _logger;
    private readonly ICurrentAccountService _currentAccountService;
    private readonly IMapper _mapper;

    public GetStaffProfileHandler(IUnitOfWork unitOfWork, ILogger<GetStaffProfileHandler> logger,
        IJwtService jwtService, ICurrentAccountService currentAccountService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _currentAccountService = currentAccountService;
        _jwtService = jwtService;
        _mapper = mapper;
    }

    public async Task<StaffResponse> Handle(GetStaffProfileQuery request, CancellationToken cancellationToken)
    {
        var account = await _currentAccountService.GetCurrentAccount();
        if (account is null)
        {
            throw new ApiException(ResponseCode.AuthErrorAccountNotFound);
        }

        return  _mapper.Map<StaffResponse>(account);
    }
}
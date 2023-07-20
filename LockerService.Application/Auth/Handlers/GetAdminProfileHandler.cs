using LockerService.Application.Auth.Queries;

namespace LockerService.Application.Auth.Handlers;

public class GetAdminProfileHandler : IRequestHandler<GetAdminProfileQuery, AccountResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<GetAdminProfileHandler> _logger;
    private readonly ICurrentPrincipalService _currentUserService;
    private readonly IMapper _mapper;

    public GetAdminProfileHandler(IUnitOfWork unitOfWork, ILogger<GetAdminProfileHandler> logger,
        IJwtService jwtService, ICurrentPrincipalService currentUserService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _currentUserService = currentUserService;
        _jwtService = jwtService;
        _mapper = mapper;
    }

    public async Task<AccountResponse> Handle(GetAdminProfileQuery request, CancellationToken cancellationToken)
    {
        var accountId = _currentUserService.CurrentPrincipal;
        if (accountId is null)
        {
            throw new ApiException(ResponseCode.AuthErrorAccountNotFound);
        }

        var accountQuery = await _unitOfWork.AccountRepository.GetAsync(
            predicate: account => Equals(long.Parse(accountId), account.Id)
        );

        var account = accountQuery.FirstOrDefault();
        if (account is null)
        {
            throw new ApiException(ResponseCode.AuthErrorAccountNotFound);
        }

        return _mapper.Map<AccountResponse>(account);
    }
}
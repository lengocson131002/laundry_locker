using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Accounts.Models;
using LockerService.Application.Features.Auth.Queries;

namespace LockerService.Application.Features.Auth.Handlers;

public class GetAdminProfileHandler : IRequestHandler<GetAdminProfileQuery, AccountResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<GetAdminProfileHandler> _logger;
    private readonly ICurrentAccountService _currentAccountService;
    private readonly IMapper _mapper;

    public GetAdminProfileHandler(IUnitOfWork unitOfWork, ILogger<GetAdminProfileHandler> logger,
        IJwtService jwtService, ICurrentAccountService currentAccountService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _currentAccountService = currentAccountService;
        _jwtService = jwtService;
        _mapper = mapper;
    }

    public async Task<AccountResponse> Handle(GetAdminProfileQuery request, CancellationToken cancellationToken)
    {
        var account = await _currentAccountService.GetCurrentAccount();
        if (account is null)
        {
            throw new ApiException(ResponseCode.AuthErrorAccountNotFound);
        }

        return _mapper.Map<AccountResponse>(account);
    }
}
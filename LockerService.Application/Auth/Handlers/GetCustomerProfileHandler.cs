using LockerService.Application.Auth.Queries;

namespace LockerService.Application.Auth.Handlers;

public class GetCustomerProfileHandler : IRequestHandler<GetCustomerProfileQuery, CustomerResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<GetCustomerProfileHandler> _logger;
    private readonly ICurrentAccountService _currentAccountService;
    private readonly IMapper _mapper;

    public GetCustomerProfileHandler(IUnitOfWork unitOfWork, ILogger<GetCustomerProfileHandler> logger,
        IJwtService jwtService, ICurrentAccountService currentAccountService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _currentAccountService = currentAccountService;
        _jwtService = jwtService;
        _mapper = mapper;
    }

    public async Task<CustomerResponse> Handle(GetCustomerProfileQuery request, CancellationToken cancellationToken)
    {
        var account = await _currentAccountService.GetCurrentAccount();
        if (account is null)
        {
            throw new ApiException(ResponseCode.AuthErrorAccountNotFound);
        }

        return _mapper.Map<CustomerResponse>(account);
    }
}
using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Auth.Queries;
using LockerService.Application.Features.Customers.Models;

namespace LockerService.Application.Features.Auth.Handlers;

public class GetCustomerProfileHandler : IRequestHandler<GetCustomerProfileQuery, CustomerResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<GetCustomerProfileHandler> _logger;
    private readonly ICurrentPrincipalService _currentPrincipalService;
    private readonly IMapper _mapper;

    public GetCustomerProfileHandler(
        IUnitOfWork unitOfWork, 
        ILogger<GetCustomerProfileHandler> logger,
        IJwtService jwtService, 
        ICurrentPrincipalService currentPrincipalService, 
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _currentPrincipalService = currentPrincipalService;
        _jwtService = jwtService;
        _mapper = mapper;
    }

    public async Task<CustomerResponse> Handle(GetCustomerProfileQuery request, CancellationToken cancellationToken)
    {
        var currentAccountId = _currentPrincipalService.CurrentSubjectId;
        if (currentAccountId == null)
        {
            throw new ApiException(ResponseCode.Unauthorized);
        }

        var customer = await _unitOfWork.AccountRepository
            .Get(acc => acc.Id == currentAccountId)
            .Include(acc => acc.Wallet)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        
        return _mapper.Map<CustomerResponse>(customer);
    }
}
using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Accounts.Models;
using LockerService.Application.Features.Auth.Commands;

namespace LockerService.Application.Features.Auth.Handlers;

public class UpdateStaffProfileHandler : IRequestHandler<UpdateStaffProfileCommand, AccountResponse>
{
    private readonly ICurrentAccountService _currentAccountService;
    private readonly IJwtService _jwtService;
    private readonly ILogger<UpdateStaffProfileHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStaffProfileHandler(IUnitOfWork unitOfWork, ILogger<UpdateStaffProfileHandler> logger,
        IJwtService jwtService,
        ICurrentAccountService currentUserService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _jwtService = jwtService;
        _currentAccountService = currentUserService;
        _mapper = mapper;
    }

    public async Task<AccountResponse> Handle(UpdateStaffProfileCommand request, CancellationToken cancellationToken)
    {
        var account = await _currentAccountService.GetCurrentAccount();
        if (account is null) throw new ApiException(ResponseCode.Unauthorized);

        // Check username
        if (request.Username != null && !Equals(request.Username, account.Username))
        {
            var checkStaff = await _unitOfWork.AccountRepository
                .Get(acc => Equals(acc.Username, request.Username))
                .FirstOrDefaultAsync(cancellationToken);

            if (checkStaff != null)
            {
                throw new ApiException(ResponseCode.AccountErrorUsernameExisted);
            }
        }

        account.Avatar = request.Avatar ?? account.Avatar;
        account.FullName = request.FullName ?? account.FullName;
        account.Description = request.Description ?? account.Description;
        account.Avatar = request.Avatar ?? account.Avatar;
        account.Username = request.Username ?? account.Username;
        account.PhoneNumber = request.PhoneNumber ?? account.PhoneNumber;

        await _unitOfWork.AccountRepository.UpdateAsync(account);

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<AccountResponse>(account);
    }
}
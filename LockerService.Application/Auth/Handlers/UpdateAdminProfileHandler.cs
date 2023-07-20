namespace LockerService.Application.Auth.Handlers;

public class UpdateAdminProfileHandler : IRequestHandler<UpdateAdminProfileCommand, AccountResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<UpdateAdminProfileHandler> _logger;
    private readonly ICurrentPrincipalService _currentUserService;
    private readonly IMapper _mapper;

    public UpdateAdminProfileHandler(IUnitOfWork unitOfWork, ILogger<UpdateAdminProfileHandler> logger,
        IJwtService jwtService,
        ICurrentPrincipalService currentUserService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _jwtService = jwtService;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<AccountResponse> Handle(UpdateAdminProfileCommand request, CancellationToken cancellationToken)
    {
        // Check username
        var checkUsernameQuery = await _unitOfWork.AccountRepository.GetAsync(
            predicate: account => Equals(account.Username, request.Username)
        );

        if (checkUsernameQuery.FirstOrDefault() is not null)
        {
            throw new ApiException(ResponseCode.AccountErrorUsernameExisted);
        }

        // Check phone
        var checkPhoneQuery = await _unitOfWork.AccountRepository.GetAsync(
            predicate: account => Equals(account.Username, request.Username)
        );

        if (checkPhoneQuery.FirstOrDefault() is not null)
        {
            throw new ApiException(ResponseCode.AccountErrorPhoneNumberExisted);
        }

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
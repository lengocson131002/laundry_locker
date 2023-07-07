namespace LockerService.Application.Accounts.Handlers;

public class AddStaffHandler : IRequestHandler<AddStaffRequest, AccountResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AddStaffHandler> _logger;
    private readonly IMapper _mapper;

    public AddStaffHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<AddStaffHandler> logger,
        IJwtService jwtService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<AccountResponse> Handle(AddStaffRequest request, CancellationToken cancellationToken)
    {
        var accountQuery =
            await _unitOfWork.AccountRepository.GetAsync(a =>
                a.PhoneNumber != null && Equals(a.PhoneNumber, request.PhoneNumber));
        if (accountQuery.FirstOrDefault() != null)
        {
            throw new ApiException(ResponseCode.AccountErrorExistedStaff);
        }

        var account = new Account()
        {
            Username = request.PhoneNumber,
            PhoneNumber = request.PhoneNumber,
            Password = "123456",
            Status = AccountStatus.Active,
            Role = Role.Staff,
        };

        if (request.StoreId != null)
        {
            var storeQuery =
                await _unitOfWork.StoreRepository.GetAsync(s =>
                    Equals(s.Id, request.StoreId));
            var store = storeQuery.FirstOrDefault();
            if (store == null)
            {
                throw new ApiException(ResponseCode.StoreErrorNotFound);
            }

            account.Store = store;
        }

        await _unitOfWork.AccountRepository.AddAsync(account);

        // Save changes
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<AccountResponse>(account);
    }
}
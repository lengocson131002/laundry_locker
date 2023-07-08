namespace LockerService.Application.Staffs.Handlers;

public class AddStaffHandler : IRequestHandler<AddStaffCommand, AccountResponse>
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

    public async Task<AccountResponse> Handle(AddStaffCommand request, CancellationToken cancellationToken)
    {
        var storeQuery =
            await _unitOfWork.StoreRepository.GetAsync(s =>
                Equals(s.Id, request.StoreId));
        var store = storeQuery.FirstOrDefault();
        if (store == null)
        {
            throw new ApiException(ResponseCode.StoreErrorNotFound);
        }

        var accountQuery =
            await _unitOfWork.AccountRepository.GetAsync(a =>
                a.PhoneNumber != null && Equals(a.PhoneNumber, request.PhoneNumber));

        var account = accountQuery.FirstOrDefault();
        if (account is null)
        {
            account = new Account()
            {
                Username = request.PhoneNumber,
                PhoneNumber = request.PhoneNumber,
                Password = "123456",
                Status = AccountStatus.Active,
                Role = Role.Staff,
                Store = store
            };
            await _unitOfWork.AccountRepository.AddAsync(account);
        }
        else
        {
            if (account.Store is not null)
            {
                throw new ApiException(ResponseCode.StaffErrorBelongToAStore);
            }

            account.Store = store;
            await _unitOfWork.AccountRepository.UpdateAsync(account);
        }

        // Save changes
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<AccountResponse>(account);
    }
}
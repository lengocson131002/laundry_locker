namespace LockerService.Application.Staffs.Handlers;

public class ActivateStaffHandler : IRequestHandler<ActivateStaffCommand, StatusResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<ActivateStaffHandler> _logger;
    private readonly IMapper _mapper;

    public ActivateStaffHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<ActivateStaffHandler> logger,
        IJwtService jwtService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<StatusResponse> Handle(ActivateStaffCommand request, CancellationToken cancellationToken)
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
                Equals(a.Id, request.Id));

        var account = accountQuery.FirstOrDefault();

        if (account is null)
        {
            throw new ApiException(ResponseCode.StaffErrorNotFound);
        }

        if (account.Status != AccountStatus.Inactive)
        {
            throw new ApiException(ResponseCode.StaffErrorInvalidStatus);
        }

        account.Status = AccountStatus.Active;

        await _unitOfWork.AccountRepository.UpdateAsync(account);

        // Save changes
        await _unitOfWork.SaveChangesAsync();
        return new StatusResponse(true);
    }
}
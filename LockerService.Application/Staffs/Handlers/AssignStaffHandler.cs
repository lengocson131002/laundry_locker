namespace LockerService.Application.Staffs.Handlers;

public class AssignStaffHandler : IRequestHandler<AssignStaffCommand, StatusResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AssignStaffHandler> _logger;
    private readonly IMapper _mapper;

    public AssignStaffHandler(
        IMapper mapper, 
        IUnitOfWork unitOfWork, 
        ILogger<AssignStaffHandler> logger,
        IJwtService jwtService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _jwtService = jwtService;
    }

    public async Task<StatusResponse> Handle(AssignStaffCommand request, CancellationToken cancellationToken)
    {
        var storeQuery =
            await _unitOfWork.StoreRepository.GetAsync(s =>
                Equals(s.Id, request.StoreId));
        var store = storeQuery.FirstOrDefault();
        if (store == null)
        {
            throw new ApiException(ResponseCode.StoreErrorNotFound);
        }

        var lockerQuery =
            await _unitOfWork.LockerRepository.GetAsync(locker =>
                locker.StoreId != null && Equals(request.StoreId, locker.StoreId));
        var locker = lockerQuery.FirstOrDefault();
        if (locker is null)
        {
            throw new ApiException(ResponseCode.LockerErrorNotFound);
        }

        var accountQuery =
            await _unitOfWork.AccountRepository.GetAsync(a =>
                Equals(a.Id, request.Id));

        var account = accountQuery.FirstOrDefault();

        if (account is null)
        {
            throw new ApiException(ResponseCode.StaffErrorNotFound);
        }


        var accountLockerQuery =
            await _unitOfWork.StaffLockerRepository.GetAsync(al =>
                Equals(al.StaffId, request.Id) && Equals(al.LockerId, request.LockerId));

        var accountLocker = accountLockerQuery.FirstOrDefault();

        if (accountLocker is not null)
        {
            throw new ApiException(ResponseCode.StaffErrorAssignedBefore);
        }

        accountLocker = new StaffLocker
        {
            Staff = account,
            Locker = locker
        };

        await _unitOfWork.StaffLockerRepository.AddAsync(accountLocker);

        // Save changes
        await _unitOfWork.SaveChangesAsync();
        return new StatusResponse(true);
    }
}
namespace LockerService.Application.Lockers.Handlers;

public class RevokeStaffHandler : IRequestHandler<RevokeStaffCommand, StatusResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<RevokeStaffHandler> _logger;
    private readonly IMapper _mapper;

    public RevokeStaffHandler(
        IMapper mapper,
        IUnitOfWork unitOfWork,
        ILogger<RevokeStaffHandler> logger,
        IJwtService jwtService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _jwtService = jwtService;
    }

    public async Task<StatusResponse> Handle(RevokeStaffCommand request, CancellationToken cancellationToken)
    {
        // Check locker
        var lockerQuery = await _unitOfWork.LockerRepository.GetAsync(
            predicate: locker => locker.Id == request.LockerId);

        var locker = lockerQuery.FirstOrDefault();
        if (locker is null)
        {
            throw new ApiException(ResponseCode.LockerErrorNotFound);
        }

        // Check staff
        var staffQuery = await _unitOfWork.AccountRepository.GetAsync(
            predicate: staff => staff.Id == request.StaffId);

        var staff = staffQuery.FirstOrDefault();
        if (staff is null)
        {
            throw new ApiException(ResponseCode.StaffErrorNotFound);
        }

        var slQuery =
            await _unitOfWork.StaffLockerRepository.GetAsync(
                al => Equals(al.StaffId, request.StaffId)
                      && Equals(al.LockerId, request.LockerId));
        
        var staffLocker = slQuery.FirstOrDefault();
        if (staffLocker is null)
        {
            throw new ApiException(ResponseCode.StaffLockerErrorNotFound);
        }

        await _unitOfWork.StaffLockerRepository.DeleteAsync(staffLocker);

        // Save changes
        await _unitOfWork.SaveChangesAsync();
        return new StatusResponse(true);
    }
}
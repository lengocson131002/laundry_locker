namespace LockerService.Application.Lockers.Handlers;

public class AssignStaffHandler : IRequestHandler<AssignStaffCommand, StatusResponse>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public AssignStaffHandler(
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<StatusResponse> Handle(AssignStaffCommand request,
        CancellationToken cancellationToken)
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

        // Check duplicate
        var slQuery =
            await _unitOfWork.StaffLockerRepository.GetAsync(
                al => Equals(al.StaffId, request.StaffId)
                      && Equals(al.LockerId, request.LockerId));

        if (slQuery.FirstOrDefault() is not null)
        {
            throw new ApiException(ResponseCode.StaffLockerErrorExisted);
        }

        // Check store
        if (!Equals(locker.Store, staff.Store))
        {
            throw new ApiException(ResponseCode.StoreErrorStaffAndLockerNotInSameStore);
        }

        var staffLocker = new StaffLocker
        {
            Staff = staff,
            Locker = locker,
        };

        await _unitOfWork.StaffLockerRepository.AddAsync(staffLocker);

        await _unitOfWork.SaveChangesAsync();
        return new StatusResponse(true);
    }
}
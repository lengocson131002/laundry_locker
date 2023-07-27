namespace LockerService.Application.Staffs.Handlers;

public class RevokeLockerHandler : IRequestHandler<RevokeLockerCommand, StatusResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public RevokeLockerHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<StatusResponse> Handle(RevokeLockerCommand request, CancellationToken cancellationToken)
    {
        // check staff
        var staff = await _unitOfWork.AccountRepository.GetStaffById(request.StaffId);
        if (staff == null)
        {
            throw new ApiException(ResponseCode.StaffErrorNotFound);
        }
        
        // check lockers
        var assignments = new List<StaffLocker>();
        foreach (var lockerId in request.LockerIds)
        {
            var locker = await _unitOfWork.LockerRepository.GetByIdAsync(lockerId);
            if (locker == null)
            {
                throw new ApiException(ResponseCode.LockerErrorNotFound);
            }

            var assigned = await _unitOfWork.StaffLockerRepository
                .Get(item => item.StaffId == request.StaffId && item.LockerId == lockerId)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (assigned == null)
            {
                throw new ApiException(ResponseCode.StaffLockerErrorNotFound);
            }
            
            assignments.Add(assigned);
        }

        await _unitOfWork.StaffLockerRepository.DeleteRange(assignments);
        await _unitOfWork.SaveChangesAsync();

        return new StatusResponse(true);
    }
}
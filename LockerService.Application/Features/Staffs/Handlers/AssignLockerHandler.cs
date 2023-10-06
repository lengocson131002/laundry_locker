using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Staffs.Commands;

namespace LockerService.Application.Features.Staffs.Handlers;

public class AssignLockerHandler : IRequestHandler<AssignLockerCommand, StatusResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public AssignLockerHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<StatusResponse> Handle(AssignLockerCommand request, CancellationToken cancellationToken)
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

            if (!Equals(staff.StoreId, locker.StoreId))
            {
                throw new ApiException(ResponseCode.StoreErrorStaffAndLockerNotInSameStore);
            }

            var assigned = await _unitOfWork.StaffLockerRepository
                .Get(item => item.StaffId == request.StaffId && item.LockerId == lockerId)
                .AnyAsync(cancellationToken);
            
            if (assigned)
            {
                throw new ApiException(ResponseCode.StaffLockerErrorExisted);
            }
            
            assignments.Add(new StaffLocker()
            {
                Staff = staff,
                Locker = locker
            });
        }

        await _unitOfWork.StaffLockerRepository.AddRange(assignments);
        await _unitOfWork.SaveChangesAsync();

        return new StatusResponse(true);
    }
}
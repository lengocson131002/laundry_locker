using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Lockers.Commands;

namespace LockerService.Application.Features.Lockers.Handlers;

public class RevokeStaffHandler : IRequestHandler<RevokeStaffCommand, StatusResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RevokeStaffHandler> _logger;

    public RevokeStaffHandler(
        IUnitOfWork unitOfWork,
        ILogger<RevokeStaffHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
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

        // check staffs
        var assignments = new List<StaffLocker>();
        foreach (var staffId in request.StaffIds)
        {
            var staff = await _unitOfWork.AccountRepository.GetStaffById(staffId);
            if (staff is null)
            {
                throw new ApiException(ResponseCode.StaffErrorNotFound);
            }

            var existed = await _unitOfWork.StaffLockerRepository
                .Get(item => item.LockerId == request.LockerId && item.StaffId == staffId)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (existed == null)
            {
                throw new ApiException(ResponseCode.StaffLockerErrorNotFound);
            }
            
            assignments.Add(existed);
        }
        
        await _unitOfWork.StaffLockerRepository.DeleteRange(assignments);
        await _unitOfWork.SaveChangesAsync();
        return new StatusResponse(true);
    }
}
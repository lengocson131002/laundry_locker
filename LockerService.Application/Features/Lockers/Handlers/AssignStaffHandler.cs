using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Lockers.Commands;

namespace LockerService.Application.Features.Lockers.Handlers;

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
        var assignments = new List<StaffLocker>();
        foreach (var staffId in request.StaffIds)
        {
            var staff = await _unitOfWork.AccountRepository.GetStaffById(staffId);
            if (staff is null)
            {
                throw new ApiException(ResponseCode.StaffErrorNotFound);
            }
        
            // Check duplicate
            var assigned = await _unitOfWork.StaffLockerRepository
                .Get(item => item.LockerId == request.LockerId && item.StaffId == staffId)
                .AnyAsync(cancellationToken);
            
            if (assigned)
            {
                throw new ApiException(ResponseCode.StaffLockerErrorExisted);
            }
        
            // Check store
            if (!Equals(locker.StoreId, staff.StoreId))
            {
                throw new ApiException(ResponseCode.StoreErrorStaffAndLockerNotInSameStore);
            }
        
            var staffLocker = new StaffLocker
            {
                Staff = staff,
                Locker = locker,
            };
        
            assignments.Add(staffLocker);
        }
        
        await _unitOfWork.StaffLockerRepository.AddRange(assignments);
        await _unitOfWork.SaveChangesAsync();
        return new StatusResponse(true);
    }
}
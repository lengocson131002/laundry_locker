namespace LockerService.Application.Staffs.Handlers;

public class UpdateStaffHandler : IRequestHandler<UpdateStaffCommand, StaffDetailResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateStaffHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<StaffDetailResponse> Handle(UpdateStaffCommand request, CancellationToken cancellationToken)
    {
        var staffQuery =
            await _unitOfWork.AccountRepository.GetAsync(a =>
                    Equals(a.Id, request.Id),
                includes: new List<Expression<Func<Account, object>>>
                {
                    staff => staff.Store
                });

        var staff = staffQuery.FirstOrDefault();
        if (staff is null)
        {
            throw new ApiException(ResponseCode.StaffErrorNotFound);
        }

        // Check store
        if (request.StoreId is not null && !Equals(request.StoreId, staff.StoreId))
        {
            
            var staffLockers = await _unitOfWork.StaffLockerRepository
                .Get(sl => Equals(sl.StaffId, staff.Id))
                .ToListAsync(cancellationToken);
            
            // Remove previous assignments
            if (staffLockers.Any())
            {
                await _unitOfWork.StaffLockerRepository.DeleteRange(staffLockers);
            }

            var storeQuery =
                await _unitOfWork.StoreRepository.GetAsync(s =>
                    Equals(s.Id, request.StoreId));
            var store = storeQuery.FirstOrDefault();
            if (store is null)
            {
                throw new ApiException(ResponseCode.StoreErrorNotFound);
            }

            staff.Store = store;
        }

        // Check username
        if (request.Username != null && !Equals(request.Username, staff.Username))
        {
            var checkStaff = await _unitOfWork.AccountRepository
                .GetByUsername(request.Username)
                .FirstOrDefaultAsync(cancellationToken);

            if (checkStaff != null)
            {
                throw new ApiException(ResponseCode.AccountErrorUsernameExisted);
            }
            
            staff.Username = request.Username;
        }
        
        staff.Avatar = request.Avatar ?? staff.Avatar;
        staff.FullName = request.FullName ?? staff.FullName;
        staff.Description = request.Description ?? staff.Description;
        staff.PhoneNumber = request.PhoneNumber ?? staff.PhoneNumber;
        
        await _unitOfWork.AccountRepository.UpdateAsync(staff);

        // Save changes
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<StaffDetailResponse>(staff);
    }
}
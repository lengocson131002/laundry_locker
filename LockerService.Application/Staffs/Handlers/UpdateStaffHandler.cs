namespace LockerService.Application.Staffs.Handlers;

public class UpdateStaffHandler : IRequestHandler<UpdateStaffCommand, StaffDetailResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<UpdateStaffHandler> _logger;
    private readonly IMapper _mapper;

    public UpdateStaffHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<UpdateStaffHandler> logger,
        IJwtService jwtService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _jwtService = jwtService;
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
        if (request.StoreId is not null)
        {
            var slQuery =
                await _unitOfWork.StaffLockerRepository.GetAsync(sl =>
                    Equals(sl.StaffId, staff.Id));
            if (slQuery.FirstOrDefault() is not null)
            {
                throw new ApiException(ResponseCode.StaffErrorInAssignment);
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

        // Check phone number
        if (request.PhoneNumber is not null && !Equals(request.PhoneNumber, staff.PhoneNumber))
        {
            var checkStaff = await _unitOfWork.AccountRepository.GetStaffByPhoneNumber(request.PhoneNumber);

            if (checkStaff is not null)
            {
                throw new ApiException(ResponseCode.StaffErrorExisted);
            }

            staff.PhoneNumber = request.PhoneNumber;
            staff.Username = request.PhoneNumber;
        }

        staff.Avatar = request.Avatar ?? staff.Avatar;
        staff.FullName = request.FullName ?? staff.FullName;
        staff.Description = request.Description ?? staff.Description;

        await _unitOfWork.AccountRepository.UpdateAsync(staff);

        // Save changes
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<StaffDetailResponse>(staff);
    }
}
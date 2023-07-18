using LockerService.Application.Staffs.Models;

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

        if (request.StoreId is not null)
        {
            var storeQuery =
                await _unitOfWork.StoreRepository.GetAsync(s =>
                    Equals(s.Id, request.StoreId));
            var store = storeQuery.FirstOrDefault();
            if (store == null)
            {
                throw new ApiException(ResponseCode.StoreErrorNotFound);
            }

            staff.Store = store;
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
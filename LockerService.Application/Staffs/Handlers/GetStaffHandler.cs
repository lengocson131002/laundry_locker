using LockerService.Application.Staffs.Models;

namespace LockerService.Application.Staffs.Handlers;

public class GetStaffHandler : IRequestHandler<GetStaffQuery, StaffDetailResponse>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetStaffHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<StaffDetailResponse> Handle(GetStaffQuery request,
        CancellationToken cancellationToken)
    {
        var staffQuery = await _unitOfWork.AccountRepository.GetAsync(
            staff => staff.Id == request.Id
                     && Equals(staff.Role, Role.Staff),
            includes: new List<Expression<Func<Account, object>>>()
            {
                staff => staff.Store,
                staff => staff.Store.Location,
                staff => staff.Store.Location.Province,
                staff => staff.Store.Location.District,
                staff => staff.Store.Location.Ward,
            });

        var staff = staffQuery.FirstOrDefault();
        if (staff is null)
        {
            throw new ApiException(ResponseCode.StaffErrorNotFound);
        }

        return _mapper.Map<StaffDetailResponse>(staff);
    }
}
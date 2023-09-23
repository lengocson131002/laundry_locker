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
        var staff = await _unitOfWork.AccountRepository
            .Get(
                staff => staff.Id == request.Id && staff.IsStaff,
                includes: new List<Expression<Func<Account, object>>>
                {
                    staff => staff.Store,
                    staff => staff.Store.Location,
                    staff => staff.Store.Location.Province,
                    staff => staff.Store.Location.District,
                    staff => staff.Store.Location.Ward
                },
                disableTracking: true
            ).FirstOrDefaultAsync(cancellationToken);

        if (staff is null)
        {
            throw new ApiException(ResponseCode.StaffErrorNotFound);
        }

        return _mapper.Map<StaffDetailResponse>(staff);
    }
}
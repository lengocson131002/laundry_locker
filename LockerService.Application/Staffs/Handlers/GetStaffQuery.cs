namespace LockerService.Application.Staffs.Handlers;

public class GetStaffHandler : IRequestHandler<GetStaffQuery, AccountDetailResponse>
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

    public async Task<AccountDetailResponse> Handle(GetStaffQuery request,
        CancellationToken cancellationToken)
    {
        var staffQuery = await _unitOfWork.AccountRepository.GetAsync(
            staff => staff.Id == request.StoreId && staff.StoreId == request.StoreId
        );

        var staff = staffQuery.FirstOrDefault();
        if (staff is null)
        {
            throw new ApiException(ResponseCode.StoreErrorNotFound);
        }

        return _mapper.Map<AccountDetailResponse>(staff);
    }
}
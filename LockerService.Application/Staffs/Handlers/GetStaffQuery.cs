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
        var storeQuery =
            await _unitOfWork.StoreRepository.GetAsync(s =>
                Equals(s.Id, request.StoreId));
        var store = storeQuery.FirstOrDefault();
        if (store == null)
        {
            throw new ApiException(ResponseCode.StoreErrorNotFound);
        }

        var staffQuery = await _unitOfWork.AccountRepository.GetAsync(
            staff => staff.Id == request.Id && staff.StoreId == request.StoreId
        );

        var staff = staffQuery.FirstOrDefault();
        if (staff is null)
        {
            throw new ApiException(ResponseCode.StaffErrorNotFound);
        }

        return _mapper.Map<StaffDetailResponse>(staff);
    }
}
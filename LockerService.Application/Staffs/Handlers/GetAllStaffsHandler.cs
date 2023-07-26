using LockerService.Application.Staffs.Models;

namespace LockerService.Application.Staffs.Handlers;

public class GetAllStaffsHandler : IRequestHandler<GetAllStaffsQuery, PaginationResponse<Account, StaffResponse>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetAllStaffsHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginationResponse<Account, StaffResponse>> Handle(GetAllStaffsQuery request,
        CancellationToken cancellationToken)
    {
        var staffs = await _unitOfWork.AccountRepository.GetAsync(
            predicate: request.GetExpressions(),
            includes: new List<Expression<Func<Account, object>>>()
            {
                staff => staff.Store
            },
            orderBy: request.GetOrder()
        );

        return new PaginationResponse<Account, StaffResponse>(
            staffs,
            request.PageNumber,
            request.PageSize,
            entity => _mapper.Map<StaffResponse>(entity));
    }
}
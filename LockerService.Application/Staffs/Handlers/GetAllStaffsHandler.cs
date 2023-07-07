namespace LockerService.Application.Staffs.Handlers;

public class GetAllStaffsHandler : IRequestHandler<GetAllStaffsQuery, PaginationResponse<Account, AccountResponse>>
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

    public async Task<PaginationResponse<Account, AccountResponse>> Handle(GetAllStaffsQuery request,
        CancellationToken cancellationToken)
    {
        var staffs = await _unitOfWork.AccountRepository.GetAsync(
            predicate: request.GetExpressions(),
            orderBy: request.GetOrder()
        );

        return new PaginationResponse<Account, AccountResponse>(
            staffs,
            request.PageNumber,
            request.PageSize,
            entity => _mapper.Map<AccountResponse>(entity));
    }
}
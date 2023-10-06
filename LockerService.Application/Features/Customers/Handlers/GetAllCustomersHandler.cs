using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Customers.Models;
using LockerService.Application.Features.Customers.Queries;

namespace LockerService.Application.Features.Customers.Handlers;

public class GetAllCustomersHandler : IRequestHandler<GetAllCustomersQuery, PaginationResponse<Account, CustomerResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllCustomersHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginationResponse<Account, CustomerResponse>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        var query = await _unitOfWork.AccountRepository.GetAsync(
            predicate: request.GetExpressions(),
            orderBy: request.GetOrder(),
            disableTracking: true
        );

        return new PaginationResponse<Account, CustomerResponse>(
            query,
            request.PageNumber,
            request.PageSize,
            cus => _mapper.Map<CustomerResponse>(cus));
    }

}
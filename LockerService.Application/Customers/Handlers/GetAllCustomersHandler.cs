using System.Linq.Dynamic.Core;
using LockerService.Application.Customers.Models;
using LockerService.Application.Customers.Queries;

namespace LockerService.Application.Customers.Handlers;

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
            disableTracking: true
        );

        // Count total orders
        var countQuery = query.Select(acc => new
        {   
            Account = acc,
            OrderCount = acc.ReceiveOrders.Count + acc.SendOrders.Count
        });

        if (!string.IsNullOrWhiteSpace(request.SortColumn))
        {
            countQuery = countQuery.OrderBy($"{request.SortColumn} {request.SortDir.ToString().ToLower()}");
        }

        var count = countQuery.Count();
        var customers = countQuery
            .AsEnumerable()
            .Select(item =>
            {
                var customResponse = _mapper.Map<CustomerResponse>(item.Account);
                return customResponse;
            })
            .ToList();
        
        return new PaginationResponse<Account, CustomerResponse>(customers, count, request.PageNumber, request.PageSize);
    }

}
using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Customers.Models;
using LockerService.Application.Features.Customers.Queries;

namespace LockerService.Application.Features.Customers.Handlers;

public class GetCustomerDetailHandler : IRequestHandler<GetCustomerDetailQuery, CustomerDetailResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCustomerDetailHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CustomerDetailResponse> Handle(GetCustomerDetailQuery request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.AccountRepository
            .Get(cus => cus.Id == request.Id && Equals(cus.Role, Role.Customer))
            .Include(cus => cus.Wallet)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (customer == null)
        {
            throw new ApiException(ResponseCode.AuthErrorAccountNotFound);
        }

        return _mapper.Map<CustomerDetailResponse>(customer);
    }
}
using LockerService.Application.Customers.Models;
using LockerService.Application.Customers.Queries;

namespace LockerService.Application.Customers.Handlers;

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
            .FirstOrDefaultAsync(cancellationToken);

        if (customer == null)
        {
            throw new ApiException(ResponseCode.AuthErrorAccountNotFound);
        }

        return _mapper.Map<CustomerDetailResponse>(customer);
    }
}
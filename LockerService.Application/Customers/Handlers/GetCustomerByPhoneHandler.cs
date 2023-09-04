using LockerService.Application.Customers.Models;
using LockerService.Application.Customers.Queries;

namespace LockerService.Application.Customers.Handlers;

public class GetCustomerByPhoneHandler : IRequestHandler<GetCustomerByPhoneQuery, CustomerDetailResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCustomerByPhoneHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CustomerDetailResponse> Handle(GetCustomerByPhoneQuery request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.AccountRepository.GetCustomerByPhoneNumber(request.Phone.NormalizePhoneNumber());
        if (customer == null)
        {
            throw new ApiException(ResponseCode.AuthErrorAccountNotFound);
        }

        return _mapper.Map<CustomerDetailResponse>(customer);
    }
}
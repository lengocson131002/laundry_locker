using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Payments.Models;
using LockerService.Application.Features.Payments.Queries;
using LockerService.Domain;

namespace LockerService.Application.Features.Payments.Handlers;

public class GetAllPaymentHandler : IRequestHandler<GetAllPaymentQuery, PaginationResponse<Payment, PaymentResponse>>
{
    private readonly ICurrentAccountService _currentAccountService;

    private readonly IMapper _mapper;
    
    private readonly IUnitOfWork _unitOfWork;

    public GetAllPaymentHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentAccountService currentAccountService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentAccountService = currentAccountService;
    }

    public async Task<PaginationResponse<Payment, PaymentResponse>> Handle(GetAllPaymentQuery request,
        CancellationToken cancellationToken)
    {
        var currentAccount = await _currentAccountService.GetCurrentAccount();
        if (currentAccount != null && currentAccount.IsCustomer)
        {
            request.CustomerId = currentAccount.Id;
        }
        
        var orders = _unitOfWork.PaymentRepository
            .Get(
                request.GetExpressions(),
                request.GetOrder(),
                disableTracking: true);

        return new PaginationResponse<Payment, PaymentResponse>(
            orders,
            request.PageNumber,
            request.PageSize,
            payment => _mapper.Map<PaymentResponse>(payment));
    }
}
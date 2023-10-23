using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Payments.Models;
using LockerService.Application.Features.Payments.Queries;

namespace LockerService.Application.Features.Payments.Handlers;

public class GetPaymentHandler : IRequestHandler<GetPaymentQuery, PaymentDetailResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    public GetPaymentHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaymentDetailResponse> Handle(GetPaymentQuery request, CancellationToken cancellationToken)
    {
        var payment = await _unitOfWork.PaymentRepository
                .Get(payment => payment.Id == request.Id)
                .Include(payment => payment.Customer)
                .FirstOrDefaultAsync(cancellationToken);

        if (payment == null)
        {
            throw new ApiException(ResponseCode.PaymentErrorNotFound);
        }

        return _mapper.Map<PaymentDetailResponse>(payment);
    }
}
using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Payments.Models;
using LockerService.Application.Features.Payments.Queries;

namespace LockerService.Application.Features.Payments.Handlers;

public class GetPaymentHandler : IRequestHandler<GetPaymentQuery, PaymentResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    public GetPaymentHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaymentResponse> Handle(GetPaymentQuery request, CancellationToken cancellationToken)
    {
        var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(request.Id);

        if (payment == null)
        {
            throw new ApiException(ResponseCode.PaymentErrorNotFound);
        }

        return _mapper.Map<PaymentResponse>(payment);
    }
}
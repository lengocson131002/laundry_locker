using LockerService.Application.Bills.Models;
using LockerService.Application.Bills.Queries;

namespace LockerService.Application.Bills.Handlers;

public class GetBillHandler : IRequestHandler<BillQuery, BillResponse>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetBillHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<BillResponse> Handle(BillQuery request, CancellationToken cancellationToken)
    {
        var orderQuery = await _unitOfWork.OrderRepository.GetAsync(order => order.Id == request.OrderId);
        var order = await orderQuery
            .Include(order => order.Bill)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (order == null)
        {
            throw new ApiException(ResponseCode.BillErrorNotFound);
        }

        return _mapper.Map<BillResponse>(order.Bill);
    }
}
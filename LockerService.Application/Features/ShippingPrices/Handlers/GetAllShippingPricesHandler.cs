using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.ShippingPrices.Models;
using LockerService.Application.Features.ShippingPrices.Queries;

namespace LockerService.Application.Features.ShippingPrices.Handlers;

public class GetAllShippingPricesHandler : IRequestHandler<GetAllShippingPricesQuery, ListResponse<ShippingPriceResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    
    private readonly IMapper _mapper;

    public GetAllShippingPricesHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ListResponse<ShippingPriceResponse>> Handle(GetAllShippingPricesQuery request, CancellationToken cancellationToken)
    {
        var shippingPrices = await _unitOfWork.ShippingPriceRepository
            .Get()
            .OrderBy(price => price.FromDistance)
            .ToListAsync(cancellationToken);
        
        var response = _mapper.Map<List<ShippingPriceResponse>>(shippingPrices);
        
        return new ListResponse<ShippingPriceResponse>(response);
    }
}
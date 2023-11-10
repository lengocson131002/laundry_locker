using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.ShippingPrices.Commands;
using LockerService.Application.Features.ShippingPrices.Models;

namespace LockerService.Application.Features.ShippingPrices.Handlers;

public class AddShippingPriceHandler : IRequestHandler<AddShippingPriceCommand, ShippingPriceResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    public AddShippingPriceHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ShippingPriceResponse> Handle(AddShippingPriceCommand request, CancellationToken cancellationToken)
    {
        // validate existed shipping price with request's from length
        var existedShippingPrice = await _unitOfWork.ShippingPriceRepository.GetByFromDistance(request.FromDistance);
        if (existedShippingPrice != null)
        {
            throw new ApiException(ResponseCode.ShippingPriceErrorExisted);
        }

        var shippingPrice = new ShippingPrice(request.FromDistance, request.Price);
        var savedPrice = await _unitOfWork.ShippingPriceRepository.AddAsync(shippingPrice);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ShippingPriceResponse>(savedPrice);
    }
}
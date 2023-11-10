using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.ShippingPrices.Commands;
using LockerService.Application.Features.ShippingPrices.Models;

namespace LockerService.Application.Features.ShippingPrices.Handlers;

public class RemoveShippingPriceHandler : IRequestHandler<RemoveShippingPriceCommand, ShippingPriceResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    public RemoveShippingPriceHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ShippingPriceResponse> Handle(RemoveShippingPriceCommand request, CancellationToken cancellationToken)
    {
        var shippingPrice = await _unitOfWork.ShippingPriceRepository.GetByIdAsync(request.ShippingPriceId);
        if (shippingPrice == null)
        {
            throw new ApiException(ResponseCode.ShippingPriceErrorNotFound);
        }

        await _unitOfWork.ShippingPriceRepository.DeleteAsync(shippingPrice);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ShippingPriceResponse>(shippingPrice);
    }
}
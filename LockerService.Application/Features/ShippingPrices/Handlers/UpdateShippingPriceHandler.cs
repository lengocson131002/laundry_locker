using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.ShippingPrices.Commands;
using LockerService.Application.Features.ShippingPrices.Models;

namespace LockerService.Application.Features.ShippingPrices.Handlers;

public class UpdateShippingPriceHandler: IRequestHandler<UpdateShippingPriceCommand, ShippingPriceResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;
    
    public UpdateShippingPriceHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ShippingPriceResponse> Handle(UpdateShippingPriceCommand request, CancellationToken cancellationToken)
    {
        var shippingPrice = await _unitOfWork.ShippingPriceRepository.GetByIdAsync(request.ShippingPriceId);
        if (shippingPrice == null)
        {
            throw new ApiException(ResponseCode.ShippingPriceErrorNotFound);
        }

        var requestFromLength = request.FromDistance;
        if (requestFromLength != null && !Equals(shippingPrice.FromDistance, requestFromLength))
        {
            // check existed
            var existedShippingPrice = await _unitOfWork.ShippingPriceRepository.GetByFromDistance(requestFromLength.Value);
            if (existedShippingPrice != null)
            {
                throw new ApiException(ResponseCode.ShippingPriceErrorExisted);
            }

            shippingPrice.FromDistance = requestFromLength.Value;
        }

        if (request.Price != null)
        {
            shippingPrice.Price = request.Price.Value;
        }

        await _unitOfWork.ShippingPriceRepository.UpdateAsync(shippingPrice);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ShippingPriceResponse>(shippingPrice);
    }
}
using LockerService.Application.Common.Enums;
using LockerService.Application.Common.Exceptions;
using LockerService.Application.Common.Services;
using LockerService.Domain.Entities;
using LockerService.Domain.Enums;

namespace LockerService.Infrastructure.Services;

public class FeeService : IFeeService
{
    public double CalculateFree(in Order order)
    {
        var service = order.Service;
        switch (service?.FeeType)
        {
            case FeeType.ByTime:
            {
                var  timespan = DateTimeOffset.Now - order.CreatedAt;
                var orderTimeInHours = Round(timespan.TotalHours);
                if (service.Fee == null)
                {
                    throw new ApiException(ResponseCode.OrderErrorServiceFeeIsMissing);
                }

                order.Amount = orderTimeInHours;
                return Round(orderTimeInHours * (double)service.Fee);
            }
            
            case FeeType.ByUnitPrice:
                if (order.Amount == null || order.Amount <= 0)
                {
                    throw new ApiException(ResponseCode.OrderErrorAmountIsRequired);
                }
                
                if (service.Fee == null)
                {
                    throw new ApiException(ResponseCode.OrderErrorServiceFeeIsMissing);
                }
                
                return Round((double) order.Amount * (double) service.Fee);
            
            case FeeType.ByInputPrice:
                if (order.Fee == null || order.Fee <= 0)
                {
                    throw new ApiException(ResponseCode.OrderErrorFeeIsRequired);
                }

                return Round((double) order.Fee);
            
            default:
                throw new ApiException(ResponseCode.OrderErrorServiceFeeTypeIsMissing);
        }
    }

    private double Round(double fee)
    {
        return Math.Round(fee, 2, MidpointRounding.AwayFromZero);
    }
}
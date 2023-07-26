using LockerService.Application.Common.Enums;
using LockerService.Application.Common.Exceptions;
using LockerService.Application.Common.Persistence;
using LockerService.Application.Common.Services;
using LockerService.Domain.Entities;
using LockerService.Domain.Enums;

namespace LockerService.Infrastructure.Services;

public class FeeService : IFeeService
{
    private readonly IUnitOfWork _unitOfWork;

    public FeeService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task CalculateFree(Order order)
    {
        switch (order.Type)
        {
            case OrderType.Storage:
                await CalculateStoreFee(order);
                break;
            case OrderType.Laundry:
                await CalculateLaundryFree(order);
                break;
        }
    }

    private Task CalculateStoreFee(Order order)
    {
        order.Price = 100000;
        return Task.CompletedTask;
    }

    private Task CalculateLaundryFree(Order order)
    {
        if (!order.UpdatedInfo)
        {
            throw new ApiException(ResponseCode.OrderDetailErrorInfoRequired);
        }

        var servicePrice = order.Details.Sum(item => item.Price * (decimal) item.Quantity!);
        order.Price = Round(servicePrice);
        return Task.CompletedTask;
    }

    private decimal Round(decimal fee)
    {
        return Math.Round(fee, 2, MidpointRounding.AwayFromZero);
    }
}
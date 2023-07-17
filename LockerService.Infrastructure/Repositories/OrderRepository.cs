using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Common.Utils;
using LockerService.Domain.Entities;
using LockerService.Domain.Enums;
using LockerService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LockerService.Infrastructure.Repositories;

public class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    private readonly ApplicationDbContext _dbContext;


    public OrderRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> GenerateOrderPinCode(int length = 6)
    {
        while (true)
        {
            var pinCode = GeneratorUtils.GenerateToken(length);

            var order = await _dbContext.Orders.FirstOrDefaultAsync(
                order => pinCode.Equals(order.PinCode)
                         && !OrderStatus.Completed.Equals(order.Status)
                         && !OrderStatus.Canceled.Equals(order.Status));

            if (order == null) return pinCode;
        }
    }

    public async Task<Order?> GetOrderByPinCode(string pinCode)
    {
        return await _dbContext.Orders.FirstOrDefaultAsync(
            order => pinCode.Equals(order.PinCode)
                     && !OrderStatus.Completed.Equals(order.Status)
                     && !OrderStatus.Canceled.Equals(order.Status));
    }
}
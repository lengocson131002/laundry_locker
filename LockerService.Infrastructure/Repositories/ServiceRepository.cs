using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Entities;
using LockerService.Domain.Enums;
using LockerService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LockerService.Infrastructure.Repositories;

public class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    private const string AllowedCharacters = "0123456789";
    
    public OrderRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> GenerateOrderPinCode(int length = 6)
    {
        while (true)
        {
            var pinCode = GeneratePinCode(length);
            var order = await GetOrderByPinCode(pinCode);
            if (order  == null) return pinCode;
        }
    }

    public async Task<Order?> GetOrderByPinCode(string pinCode)
    {
        return await _dbContext.Orders.FirstOrDefaultAsync(
            order => pinCode.Equals(order.PinCode)
                     && !OrderStatus.Completed.Equals(order.Status)
                     && !OrderStatus.Canceled.Equals(order.Status));
    }

    private string GeneratePinCode(int length)
    {
        var rand = new Random();
        
        var otp = string.Empty;

        for (var i = 0; i < length; i++)
        {
            otp += AllowedCharacters[rand.Next(0, AllowedCharacters.Length)];
        }

        return otp;
    }
}
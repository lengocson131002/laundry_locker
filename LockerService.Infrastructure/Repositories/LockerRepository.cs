using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Lockers.Models;
using LockerService.Domain.Entities;
using LockerService.Domain.Enums;
using LockerService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LockerService.Infrastructure.Repositories;

public class LockerRepository : BaseRepository<Locker>, ILockerRepository
{
    private readonly ApplicationDbContext _dbContext;

    public LockerRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int?> FindAvailableBox(int lockerId)
    {
        var availableBoxes = await FindAvailableBoxes(lockerId);
        return availableBoxes.Any() ? availableBoxes[0] : null;
    }

    public async Task<IList<int>> FindAvailableBoxes(int lockerId)
    {
        var boxes = await GetAllBoxes(lockerId);
        return boxes
            .Where(box => box.IsAvailable)
            .Select(box => box.BoxOrder)
            .ToList();
    }

    public async Task<IList<BoxStatus>> GetAllBoxes(int lockerId)
    {
        var boxCount = await _dbContext.Lockers
            .Where(locker => locker.Id == lockerId)
            .Select(locker => locker.ColumnCount * locker.RowCount)
            .FirstAsync();

        if (boxCount == 0) return new List<BoxStatus>();

        var latestOrders = await _dbContext.Orders
            .AsNoTracking()
            .Where(order => order.LockerId == lockerId)
            .GroupBy(order => order.ReceiveBoxOrder)
            .Select(group => group.OrderByDescending(order => order.CreatedAt).First())
            .ToListAsync();

        return Enumerable.Range(1, boxCount)
            .GroupJoin(
                latestOrders,
                boxOrder => boxOrder,
                order => order.ReceiveBoxOrder,
                (boxOrder, order) => new { boxOrder, order })
            .SelectMany(item => item.order.DefaultIfEmpty(), (boxOrder, order) => new BoxStatus
            {
                BoxOrder = boxOrder.boxOrder,
                IsAvailable = order == null || (!OrderStatus.Initialized.Equals(order.Status)
                              && !OrderStatus.Waiting.Equals(order.Status)
                              && !OrderStatus.Returned.Equals(order.Status)),
                OrderStatus = order?.Status,
                OrderId = order?.Id,
            })
            .ToList();
    }

    public async Task<Locker?> FindByMac(string mac)
    {
        return await _dbContext.Lockers
            .FirstOrDefaultAsync(lo => lo.MacAddress.ToLower().Equals(mac.ToLower()));
    }

    public async Task<Locker?> FindByName(string name)
    {
        return await _dbContext.Lockers
            .FirstOrDefaultAsync(lo => lo.Name.ToLower().Equals(name.ToLower()));
    }
}
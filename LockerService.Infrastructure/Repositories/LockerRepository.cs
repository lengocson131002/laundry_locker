using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Enums;
using LockerService.Infrastructure.Persistence;

namespace LockerService.Infrastructure.Repositories;

public class LockerRepository : BaseRepository<Locker>, ILockerRepository
{
    private readonly ApplicationDbContext _dbContext;

    public LockerRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Box?> FindAvailableBox(long lockerId)
    {
        var boxes = await FindAvailableBoxes(lockerId);
        return boxes.Any() ? boxes[0] : null;
    }

    public async Task<IList<Box>> FindAvailableBoxes(long lockerId)
    {
        return await GetAllBoxesQueryable(lockerId)
            .Where(box => box.IsActive && (box.LastOrder == null
                                           || (!OrderStatus.Initialized.Equals(box.LastOrder.Status)
                                               && !OrderStatus.Waiting.Equals(box.LastOrder.Status)
                                               && !OrderStatus.Returned.Equals(box.LastOrder.Status))))
            .ToListAsync();
    }

    public async Task<IList<Box>> GetAllBoxes(long lockerId)
    {
        return await GetAllBoxesQueryable(lockerId)
            .ToListAsync();
    }

    public async Task<Locker?> FindByName(string name)
    {
        return await _dbContext.Lockers
            .FirstOrDefaultAsync(lo => lo.Name.ToLower().Equals(name.ToLower()));
    }

    public async Task<Locker?> FindByCode(string code)
    {
        return await _dbContext.Lockers.FirstOrDefaultAsync(lo => Equals(lo.Code, code));
    }

    private IQueryable<Box> GetAllBoxesQueryable(long lockerId)
    {
        return _dbContext.Boxes
            .Where(box => box.LockerId == lockerId)
            .OrderBy(box => box.Number)
            .GroupJoin(
                _dbContext.Orders
                    .Include(order => order.Sender)
                    .Include(order => order.Receiver),
                box => box.Id,
                order => order.ReceiveBoxId,
                (box, orders) => new Box
                {
                    Id = box.Id,
                    Number = box.Number,
                    PinNo = box.PinNo,
                    IsActive = box.IsActive,
                    CreatedAt = box.CreatedAt,
                    UpdatedAt = box.UpdatedAt,
                    LockerId = box.LockerId,
                    LastOrder = orders.OrderByDescending(or => or.CreatedAt).First()
                });
    }
}
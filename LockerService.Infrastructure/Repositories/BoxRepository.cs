using LinqKit;
using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Enums;
using LockerService.Infrastructure.Persistence;

namespace LockerService.Infrastructure.Repositories;

public class BoxRepository : BaseRepository<Box>, IBoxRepository
{
    private readonly ApplicationDbContext _dbContext;

    public BoxRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Box?> FindBox(long lockerId, int number)
    {
        return await _dbContext.Boxes
            .FirstOrDefaultAsync(box => box.LockerId == lockerId && box.Number == number);
    }

    public async Task<Box?> FindAvailableBox(long lockerId)
    {
        var boxes = await FindAvailableBoxes(lockerId);
        return boxes.Any() ? boxes[0] : null;
    }

    public async Task<IList<Box>> FindAvailableBoxes(long lockerId)
    {
        return await GetAllBoxesQueryable(lockerId)
            .Where(box => box.IsAvailable)
            .ToListAsync();
    }

    public async Task<IList<Box>> GetAllBoxes(long lockerId)
    {
        return await GetAllBoxesQueryable(lockerId)
            .ToListAsync();
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
                order => order.ReceiveBoxId ?? order.SendBoxId,
                (box, orders) => new Box
                {
                    Id = box.Id,
                    Number = box.Number,
                    IsActive = box.IsActive,
                    CreatedAt = box.CreatedAt,
                    UpdatedAt = box.UpdatedAt,
                    LockerId = box.LockerId,
                    LastOrder = orders.OrderByDescending(or => or.CreatedAt).First()
                });
    }
    
}
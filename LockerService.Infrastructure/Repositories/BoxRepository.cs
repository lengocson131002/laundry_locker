using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Entities;
using LockerService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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
        return await _dbContext.Boxes.FirstOrDefaultAsync(box => box.LockerId == lockerId && box.Number == number);
    }
}
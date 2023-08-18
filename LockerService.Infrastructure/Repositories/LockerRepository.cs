using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Infrastructure.Persistence;

namespace LockerService.Infrastructure.Repositories;

public class LockerRepository : BaseRepository<Locker>, ILockerRepository
{
    private readonly ApplicationDbContext _dbContext;

    public LockerRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
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
}
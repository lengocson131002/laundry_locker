using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Infrastructure.Persistence;

namespace LockerService.Infrastructure.Repositories;

public class HardwareRepository : BaseRepository<Hardware>, IHardwareRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public HardwareRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
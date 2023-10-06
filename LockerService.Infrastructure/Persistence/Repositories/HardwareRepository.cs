using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Infrastructure.Persistence.Contexts;

namespace LockerService.Infrastructure.Persistence.Repositories;

public class HardwareRepository : BaseRepository<Hardware>, IHardwareRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public HardwareRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
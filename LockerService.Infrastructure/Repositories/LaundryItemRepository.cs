using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Infrastructure.Persistence;

namespace LockerService.Infrastructure.Repositories;

public class LaundryItemRepository : BaseRepository<LaundryItem>, ILaundryItemRepository
{
    private readonly ApplicationDbContext _dbContext; 
    
    public LaundryItemRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
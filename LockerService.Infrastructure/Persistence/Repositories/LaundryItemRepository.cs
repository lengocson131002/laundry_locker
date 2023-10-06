using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Infrastructure.Persistence.Contexts;

namespace LockerService.Infrastructure.Persistence.Repositories;

public class LaundryItemRepository : BaseRepository<LaundryItem>, ILaundryItemRepository
{
    private readonly ApplicationDbContext _dbContext; 
    
    public LaundryItemRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
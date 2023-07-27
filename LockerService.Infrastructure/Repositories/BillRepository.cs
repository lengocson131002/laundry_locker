using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Infrastructure.Persistence;

namespace LockerService.Infrastructure.Repositories;

public class BillRepository : BaseRepository<Bill>, IBillRepository
{
    private readonly ApplicationDbContext _dbContext;

    public BillRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
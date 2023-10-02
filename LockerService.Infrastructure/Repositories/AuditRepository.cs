using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Infrastructure.Persistence;

namespace LockerService.Infrastructure.Repositories;

public class AuditRepository : BaseRepository<Audit>, IAuditRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AuditRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
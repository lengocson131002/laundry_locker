using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Infrastructure.Persistence.Contexts;

namespace LockerService.Infrastructure.Persistence.Repositories;

public class AuditRepository : BaseRepository<Audit>, IAuditRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AuditRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
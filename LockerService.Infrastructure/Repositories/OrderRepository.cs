using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Entities;
using LockerService.Infrastructure.Persistence;

namespace LockerService.Infrastructure.Repositories;

public class ServiceRepository : BaseRepository<Service>, IServiceRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public ServiceRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
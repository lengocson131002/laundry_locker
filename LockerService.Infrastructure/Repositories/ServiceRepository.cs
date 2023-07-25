using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Entities;
using LockerService.Domain.Enums;
using LockerService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LockerService.Infrastructure.Repositories;

public class ServiceRepository : BaseRepository<Service>, IServiceRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public ServiceRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
    
}
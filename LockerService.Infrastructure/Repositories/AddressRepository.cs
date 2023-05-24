using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Entities;
using LockerService.Infrastructure.Persistence;

namespace LockerService.Infrastructure.Repositories;

public class AddressRepository : BaseRepository<Address>, IAddressRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public AddressRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
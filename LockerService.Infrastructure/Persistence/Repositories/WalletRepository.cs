using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Infrastructure.Persistence.Contexts;

namespace LockerService.Infrastructure.Persistence.Repositories;

public class WalletRepository: BaseRepository<Wallet>, IWalletRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public WalletRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
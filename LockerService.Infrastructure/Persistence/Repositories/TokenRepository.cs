using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Infrastructure.Persistence.Contexts;

namespace LockerService.Infrastructure.Persistence.Repositories;

public class TokenRepository : BaseRepository<Token>, ITokenRepository
{
    private readonly ApplicationDbContext _dbContext;

    public TokenRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
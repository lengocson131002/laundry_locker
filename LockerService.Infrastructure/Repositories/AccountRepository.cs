using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Entities;
using LockerService.Domain.Enums;
using LockerService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LockerService.Infrastructure.Repositories;

public class AccountRepository : BaseRepository<Account>, IAccountRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public AccountRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Account?> FindCustomer(string phoneNumber)
    {
        return await _dbContext.Accounts.FirstOrDefaultAsync(acc =>
            Equals(Role.Customer, acc.Role) && Equals(phoneNumber, acc.PhoneNumber));
    }
}
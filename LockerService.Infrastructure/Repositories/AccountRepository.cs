using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Enums;
using LockerService.Infrastructure.Persistence;

namespace LockerService.Infrastructure.Repositories;

public class AccountRepository : BaseRepository<Account>, IAccountRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AccountRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Account?> GetStaffById(long id)
    {
        var accountQuery = await GetAsync(account =>
            Equals(account.Id, id)
            && Equals(account.Role, Role.Staff));
        return accountQuery.FirstOrDefault();
    }

    public async Task<Account?> GetStaffByPhoneNumber(string phoneNumber)
    {
        var accountQuery = await GetAsync(account =>
            Equals(account.PhoneNumber, phoneNumber)
            && Equals(account.Role, Role.Staff));
        return accountQuery.FirstOrDefault();
    }

    public async Task<Account?> GetCustomerByUsername(string username)
    {
        var accountQuery = await GetAsync(account =>
            Equals(account.Username, username)
            && Equals(account.Role, Role.Customer));
        return accountQuery.FirstOrDefault();
    }

    public async Task<Account?> GetCustomerByPhoneNumber(string phoneNumber)
    {
        var accountQuery = await GetAsync(account =>
            Equals(account.PhoneNumber, phoneNumber)
            && Equals(account.Role, Role.Customer));
        return accountQuery.FirstOrDefault();
    }

    public async Task<Account?> GetCustomerById(long id)
    {
        var accountQuery = await GetAsync(account =>
            Equals(account.Id, id)
            && Equals(account.Role, Role.Customer));
        return accountQuery.FirstOrDefault();
    }

    public IQueryable<Account> GetStaffs()
    {
        return _dbContext.Accounts
            .Where(account => Equals(account.Role, Role.Staff));
    }

    public IQueryable<Account> GetCustomers()
    {
        return _dbContext.Accounts
            .Where(account => Equals(account.Role, Role.Customer));
    }

    public IQueryable<Account> GetAdmins()
    {
        return _dbContext.Accounts
            .Where(account => Equals(account.Role, Role.Admin));
    }
}
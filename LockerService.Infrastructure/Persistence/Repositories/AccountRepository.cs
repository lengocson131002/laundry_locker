using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Enums;
using LockerService.Infrastructure.Persistence.Contexts;

namespace LockerService.Infrastructure.Persistence.Repositories;

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
            && Equals(account.Role, Role.LaundryAttendant));
        return accountQuery.FirstOrDefault();
    }

    public async Task<Account?> GetCustomerByPhoneNumber(string phoneNumber)
    {
        var accountQuery = await Get(account => Equals(account.PhoneNumber, phoneNumber) 
                                                && Equals(account.Role, Role.Customer))
            .Include(account => account.Wallet)
            .FirstOrDefaultAsync();
        
        return accountQuery;
    }

    public IQueryable<Account> GetStaffs(long? storeId = null, List<Role>? roles = null, bool? isActive = null)
    {
        var staffQuery = _dbContext.Accounts
            .Where(account => (storeId == null || account.StoreId == storeId) 
                              && (isActive == null || account.IsActive == isActive) 
                              && account.IsStaff);

        if (roles != null && roles.Count > 0)
        {
            return staffQuery.Where(acc => roles.Contains(acc.Role));
        }

        return staffQuery;
    }

    public IQueryable<Account> GetCustomers(bool? isActive = null)
    {
        return _dbContext.Accounts
            .Where(account => Equals(account.Role, Role.Customer) && (isActive == null || account.IsActive == isActive));
    }

    public IQueryable<Account> GetByUsername(string username)
    {
        return _dbContext.Accounts
            .Where(account => Equals(account.Username, username));
    }

}
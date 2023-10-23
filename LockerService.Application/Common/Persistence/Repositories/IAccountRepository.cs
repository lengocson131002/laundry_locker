namespace LockerService.Application.Common.Persistence.Repositories;

public interface IAccountRepository : IBaseRepository<Account>
{
    public Task<Account?> GetStaffById(long id);

    public Task<Account?> GetCustomerByPhoneNumber(string phoneNumber);

    public IQueryable<Account> GetStaffs(long? storeId = null, Role? role = null, bool? isActive = null);

    public IQueryable<Account> GetCustomers(bool? isActive = null);

    public IQueryable<Account> GetByUsername(string username);

    public IQueryable<Account> GetAdmins(bool? isActive = null);
}

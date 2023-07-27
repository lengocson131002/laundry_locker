namespace LockerService.Application.Common.Persistence.Repositories;

public interface IAccountRepository : IBaseRepository<Account>
{
    public Task<Account?> GetStaffById(long id);

    public Task<Account?> GetStaffByPhoneNumber(string phoneNumber);

    public Task<Account?> GetCustomerByUsername(string username);
    
    public Task<Account?> GetCustomerByPhoneNumber(string phoneNumber);

    public Task<Account?> GetCustomerById(long id);
}
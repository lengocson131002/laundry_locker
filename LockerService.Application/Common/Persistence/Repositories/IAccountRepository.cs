namespace LockerService.Application.Common.Persistence.Repositories;

public interface IAccountRepository : IBaseRepository<Account>
{
    Task<Account?> FindCustomer(string phone);
}
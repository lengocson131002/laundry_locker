namespace LockerService.Application.Common.Services;

public interface ICurrentAccountService
{
    public Task<Account?> GetCurrentAccount();

    public Task<Account> GetRequiredCurrentAccount();
}
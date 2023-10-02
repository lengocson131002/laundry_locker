using LockerService.Application.Common.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LockerService.Infrastructure.Services;

public class CurrentAccountService : ICurrentAccountService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ICurrentPrincipalService _currentPrincipalService;

    public CurrentAccountService(ICurrentPrincipalService currentPrincipalService, IServiceScopeFactory serviceScopeFactory)
    {
        _currentPrincipalService = currentPrincipalService;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<Account?> GetCurrentAccount()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>(); 
        var currentAccountId = _currentPrincipalService.CurrentSubjectId;
        if (currentAccountId == null)
        {
            return null;
        }

        return await unitOfWork.AccountRepository.GetByIdAsync(currentAccountId);
    }
}
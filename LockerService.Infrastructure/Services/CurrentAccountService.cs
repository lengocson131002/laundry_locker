using LockerService.Application.Common.Services;

namespace LockerService.Infrastructure.Services;

public class CurrentAccountService : ICurrentAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentPrincipalService _currentPrincipalService;

    public CurrentAccountService(IUnitOfWork unitOfWork, ICurrentPrincipalService currentPrincipalService)
    {
        _unitOfWork = unitOfWork;
        _currentPrincipalService = currentPrincipalService;
    }

    public async Task<Account?> GetCurrentAccount()
    {
        var currentAccountId = _currentPrincipalService.CurrentSubjectId;
        if (currentAccountId == null)
        {
            return null;
        }

        return await _unitOfWork.AccountRepository.GetByIdAsync(currentAccountId);
    }
}
using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Enums;

namespace LockerService.Infrastructure.Services;

public class ResourceAuthorizeService : IResourceAuthorizeService
{
    private readonly IUnitOfWork _unitOfWork;


    public ResourceAuthorizeService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<bool> AuthorizeStore(Account account, Store store, AuditType? operationType = null)
    {
        if (Equals(operationType, AuditType.Read))
        {
            return Task.FromResult(true);
        }
        
        var authorized = account.IsAdmin
                         || (account.IsManager && Equals(account.StoreId, store.Id));

        return Task.FromResult(authorized);
    }

    public Task<bool> AuthorizeService(Account account, Service service, AuditType? operationType = null)
    {
        if (Equals(operationType, AuditType.Read))
        {
            return Task.FromResult(true);
        }
        
        var authorized = account.IsAdmin
                         || (account.IsManager && Equals(account.StoreId, service.StoreId));

        return Task.FromResult(authorized);
    }

    public Task<bool> AuthorizeStaff(Account account, Account staff, AuditType? operationType = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AuthorizeLocker(Account account, Locker locker, AuditType? operationType = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AuthorizeOrder(Account account, Order order, AuditType? operationType = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AuthorizeNotification(Account account, Notification notification, AuditType? operationType = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AuthorizeCustomer(Account account, Account customer, AuditType? operationType = null)
    {
        throw new NotImplementedException();
    }
}
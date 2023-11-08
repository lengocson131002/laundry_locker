namespace LockerService.Application.Common.Services;

public interface IResourceAuthorizeService
{
    public Task<bool> AuthorizeStore(Account account, Store store, AuditType? operationType = null);

    public Task<bool> AuthorizeService(Account account, Service service, AuditType? operationType = null);

    public Task<bool> AuthorizeStaff(Account account, Account staff, AuditType? operationType = null);

    public Task<bool> AuthorizeLocker(Account account, Locker locker, AuditType? operationType = null);

    public Task<bool> AuthorizeOrder(Account account, Order order, AuditType? operationType = null);

    public Task<bool> AuthorizeNotification(Account account, Notification notification, AuditType? operationType = null);

    public Task<bool> AuthorizeCustomer(Account account, Account customer, AuditType? operationType = null);
}
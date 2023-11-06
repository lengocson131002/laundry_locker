namespace LockerService.Infrastructure.Common.Constants;

public class AuditConstants
{
    public static readonly List<string> TrackedEntities = new List<string>()
    {
        "Account",
        "Box",
        "Hardware",
        "LaundryItem",
        "Locker",
        "LockerOrderType",
        "Order",
        "OrderDetail",
        "Service",
        "Setting",
        "Store"
    };
}
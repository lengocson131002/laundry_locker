namespace LockerService.Application.Common.Persistence;

public interface IApplicationDbContext
{
    DbSet<Account> Accounts { get; }
    
    DbSet<Address> Addresses { get; }

    DbSet<Hardware> Hardwares { get; }

    DbSet<Location> Locations { get; }

    DbSet<Locker> Lockers { get; }

    DbSet<LockerTimeline> LockerTimelines { get; }

    DbSet<Order> Orders { get; }
    
    DbSet<OrderTimeline> OrderTimelines { get; }
    
    DbSet<Service> Services { get; }
    
    DbSet<OrderDetail> OrderDetails { get; }
    
    DbSet<Bill> Bills { get; }
    
    DbSet<Store> Stores { get; }
    
    DbSet<Box> Boxes { get; }
    
    DbSet<Notification> Notifications { get; }
    
    DbSet<Setting> Settings { get; }

    DbSet<StaffLocker> StaffLockers { get; }

    DbSet<Token> Tokens { get; }
}
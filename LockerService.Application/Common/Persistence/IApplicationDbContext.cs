namespace LockerService.Application.Common.Persistence;

public interface IApplicationDbContext
{
    DbSet<Account> Accounts { get; }
    DbSet<AccountLocker> AccountLockers { get; }
    
    DbSet<Address> Addresses { get; }

    DbSet<Hardware> Hardwares { get; }

    DbSet<Location> Locations { get; }

    DbSet<Locker> Lockers { get; }

    DbSet<LockerTimeline> LockerTimelines { get; }

    DbSet<Order> Orders { get; }
    
    DbSet<OrderTimeline> OrderTimelines { get; }
    
    DbSet<Service> Services { get; }
    DbSet<Store> Stores { get; }

}
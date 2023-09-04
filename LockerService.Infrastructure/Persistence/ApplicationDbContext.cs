namespace LockerService.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    
    private readonly AuditableEntitySaveChangesInterceptor _saveChangesInterceptor;
    
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options, AuditableEntitySaveChangesInterceptor saveChangesInterceptor) : base(options)
    {
        _saveChangesInterceptor = saveChangesInterceptor;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_saveChangesInterceptor);
    }

    public DbSet<Account> Accounts => Set<Account>();
    
    public DbSet<Address> Addresses => Set<Address>();
    
    public DbSet<Hardware> Hardwares => Set<Hardware>();
    
    public DbSet<Location> Locations => Set<Location>();

    public DbSet<Locker> Lockers => Set<Locker>();

    public DbSet<LockerTimeline> LockerTimelines => Set<LockerTimeline>();
    
    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderTimeline> OrderTimelines => Set<OrderTimeline>();

    public DbSet<Service> Services => Set<Service>();

    public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();

    public DbSet<Bill> Bills => Set<Bill>();
        
    public DbSet<StaffLocker> AccountLockers => Set<StaffLocker>();

    public DbSet<Store> Stores => Set<Store>();

    public DbSet<Box> Boxes => Set<Box>();

    public DbSet<Notification> Notifications => Set<Notification>();

    public DbSet<Setting> Settings => Set<Setting>();

    public DbSet<StaffLocker> StaffLockers => Set<StaffLocker>();

    public DbSet<Token> Tokens => Set<Token>();

    public DbSet<LaundryItem> LaundryItems => Set<LaundryItem>();
}
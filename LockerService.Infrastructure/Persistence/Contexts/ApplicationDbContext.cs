using LockerService.Domain;
using LockerService.Infrastructure.Persistence.Interceptors;

namespace LockerService.Infrastructure.Persistence.Contexts;

public class ApplicationDbContext : DbContext
{
    
    private readonly AuditableEntitySaveChangesInterceptor _saveChangesInterceptor;
    
    private readonly SoftDeleteInterceptor _softDeleteInterceptor;
    
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options, AuditableEntitySaveChangesInterceptor saveChangesInterceptor, SoftDeleteInterceptor softDeleteInterceptor) : base(options)
    {
        _saveChangesInterceptor = saveChangesInterceptor;
        _softDeleteInterceptor = softDeleteInterceptor;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_saveChangesInterceptor);
        // .AddInterceptors(_softDeleteInterceptor);
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        // {
        //     if (typeof(BaseAuditableEntity).IsAssignableFrom(entityType.ClrType))
        //     {
        //         entityType.AddSoftDeleteQueryFilter();
        //     }
        // }
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

    public DbSet<Audit> Audits => Set<Audit>();

    public DbSet<Payment> Payments => Set<Payment>();

    public DbSet<StoreService> StoreServices => Set<StoreService>();
}
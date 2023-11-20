using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LockerService.Domain.Entities;

/**
 * Mapping between store and service
 * and config service price for each store
 */

[Table("StoreService")]
public class StoreService : BaseAuditableEntity
{
    [Key]
    public long Id { get; set; }
    
    public long ServiceId { get; set; }

    public Service Service { get; set; } = default!;

    public long StoreId { get; set; }

    public Store Store { get; set; } = default!;
    
    public decimal Price { get; set; }

    public StoreService()
    {
        
    }

    public StoreService(long storeId, long serviceId, decimal price)
    {
        StoreId = storeId;
        ServiceId = serviceId;
        Price = price;
    }
}
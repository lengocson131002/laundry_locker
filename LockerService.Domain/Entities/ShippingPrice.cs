using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LockerService.Domain.Entities;

[Table("ShippingPrice")]
public class ShippingPrice : BaseAuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    public decimal Price { get; set; }
    
    public double FromDistance { get; set; } // In KM

    public ShippingPrice() {}

    public ShippingPrice(double fromDistance, decimal price)
    {
        FromDistance = fromDistance;
        Price = price;
    }

    // From: 0KM : 0 VND
    // From: 1KM : 5000 VND
    // FROM 10KM : 8000 VND
}
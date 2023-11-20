using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Infrastructure.Persistence.Contexts;

namespace LockerService.Infrastructure.Persistence.Repositories;

public class ShippingPriceRepository : BaseRepository<ShippingPrice>, IShippingPriceRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ShippingPriceRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ShippingPrice?> GetByFromDistance(double from)
    {
        return await _dbContext.ShippingPrices
            .Where(p => p.FromDistance == from)
            .FirstOrDefaultAsync();
    }

    public async Task<decimal> CalculateShippingPrice(double distance)
    {
        var shippingFee = await _dbContext.ShippingPrices
            .Where(p => p.FromDistance < distance)
            .OrderByDescending(p => p.FromDistance)
            .FirstOrDefaultAsync();

        return shippingFee?.Price ?? 0;
    }

    public async Task<decimal> CalculateShippingPrice(Location from, Location to)
    {
        var distances = CalculateDistance(
            from.Latitude ?? 0, 
            from.Longitude ?? 0,
            to.Latitude ?? 0, 
            to.Longitude ?? 0);
        
        return await CalculateShippingPrice(distances);
        
    }

    public double CalculateDistance(double lat1, double long1, double lat2, double long2)
    {
        var d1 = lat1 * (Math.PI / 180.0);
        var num1 = long1 * (Math.PI / 180.0);
        var d2 = lat2 * (Math.PI / 180.0);
        var num2 = long2 * (Math.PI / 180.0) - num1;
        var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
        return 6371 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
    }
}
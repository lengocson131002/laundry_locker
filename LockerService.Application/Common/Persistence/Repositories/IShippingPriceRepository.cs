namespace LockerService.Application.Common.Persistence.Repositories;

public interface IShippingPriceRepository : IBaseRepository<ShippingPrice>
{
    Task<ShippingPrice?> GetByFromDistance(double from);

    Task<decimal> CalculateShippingPrice(double distance);

    Task<decimal> CalculateShippingPrice(Location from, Location to);

    public double CalculateDistance(double lat1, double long1, double lat2, double long2);
}
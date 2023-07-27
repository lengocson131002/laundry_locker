using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Infrastructure.Persistence;

namespace LockerService.Infrastructure.Repositories;

public class AddressRepository : BaseRepository<Address>, IAddressRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AddressRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Address?> CheckProvince(string provinceCode)
    {
        var provinceQuery =
            await GetAsync(p => p.Code != null && p.Code.Equals(provinceCode));
        return provinceQuery.FirstOrDefault();
    }

    public async Task<Address?> CheckDistrict(string districtCode, string provinceCode)
    {
        var districtQuery =
            await GetAsync(p => p.Code != null && p.Code.Equals(districtCode));
        var district = districtQuery.FirstOrDefault();

        return district is not null && Equals(district.ParentCode, provinceCode) ? district : null;
    }

    public async Task<Address?> CheckWardCode(string wardCode, string districtCode)
    {
        var wardQuery =
            await GetAsync(p => p.Code != null && p.Code.Equals(wardCode));
        var ward = wardQuery.FirstOrDefault();

        return ward is not null && Equals(ward.ParentCode, districtCode) ? ward : null;
    }
}
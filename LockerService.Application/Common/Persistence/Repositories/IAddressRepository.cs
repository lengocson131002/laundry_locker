namespace LockerService.Application.Common.Persistence.Repositories;

public interface IAddressRepository : IBaseRepository<Address>
{
    public Task<Address?> CheckProvince(string provinceCode);

    public Task<Address?> CheckDistrict(string districtCode, string provinceCode);
    public Task<Address?> CheckWardCode(string wardCode, string districtCode);
}
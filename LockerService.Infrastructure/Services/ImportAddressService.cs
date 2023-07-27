using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LockerService.Infrastructure.Services;

public class ImportAddressService : IHostedService, IDisposable
{
    private const string FilePath = "address.json";

    private const string ProvinceIdKey = "province_id";

    private const string ProvinceNameKey = "province_name";

    private const string DistrictKey = "districts";
    
    private const string DistrictIdKey = "district_id";

    private const string DistrictNameKey = "district_name";
    
    private const string WardKey = "wards";
    
    private const string WardIdKey = "ward_id";

    private const string WardNameKey = "ward_name";
    
    private readonly ILogger<ImportAddressService> _logger;

    private readonly IServiceScopeFactory _factory;

    public ImportAddressService(IServiceScopeFactory factory)
    {
        _factory = factory;
        
        using var scope = _factory.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        _logger = serviceProvider.GetRequiredService<ILogger<ImportAddressService>>();
    }

    public void Dispose()
    {
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var addresses = ReadAddressDataFromJson(FilePath);
        
        using var scope = _factory.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        
        if (!unitOfWork.AddressRepository.Any())
        {
            _logger.LogInformation("Start import address data");

            await unitOfWork.AddressRepository.AddRange(addresses);
            await unitOfWork.SaveChangesAsync();
        }
        
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stop import address data");
        return Task.CompletedTask;
    }

    private IEnumerable<Address> ReadAddressDataFromJson(string filePath)
    {
        List<Address> addresses = new();
        using var streamReader = new StreamReader(filePath);
        using var textReader = new JsonTextReader(streamReader);

        var obj = (JObject)JToken.ReadFrom(textReader);

        foreach (var provinceId in obj.Properties().Select(p => p.Name))
        {
            var provinceObj = (JObject?)obj[provinceId];
            if (provinceObj == null) continue;

            var province = new Address()
            {
                Code = (string?)provinceObj[ProvinceIdKey],
                Name = (string?)provinceObj[ProvinceNameKey],
            };

            var districtObjs = (JObject?) provinceObj[DistrictKey]; 
            if (districtObjs == null) continue; 
            
            foreach (var districtId in districtObjs.Properties().Select(p => p.Name))
            {
                var districtObj = (JObject?) districtObjs[districtId];
                if (districtObj == null) continue;

                var district = new Address()
                {
                    Code = (string?)districtObj[DistrictIdKey],
                    Name = (string?)districtObj[DistrictNameKey],
                    ParentCode = province.Code
                };
                
                var wardObjs = (JObject?) districtObj[WardKey]; 
                if (wardObjs == null) continue; 
                foreach (var wardId in wardObjs.Properties().Select(p => p.Name))
                {
                    var wardObj = (JObject?) wardObjs[wardId];
                    if (wardObj == null) continue;

                    var ward = new Address()
                    {
                        Code = (string?)wardObj[WardIdKey],
                        Name = (string?)wardObj[WardNameKey],
                        ParentCode = district.Code
                    };
                
                    addresses.Add(ward);
                }
                
                addresses.Add(district);
            }
            
            addresses.Add(province);
        }
        
        return addresses;
    }
}
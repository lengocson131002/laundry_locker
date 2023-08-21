using LockerService.Application.Settings.Commands;
using LockerService.Application.Settings.Models;
using LockerService.Domain.Entities.Settings;

namespace LockerService.Application.Settings.Handlers;

public class UpdateSettingHandler : IRequestHandler<UpdateSettingsCommand, SettingsResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ISettingService _settingService;

    public UpdateSettingHandler(IUnitOfWork unitOfWork, IMapper mapper, ISettingService settingService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _settingService = settingService;
    }

    public async Task<SettingsResponse> Handle(UpdateSettingsCommand request, CancellationToken cancellationToken)
    {
        var settingsResponse = new SettingsResponse(); 
        if (request.InformationSettings != null)
        {
            var informationSettings = _mapper.Map<InformationSettings>(request.InformationSettings);
            await _settingService.UpdateSettings(informationSettings, cancellationToken);
            settingsResponse.InformationSettings = informationSettings;
        }

        if (request.AccountSettings != null)
        {
            var accSettings = _mapper.Map<AccountSettings>(request.AccountSettings);
            await _settingService.UpdateSettings(accSettings, cancellationToken);
            settingsResponse.AccountSettings = accSettings;
        }

        if (request.OrderSettings != null)
        {
            var orderSettings = _mapper.Map<OrderSettings>(request.OrderSettings);
            await _settingService.UpdateSettings(orderSettings, cancellationToken);
            settingsResponse.OrderSettings = orderSettings;
        }

        if (request.ZaloAuthSettings != null)
        {
            var zaloAuthSettings = _mapper.Map<ZaloAuthSettings>(request.ZaloAuthSettings);
            await _settingService.UpdateSettings(zaloAuthSettings, cancellationToken);
            settingsResponse.ZaloAuthSettings = zaloAuthSettings;
        }

        return settingsResponse;
    }
}
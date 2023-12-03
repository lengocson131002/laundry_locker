using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Settings.Commands;
using LockerService.Application.Features.Settings.Models;
using LockerService.Domain.Entities.Settings;

namespace LockerService.Application.Features.Settings.Handlers;

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
        
        // Update general settings
        if (request.InformationSettings != null)
        {
            var informationSettings = _mapper.Map<InformationSettings>(request.InformationSettings);
            await _settingService.UpdateSettings(informationSettings, cancellationToken);
        }

        // Update account settings
        if (request.AccountSettings != null)
        {
            var accSettings = _mapper.Map<AccountSettings>(request.AccountSettings);
            await _settingService.UpdateSettings(accSettings, cancellationToken);
        }

        // Update order settings
        if (request.OrderSettings != null)
        {
            var orderSettings = _mapper.Map<OrderSettings>(request.OrderSettings);
            await _settingService.UpdateSettings(orderSettings, cancellationToken);
        }

        // Update zalo oauth settings
        if (request.ZaloAuthSettings != null)
        {
            var zaloAuthSettings = _mapper.Map<ZaloAuthSettings>(request.ZaloAuthSettings);
            await _settingService.UpdateSettings(zaloAuthSettings, cancellationToken);
        }

        // Update time settings
        if (request.TimeSettings != null)
        {
            var timeSettings = _mapper.Map<TimeSettings>(request.TimeSettings);
            await _settingService.UpdateSettings(timeSettings, cancellationToken);
        }
        
        // Update lockers settings
        if (request.LockerSettings != null)
        {
            var lockerSettings = _mapper.Map<LockerSettings>(request.LockerSettings);
            await _settingService.UpdateSettings(lockerSettings, cancellationToken);
        }

        // Update payment settings
        if (request.PaymentSettings != null)
        {
            var paymentSettings = _mapper.Map<PaymentSettings>(request.PaymentSettings);
            await _settingService.UpdateSettings(paymentSettings, cancellationToken);
        }

        var response = _mapper.Map<SettingsResponse>(request);
        return response;
    }
}
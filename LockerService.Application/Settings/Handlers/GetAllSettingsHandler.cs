using LockerService.Application.Settings.Models;
using LockerService.Application.Settings.Queries;
using LockerService.Domain.Entities.Settings;

namespace LockerService.Application.Settings.Handlers;

public class GetAllSettingsHandler : IRequestHandler<GetSettingsQuery,SettingsResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSettingsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SettingsResponse> Handle(GetSettingsQuery request, CancellationToken cancellationToken)
    {
        var settingRepo = _unitOfWork.SettingRepository;
        var infoSettingsTask = settingRepo.GetSettings<InformationSettings>();
        var accountSettingsTask = settingRepo.GetSettings<AccountSettings>();
        var orderSettingsTask = settingRepo.GetSettings<OrderSettings>();

        await Task.WhenAll(infoSettingsTask, accountSettingsTask, orderSettingsTask);
        
        return new SettingsResponse()
        {
            InformationSettings = infoSettingsTask.Result,
            AccountSettings = accountSettingsTask.Result,
            OrderSettings = orderSettingsTask.Result
        };
    }
}
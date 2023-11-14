using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Services.Commands;
using LockerService.Application.Features.Services.Models;

namespace LockerService.Application.Features.Services.Handlers;

public class AddServiceHandler : IRequestHandler<AddServiceCommand, ServiceResponse>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public AddServiceHandler(
        ILogger<AddServiceHandler> logger,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResponse> Handle(AddServiceCommand request, CancellationToken cancellationToken)
    {
        
        var service = _mapper.Map<Service>(request);
        
        service = await _unitOfWork.ServiceRepository.AddAsync(service);
        
        // Save changes 
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ServiceResponse>(service);
        
    }
}
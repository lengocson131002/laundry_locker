using LockerService.Application.Services.Commands;

namespace LockerService.Application.Services.Handlers;

public class UpdateServiceHandler : IRequestHandler<UpdateServiceCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateServiceHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }


    public async Task Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {

        var service = await _unitOfWork.ServiceRepository.GetByIdAsync(request.ServiceId);
        if (service is null)
        {
            throw new ApiException(ResponseCode.ServiceErrorNotFound);
        }

        if (!string.IsNullOrEmpty(request.Name) && !service.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase))
        {
            var query = await _unitOfWork.ServiceRepository.GetAsync(
                predicate: ser => ser.Name.Equals(service.Name, StringComparison.OrdinalIgnoreCase)
            );

            if (query.Any())
            {
                throw new ApiException(ResponseCode.ServiceErrorExistedName);
            }
            
            service.Name = request.Name;
        }

        if (request.Price != null)
        {
            service.Price = (decimal) request.Price;
        }

        if (!string.IsNullOrEmpty(request.Description))
        {
            service.Description = request.Description;
        }

        if (!string.IsNullOrEmpty(request.Image))
        {
            service.Image = request.Image;
        }

        if (!string.IsNullOrEmpty(request.Unit))
        {
            service.Unit = request.Unit;
        }
        
        await _unitOfWork.ServiceRepository.UpdateAsync(service);
        
        // Save changes
        await _unitOfWork.SaveChangesAsync();
    }
}
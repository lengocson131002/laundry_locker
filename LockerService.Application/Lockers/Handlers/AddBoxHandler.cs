namespace LockerService.Application.Lockers.Handlers;

public class AddBoxHandler : IRequestHandler<AddBoxCommand, StatusResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddBoxHandler> _logger;

    public AddBoxHandler(IUnitOfWork unitOfWork, ILogger<AddBoxHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public Task<StatusResponse> Handle(AddBoxCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
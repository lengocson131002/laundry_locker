namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Lockers;

public class LockerRemoveBoxConsumer : IConsumer<LockerRemoveBoxEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LockerResetBoxesConsumer> _logger;
    
    public LockerRemoveBoxConsumer(IUnitOfWork unitOfWork, ILogger<LockerResetBoxesConsumer> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<LockerRemoveBoxEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Receive locker remove box message: {0}", JsonSerializer.Serialize(message));

        var locker = await _unitOfWork.LockerRepository
            .Get(locker => locker.Code == message.LockerCode)
            .FirstOrDefaultAsync();
        
        if (locker == null)
        {
            return;
        }

        var box = await _unitOfWork.BoxRepository.FindBox(locker.Id, message.BoxNumber);
        if (box == null)
        {
            return;
        }

        box.DeletedAt = DateTimeOffset.UtcNow;
        await _unitOfWork.BoxRepository.UpdateAsync(box);
        await _unitOfWork.SaveChangesAsync();
    }
}
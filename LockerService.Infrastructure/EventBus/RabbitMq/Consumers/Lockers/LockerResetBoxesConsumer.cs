namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Lockers;

public class LockerResetBoxesConsumer : IConsumer<LockerResetBoxesEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LockerResetBoxesConsumer> _logger;
    
    public LockerResetBoxesConsumer(IUnitOfWork unitOfWork, ILogger<LockerResetBoxesConsumer> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<LockerResetBoxesEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Receive locker reset boxes message: {0}", JsonSerializer.Serialize(message));

        var locker = await _unitOfWork.LockerRepository
            .Get(locker => locker.Code == message.LockerCode)
            .FirstOrDefaultAsync();
        
        if (locker == null)
        {
            return;
        }
        
        var boxes = await _unitOfWork.BoxRepository
            .Get(box => box.LockerId == locker.Id && !box.Deleted)
            .ToListAsync();

        foreach (var box in boxes)
        {
            box.DeletedAt = DateTimeOffset.UtcNow;
            await _unitOfWork.BoxRepository.UpdateAsync(box);
        }
        
        await _unitOfWork.SaveChangesAsync();
    }
}
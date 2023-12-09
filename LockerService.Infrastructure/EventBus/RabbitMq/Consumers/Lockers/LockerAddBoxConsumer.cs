using LockerService.Application.Common.Persistence.Repositories;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Lockers;

public class LockerAddBoxConsumer : IConsumer<LockerAddBoxEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LockerResetBoxesConsumer> _logger;
    
    public LockerAddBoxConsumer(IUnitOfWork unitOfWork, ILogger<LockerResetBoxesConsumer> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<LockerAddBoxEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Receive locker add box message: {0}", JsonSerializer.Serialize(message));

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
            box = new Box(locker.Id, message.BoxNumber, message.BoardNo, message.Pin);
            await _unitOfWork.BoxRepository.AddAsync(box);
        }
        else
        {
            box.IsActive = true;
            box.BoardNo = message.BoardNo;
            box.Pin = message.Pin;
            await _unitOfWork.BoxRepository.UpdateAsync(box);
        }
        
        await _unitOfWork.SaveChangesAsync();
    }
}
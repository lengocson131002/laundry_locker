using LockerService.Application.EventBus.RabbitMq.Events.Lockers;
using LockerService.Application.EventBus.RabbitMq.Events.Orders;
using MassTransit;

namespace LockerService.Application.Orders.Handlers;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderResponse>
{
    private readonly ILogger<CreateOrderHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _rabbitMqBus;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderHandler(
        ILogger<CreateOrderHandler> logger,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IPublishEndpoint rabbitMqBus)
    {
        _logger = logger;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _rabbitMqBus = rabbitMqBus;
    }

    public async Task<OrderResponse> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var locker = await _unitOfWork.LockerRepository.GetByIdAsync(command.LockerId);
        if (locker == null) throw new ApiException(ResponseCode.LockerErrorNotFound);

        // Check Locker status
        if (!LockerStatus.Active.Equals(locker.Status))
        {
            throw new ApiException(ResponseCode.LockerErrorNotActive);
        }

        // Check available boxes
        var availableBox = await _unitOfWork.LockerRepository.FindAvailableBox(locker.Id);
        if (availableBox == null)
        {
            var exception = new ApiException(ResponseCode.LockerErrorNoAvailableBox);
            await _rabbitMqBus.Publish(new LockerOverloadedEvent
            {
                LockerId = locker.Id,
                Time = DateTimeOffset.UtcNow,
                ErrorCode = exception.ErrorCode,
                Error = exception.ErrorMessage
            }, cancellationToken);

            throw exception;
        }

        // Check services
        var details = new List<OrderDetail>();
        if (Equals(command.Type, OrderType.Laundry))
        {
            foreach (var serviceId in command.ServiceIds)
            {
                var service = await _unitOfWork.ServiceRepository.GetByIdAsync(serviceId);
                if (service == null || !service.IsActive)
                    throw new ApiException(ResponseCode.OrderErrorServiceIsNotAvailable);

                var orderDetail = new OrderDetail
                {
                    Service = service
                };

                details.Add(orderDetail);
            }
            await _unitOfWork.OrderDetailRepository.AddRange(details);
        }

        // Check sender and receiver
        var senderPhone = command.SenderPhone;
        var sender = await _unitOfWork.AccountRepository.FindCustomer(senderPhone);
        if (sender == null)
        {
            sender = new Account
            {
                Role = Role.Customer,
                Username = senderPhone,
                PhoneNumber = senderPhone
            };
            await _unitOfWork.AccountRepository.AddAsync(sender);
        }

        var receiverPhone = command.ReceiverPhone;
        Account? receiver = null;
        if (!string.IsNullOrEmpty(receiverPhone) && !Equals(senderPhone, receiverPhone))
        {
            receiver = await _unitOfWork.AccountRepository.FindCustomer(receiverPhone);
            if (receiver == null)
            {
                receiver = new Account
                {
                    Role = Role.Customer,
                    Username = receiverPhone,
                    PhoneNumber = receiverPhone
                };
                await _unitOfWork.AccountRepository.AddAsync(receiver);
            }
        }

        var order = new Order
        {
            LockerId = command.LockerId,
            Type = command.Type,
            Details = details,
            Status = OrderStatus.Initialized,
            Sender = sender,
            Receiver = receiver,
            SendBox = availableBox,
            ReceiveBox = availableBox
        };
        await _unitOfWork.OrderRepository.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Create new order: {0}", order.Id);

        // push event
        await _rabbitMqBus.Publish(new OrderCreatedEvent()
        {
            OrderId = order.Id,
            Time = DateTimeOffset.UtcNow,
            Status = order.Status
        }, cancellationToken);
        
        return _mapper.Map<OrderResponse>(order);
    }
}
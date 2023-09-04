namespace LockerService.Application.Orders.Queries;

public record GetOrderByPinCodeQuery(string PinCode, long LockerId) : IRequest<OrderDetailResponse>;

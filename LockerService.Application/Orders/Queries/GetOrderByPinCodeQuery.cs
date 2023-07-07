namespace LockerService.Application.Orders.Queries;

public record GetOrderByPinCodeQuery(string PinCode) : IRequest<OrderDetailResponse>;

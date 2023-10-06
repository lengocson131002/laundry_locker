using LockerService.Application.Features.Orders.Models;

namespace LockerService.Application.Features.Orders.Queries;

public record GetOrderByPinCodeQuery(string PinCode, long LockerId) : IRequest<OrderDetailResponse>;

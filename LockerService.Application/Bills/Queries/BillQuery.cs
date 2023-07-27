using LockerService.Application.Bills.Models;

namespace LockerService.Application.Bills.Queries;

public record BillQuery(long OrderId) : IRequest<BillResponse>;

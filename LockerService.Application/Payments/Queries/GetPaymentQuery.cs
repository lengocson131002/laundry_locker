using LockerService.Application.Payments.Models;

namespace LockerService.Application.Payments.Queries;

public record GetPaymentQuery(long Id) : IRequest<PaymentResponse>;
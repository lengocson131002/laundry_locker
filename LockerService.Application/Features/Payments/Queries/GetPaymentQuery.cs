using LockerService.Application.Features.Payments.Models;

namespace LockerService.Application.Features.Payments.Queries;

public record GetPaymentQuery(long Id) : IRequest<PaymentResponse>;
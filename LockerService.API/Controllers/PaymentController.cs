using LockerService.API.Attributes;
using LockerService.Application.Payments.Models;
using LockerService.Application.Payments.Queries;
using LockerService.Domain;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/payments")]
[ApiKey]
public class PaymentController : ApiControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<PaginationResponse<Payment, PaymentResponse>>> GetAllPayments(
        [FromQuery] GetAllPaymentQuery query)
    {

        return await Mediator.Send(query);

    }
    
    [HttpGet("{id:long}")]
    public async Task<ActionResult<PaymentResponse>> QueryPayment([FromRoute] long id)
    {
        return await Mediator.Send(new GetPaymentQuery(id));
    }
}
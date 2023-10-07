using LockerService.API.Attributes;
using LockerService.Application.Features.Payments.Commands;
using LockerService.Application.Features.Payments.Models;
using LockerService.Application.Features.Payments.Queries;
using LockerService.Domain;
using LockerService.Shared.Utils;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/payments")]
public class PaymentController : ApiControllerBase
{
    private readonly ILogger<PaymentController> _logger;
    
    public PaymentController(ILogger<PaymentController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Authorize]
    [ApiKey]
    public async Task<ActionResult<PaginationResponse<Payment, PaymentResponse>>> GetAllPayments(
        [FromQuery] GetAllPaymentQuery query)
    {

        return await Mediator.Send(query);

    }
    
    [HttpGet("{id:long}")]
    [ApiKey]
    public async Task<ActionResult<PaymentResponse>> QueryPayment([FromRoute] long id)
    {
        return await Mediator.Send(new GetPaymentQuery(id));
    }

    [HttpPost("callback/momo/{referenceId}")]
    public async Task MomoPaymentCallbackPost([FromRoute] string referenceId, [FromBody] MomoPaymentCallback callback)
    {
        _logger.LogInformation("PaymentRefId: {0}, Response: {1}", referenceId, JsonSerializerUtils.Serialize(callback));
        callback.PaymentReferenceId = referenceId;
        await Mediator.Send(callback);
    }
    
    [HttpGet("callback/momo/{referenceId}")]
    public async Task MomoPaymentCallbackGet([FromRoute] string referenceId, [FromQuery] MomoPaymentCallback callback)
    {
        _logger.LogInformation("PaymentRefId: {0}, Response: {1}", referenceId, JsonSerializerUtils.Serialize(callback));
        callback.PaymentReferenceId = referenceId;
        await Mediator.Send(callback);
    }
    
    [HttpGet("callback/vnpay/{referenceId}")]
    public async Task VnPayPaymentCallbackGet([FromRoute] string referenceId, [FromQuery] VnPayPaymentCallback callback)
    {
        _logger.LogInformation("PaymentRefId: {0}, Response: {1}", referenceId, JsonSerializerUtils.Serialize(callback));
        callback.PaymentReferenceId = referenceId;
        await Mediator.Send(callback);
    }


    [HttpPost]
    public async Task<ActionResult<StatusResponse>> TestPayment([FromBody] TestPaymentCommand command)
    {
        return await Mediator.Send(command);
    } 
}
using LockerService.API.Attributes;
using LockerService.Application.Features.Payments.Commands;
using LockerService.Application.Features.Payments.Models;
using LockerService.Application.Features.Payments.Queries;
using LockerService.Domain;
using LockerService.Shared.Utils;

namespace LockerService.API.Controllers;

/// <summary>
/// PAYMENT API
/// </summary>
[ApiController]
[Route("/api/v1/payments")]
public class PaymentController : ApiControllerBase
{
    private readonly ILogger<PaymentController> _logger;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    public PaymentController(ILogger<PaymentController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all payments
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize]
    [ApiKey]
    public async Task<ActionResult<PaginationResponse<Payment, PaymentResponse>>> GetAllPayments(
        [FromQuery] GetAllPaymentQuery query)
    {

        return await Mediator.Send(query);

    }
    
    /// <summary>
    /// Get a payment detail
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:long}")]
    [ApiKey]
    public async Task<ActionResult<PaymentDetailResponse>> QueryPayment([FromRoute] long id)
    {
        return await Mediator.Send(new GetPaymentQuery(id));
    }

    /// <summary>
    /// IPN callback for MOMO payment
    /// </summary>
    /// <param name="referenceId"></param>
    /// <param name="callback"></param>
    [HttpPost("callback/momo/{referenceId}")]
    public async Task MomoPaymentCallbackPost([FromRoute] string referenceId, [FromBody] MomoPaymentCallback callback)
    {
        _logger.LogInformation("PaymentRefId: {0}, Response: {1}", referenceId, JsonSerializerUtils.Serialize(callback));
        var momoCallbackCommand = new PaymentCallbackCommand()
        {
            PaymentReferenceId = referenceId,
            ReferenceTransactionId = callback.TransId,
            Amount = callback.Amount,
            IsSuccess = callback.IsSuccess
        };
        
        await Mediator.Send(momoCallbackCommand);
    }
    
    /// <summary>
    /// Return URL callback for MOMO payment
    /// </summary>
    /// <param name="referenceId"></param>
    /// <param name="callback"></param>
    [HttpGet("callback/momo/{referenceId}")]
    public async Task MomoPaymentCallbackGet([FromRoute] string referenceId, [FromQuery] MomoPaymentCallback callback)
    {
        _logger.LogInformation("PaymentRefId: {0}, Response: {1}", referenceId, JsonSerializerUtils.Serialize(callback));
        var momoCallbackCommand = new PaymentCallbackCommand()
        {
            PaymentReferenceId = referenceId,
            ReferenceTransactionId = callback.TransId,
            Amount = callback.Amount,
            IsSuccess = callback.IsSuccess
        };
        
        await Mediator.Send(momoCallbackCommand);
    }
    
    /// <summary>
    /// Return URL callback for VnPAY payment
    /// </summary>
    /// <param name="referenceId"></param>
    /// <param name="callback"></param>
    [HttpGet("callback/vnpay/{referenceId}")]
    public async Task VnPayPaymentCallbackGet([FromRoute] string referenceId, [FromQuery] VnPayPaymentCallback callback)
    {
        _logger.LogInformation("PaymentRefId: {0}, Response: {1}", referenceId, JsonSerializerUtils.Serialize(callback));
        var vnPayCallbackCommand = new PaymentCallbackCommand()
        {
            PaymentReferenceId = referenceId,
            ReferenceTransactionId = callback.vnp_TransactionNo,
            Amount = callback.vnp_Amount ?? 0,
            IsSuccess = callback.IsSuccess
        };
        
        await Mediator.Send(vnPayCallbackCommand);
    }

}
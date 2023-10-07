namespace LockerService.Application.Features.Payments.Commands;

public class MomoPaymentCallback : IRequest
{
    [JsonIgnore]
    [BindNever]
    public string PaymentReferenceId { get; set; } = default!;
    
    public string PartnerCode { get; set; } = default!;

    public string OrderId { get; set; } = default!;

    public string RequestId { get; set; } = default!;
    
    public decimal Amount { get; set; }

    public string OrderInfo { get; set; } = default!;

    public string OrderType { get; set; } = default!;

    public string TransId { get; set; } = default!;

    public int ResultCode { get; set; } = default!;

    public string Message { get; set; } = default!;

    public string PayType { get; set; } = default!;

    public string ResponseTime { get; set; } = default!;

    public string ExtraData { get; set; } = default!;

    public string Signature { get; set; } = default!;

    public bool IsSuccess => Equals(ResultCode, 0);
}
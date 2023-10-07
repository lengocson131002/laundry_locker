namespace LockerService.Application.Features.Payments.Commands;

public class VnPayPaymentCallback : IRequest
{
    [JsonIgnore]
    [BindNever] 
    public string PaymentReferenceId { get; set; } = default!;
    
    public string? vnp_TransactionStatus { get; set; } = default!;
    
    public string? vnp_TmnCode { get; set; } = default!;
    
    public long? vnp_Amount { get; set; } = default!;

    public string? vnp_BankCode { get; set; } = default!;
    
    public string? vnp_BankTranNo { get; set; } = default!;
    
    public string? vnp_CardType { get; set; } = default!;
    
    public string? vnp_PayDate { get; set; } = default!;
    
    public string? vnp_SecureHashType	 { get; set; } = default!;

    public string? vnp_TransactionNo { get; set; } = default!;

    public string? vnp_TxnRef { get; set; } = default!;

    public string? vnp_OrderInfo { get; set; } = default!;
    
    public string? vnp_SecureHash { get; set; } = default!;

    public string? vnp_ResponseCode { get; set; } = default!;

    public bool IsSuccess => "00".Equals(vnp_ResponseCode);
}
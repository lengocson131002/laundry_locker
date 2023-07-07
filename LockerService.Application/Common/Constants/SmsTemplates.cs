namespace LockerService.Application.Common.Constants;

public static class SmsTemplates
{
    public const string OrderCreatedSmsTemplate = "Bạn có một đơn hàng đã gửi tại Locker.\nDịch vụ: {0}. \nNhận tại địa chỉ: {1}. \nNhập PIN CODE : {2} để nhận hàng.\nXin cảm ơn";
    
    public const string OrderReturnedSmsTemplate = "Bạn có một đơn hàng đã được xử lý tại Locker.\nDịch vụ: {0}. \nNhận tại Địa chỉ: {1}.\nPhí: {2} \nNhập PIN CODE : {3} để nhận hàng.\nXin cảm ơn";

}
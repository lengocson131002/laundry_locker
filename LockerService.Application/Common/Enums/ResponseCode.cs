using System.ComponentModel;

namespace LockerService.Application.Common.Enums;

public enum ResponseCode
{
    [Description("Có lỗi xảy ra")] CommonError = 1,

    [Description("Lỗi định dạng dữ liệu")] ValidationError = 2,

    [Description("Lỗi mapping")] MappingError = 3,
    
    [Description("Tài khoản không được xác thực")] Unauthorized = 4,
    
    [Description("Tài khoản không được phép truy cập tài nguyên này")] Forbidden = 5,

    // File 
    
    [Description("Không tìm thấy file yêu cầu")] FileErrorNotFound = 10,

    [Description("Đã xảy ra lỗi khi xóa file. Vui lòng thử lại")] FileErrorDeleteFailed = 11,
    
    [Description("Đã xảy ra lỗi khi upload file. Vui lòng thử lại")] FileErrorUploadFailed = 12,
    
    // Auth
        
    [Description("Username hoặc Mật khẩu không chính xác. Vui lòng thử lại")] AuthErrorInvalidUsernameOrPassword = 20,
    
    [Description("Refresh token không hợp lệ")] AuthErrorInvalidRefreshToken = 21,
    
    [Description("Invalid google ID token")] AuthErrorInvalidGoogleIdToken = 22,
    
    [Description("Không tìm thấy tài khoản")] AuthErrorAccountNotFound = 23,
    
    [Description("Username hoặc OTP không hợp lệ. Vui lòng thử lại")] AuthErrorInvalidUsernameOrOtp = 24,
    
    [Description("Mật khẩu hiện tại không chính xác. Vui lòng thử lại")] AuthErrorCurrentPasswordIncorrect = 25,
    
    [Description("Mật khẩu mới phải khác với mật khẩu hiện tại. Vui lòng thử lại")] AuthErrorNewPasswordMustBeDifferent = 26,
    
    [Description("Tài khoản bị vô hiệu hóa. Vui lòng liên hệ quản trị viên để mở")] AuthErrorAccountInactive = 27,
    
    [Description("Tài khoản cần được cập nhật mật khẩu")] AuthErrorUpdatePasswordRequest = 28,
    
  
    // Locker
    [Description("Không tìm thấy Locker yêu cầu")] LockerErrorNotFound = 101,
    
    [Description("Trạng thái hiện tại của Locker không cho phép thực hiện thao tác này")] LockerErrorInvalidStatus = 102,

    [Description("Cập nhật ô tủ thất bại. Vui lòng thử lại")] LockerErrorInvalidNumberOfBoxes = 103,
    
    [Description("Thêm ô tủ thâất bại. Vui lòng thử lại")] LockerErrorOverBoxCount = 104,
    
    [Description("Cần cập nhật dịch vụ cho Locker trước khi ")] LockerErrorServiceRequired = 105,

    [Description("Địa chỉ MAC đã tồn tại. Vui lòng thử lại")] LockerErrorExistedMacAddress = 106,
    
    [Description("Tên Locker đã tồn tại. Vui lòng thử lại")] LockerErrorExistedName = 107,
    
    [Description("Không tìm thấy Locker. Vui lòng thử lại")] LockerErrorNotActive = 108,
    
    [Description("Không còn ô tủ trống. Vui lòng quay lại sau")] LockerErrorNoAvailableBox = 109,
    
    [Description("Không tìm thấy ô tủ")] LockerErrorBoxNotFound = 110,
    
    [Description("Trạng thái hiện tại của Ô tủ không cho phép thực hiện thao tác này.ép thực hiện thao tác này. Vui lòng thử lại")] LockerErrorInvalidBoxStatus = 111,
    
    [Description("Loại đơn hàng không đuợc hỗ trợ. Vui lòng thử lại")] LockerErrorUnsupportedOrderType = 112,


    // Service
    [Description("Không tìm thấy dịch vụ được yêu cầu")] ServiceErrorNotFound = 201,
    
    [Description("Cần cập nhật giá tiền cho mỗi dịch vụ trong một đơn hàng")] ServiceErrorMissingFee = 202,

    [Description("Tên dịch vụ đã tồn tại. Vui lòng thử lại")] ServiceErrorExistedName = 203,
    
    [Description("Trạng thái hiện tại của Dịch vụ không cho phép thực hiện thao tác này")] ServiceErrorInvalidStatus = 204,
    

    // Order
    [Description("Dịch vụ không tìm thấy hoặc không hoạt động")] OrderErrorServiceIsNotAvailable = 403,
    
    [Description("Không tìm thấy Đơn hàng")] OrderErrorNotFound = 404,
    
    [Description("Trạng thái hiện tại của Đơn hàng không cho phép thực hiện thao tác này")] OrderErrorInvalidStatus = 405,
    
    [Description("Order'amount is required for ByUnitPrice Service")] OrderErrorAmountIsRequired = 406,
    
    [Description("Order'fee is required for ByInputPrice Service")] OrderErrorFeeIsRequired = 407,
    
    [Description("Fee of this order's service is missing")] OrderErrorServiceFeeIsMissing = 408,
    
    [Description("FeeType of this order's service is missing")] OrderErrorServiceFeeTypeIsMissing = 409,
    
    [Description("Tài khoản bị vô hiệu hóa không được tạo đơn hàng")] OrderErrorInactiveAccount = 410,
    
    [Description("Số điện thoại {0} không thể tạo order vì đang có số Đơn hàng còn hoạt động vượt mức cho phép là {1} đơn hàng")] OrderErrorExceedAllowOrderCount = 411,

    [Description("Thời gian nhận không hợp lệ")] OrderErrorInvalidReceiveTime = 412,

    // Address
    [Description("Không tìm thấy tỉnh/thành phố")] AddressErrorProvinceNotFound = 501,
    
    [Description("Không tìm thấy quận huyện")] AddressErrorDistrictNotFound = 502,

    [Description("Không tìm thấy xã/phường")] AddressErrorWardNotFound = 503,


    // Hardware
    [Description("Không tìm thấy phần cứng")] HardwareErrorNotFound = 601,
    
    // Staff
    [Description("Không tìm thấy nhân viên")] StaffErrorNotFound = 701,
    
    [Description("Trạng thái hiện tại của Nhân viên không cho phép thực hiện thao tác này")] StaffErrorInvalidStatus = 702,
    
    [Description("Nhân viên thuộc về một cửa hàng")] StaffErrorBelongToAStore = 703,
    
    [Description("Nhân viên đã được gán quyền quản lý Locker trước đó")] StaffErrorAssignedBefore = 704,
    
    [Description("Nhân viên đã được gán quyền quản lý Locker trước đó")] StaffErrorInAssignment = 705,

    [Description("Số điện thoại nhân viên đã tồn tại")] StaffErrorExisted = 706,

    // Store
    [Description("Không tìm thấy cửa hàng")] StoreErrorNotFound = 801,
    
    [Description("Trạng thái hiện tại của cửa hàng không cho phép thực hiện thao tác này")] StoreErrorInvalidStatus = 802,
    
    [Description("Nhân viên và Locker cần chung một cửa hàng")] StoreErrorStaffAndLockerNotInSameStore = 803,
    
    // Staff Locker
    [Description("Không tìm thấy lệnh giao nhiệm vụ")] StaffLockerErrorNotFound = 901,
    
    [Description("Nhân viên đã được giao nhiệm vụ quản lý Locker")] StaffLockerErrorExisted = 902,
    
    [Description("Nhân viên không có quyền thao tác với Locker")] StaffLockerErrorNoPermission = 903,

    // Account
    [Description("Username đã tồn tại")] AccountErrorUsernameExisted = 1001,
    
    [Description("Số điện thoại đã tồn tại")] AccountErrorPhoneNumberExisted = 1002,
    
    [Description("Trạng thái hiện tại của Tài khoản không cho phép thực hiện thao tác này")] AccountErrorInvalidStatus = 1003,

    
    // OrderDetail
    [Description("Không tìm thấy chi tiết đơn hàng")] OrderDetailErrorNotFound = 1201,
    
    [Description("Vui lòng cập nhật chi tiết của đơn hàng")] OrderDetailErrorInfoRequired = 1202,
    
    [Description("Chi tiết đơn hàng đã tồn tại")] OrderDetailErrorExisted = 1203,
    
    [Description("Chi tiết của đơn hàng là bắt buộc")] OrderDetailErrorRequired = 1204,

    // Notification 
    [Description("Không tìm thấy thông báo")] NotificationErrorNotFound = 1301,
    
    [Description("Trạng thái hiện tại của Thông báo không cho phép thực hiện thao tác này")] NotificationErrorInvalidStatus = 1302,
    
    // Laundry Item
    [Description("Không tìm thấy đồ giặt này")] LaundryItemErrorNotFound = 1401,
    
    // Token
    [Description("Mã không hợp lệ hoặc đã hết hạn")] TokenErrorInvalidOrExpiredToken = 1501,

    // Bill
    [Description("Không tìm hóa đơn")] BillErrorNotFound = 1601,
    
    // Payment
    [Description("Không tìm thấy giao dịch thanh toán")] PaymentErrorNotFound = 1701,
    
    [Description("Phương thức thanh toán không được hỗ trợ. Vui lòng chọn phương thức khác")] PaymentErrorUnsupportedPaymentMethod = 1702,


    // Store service configuration
    [Description("Cửa hàng đã được thiết lập dịch vụ này trước đó")] StoreServiceErrorExisted = 1801,
    
    [Description("Cửa hàng chưa được thiết lập dịch vụ này trước đó")] StoreServiceErrorNotGFound = 1802,

    // Shipping price
    [Description("Giá ship đã được xác định cho khoảng cách này")] ShippingPriceErrorExisted = 1901,
    
    [Description("Không tìm thấy giá ship")] ShippingPriceErrorNotFound = 1902,

    // Wallet
    [Description("Số dư không đủ để thử hiện thao tác này")] WalletErrorInvalidBalance= 2001,
    
    [Description("Không tìm thấy ví của khách hàng")] WalletErrorNotFound= 2002,

    [Description("Số tiền cho phép nạp tối thiểu: {0}")] WalletErrorInvalidDepositAmount = 2003

}
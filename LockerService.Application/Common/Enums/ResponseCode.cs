using System.ComponentModel;

namespace LockerService.Application.Common.Enums;

public enum ResponseCode
{
    [Description("Common Error")] CommonError = 1,

    [Description("Validation Error")] ValidationError = 2,

    [Description("Mapping Error")] MappingError = 3,
    
    [Description("Unauthorized")] Unauthorized = 4,

    // File 
    
    [Description("File not found")] FileErrorNotFound = 10,

    [Description("Delete file failed")] FileErrorDeleteFailed = 11,
    
    [Description("Upload file failed")] FileErrorUploadFailed = 12,
    
    // Auth
        
    [Description("Invalid username or password")] AuthErrorInvalidUsernameOrPassword = 20,
    
    [Description("Invalid refresh token")] AuthErrorInvalidRefreshToken = 21,
    
    [Description("Invalid google ID token")] AuthErrorInvalidGoogleIdToken = 22,
    
    [Description("Account not found")] AuthErrorAccountNotFound = 23,
    
    [Description("Invalid username or otp")] AuthErrorInvalidUsernameOrOtp = 24,
    
    [Description("Current Password Incorrect")] AuthErrorCurrentPasswordIncorrect = 25,
    
    [Description("The new password must be different from the current password")] AuthErrorNewPasswordMustBeDifferent = 26,
    
    [Description("The account is inactive")] AuthErrorAccountInactive = 27,
  
    // Locker
    [Description("Locker not found")] LockerErrorNotFound = 101,
    
    [Description("Locker' status is not allow to do this function")] LockerErrorInvalidStatus = 102,

    [Description("Update boxes failed, please scan enough box")] LockerErrorInvalidNumberOfBoxes = 103,
    
    [Description("Add new box failed, scan enough box!")] LockerErrorOverBoxCount = 104,
    
    [Description("Config locker service before being active")] LockerErrorServiceRequired = 105,

    [Description("Existed locker's MAC address")] LockerErrorExistedMacAddress = 106,
    
    [Description("Existed locker's name")] LockerErrorExistedName = 107,
    
    [Description("Locker is currently not active")] LockerErrorNotActive = 108,
    
    [Description("No any available box. Please try later")] LockerErrorNoAvailableBox = 109,
    
    [Description("Box not found")] LockerErrorBoxNotFound = 110,
    
    [Description("Box status is not allowed to do this function")] LockerErrorInvalidBoxStatus = 111,


    // Service
    [Description("Service not found")] ServiceErrorNotFound = 201,
    
    [Description("Service's fee is required")] ServiceErrorMissingFee = 202,

    [Description("Existed service name")] ServiceErrorExistedName = 203,
    
    [Description("Service's status is not allowed to do this function")] ServiceErrorInvalidStatus = 204,
    

    // Order
    [Description("Service is not available")] OrderErrorServiceIsNotAvailable = 403,
    
    [Description("Order is not found")] OrderErrorNotFound = 404,
    
    [Description("Order'status is not allowed to do this function")] OrderErrorInvalidStatus = 405,
    
    [Description("Order'amount is required for ByUnitPrice Service")] OrderErrorAmountIsRequired = 406,
    
    [Description("Order'fee is required for ByInputPrice Service")] OrderErrorFeeIsRequired = 407,
    
    [Description("Fee of this order's service is missing")] OrderErrorServiceFeeIsMissing = 408,
    
    [Description("FeeType of this order's service is missing")] OrderErrorServiceFeeTypeIsMissing = 409,
    
    [Description("Inactive account is not allowed to create order")] OrderErrorInactiveAccount = 410,
    
    [Description("Can't create order because your account has currently had over allowed active order count")] OrderErrorExceedAllowOrderCount = 410,

    // Address
    [Description("Province not found")] AddressErrorProvinceNotFound = 501,
    
    [Description("District not found")] AddressErrorDistrictNotFound = 502,

    [Description("Ward not found")] AddressErrorWardNotFound = 503,


    // Hardware
    [Description("Hardware not found")] HardwareErrorNotFound = 601,
    
    // Staff
    [Description("Staff is not found")] StaffErrorNotFound = 701,
    
    [Description("Staff's status is not allowed to do this function")] StaffErrorInvalidStatus = 702,
    
    [Description("Staff is belonging to a store")] StaffErrorBelongToAStore = 703,
    
    [Description("Staff has been assigned to this locker before")] StaffErrorAssignedBefore = 704,
    
    [Description("Staff has been assigned to the locker(s) in this store")] StaffErrorInAssignment = 705,

    [Description("Staff with request phone number existed")] StaffErrorExisted = 706,

    // Store
    [Description("Store not found")] StoreErrorNotFound = 801,
    
    [Description("Store's status is invalid")] StoreErrorInvalidStatus = 802,
    
    [Description("Staff and Locker do not belong to the same store")] StoreErrorStaffAndLockerNotInSameStore = 803,
    
    // Staff Locker
    [Description("Assignment not found")] StaffLockerErrorNotFound = 901,
    
    [Description("Assignment existed")] StaffLockerErrorExisted = 902,
    
    [Description("Staff has no permission on the request locker")] StaffLockerErrorNoPermission = 903,

    // Account
    [Description("Username existed")] AccountErrorUsernameExisted = 1001,
    
    [Description("Phone Number existed")] AccountErrorPhoneNumberExisted = 1002,
    
    [Description("Account's status is not allowed to do this function")] AccountErrorInvalidStatus = 1003,

    // Bill
    [Description("Bill not found")] BillErrorNotFound = 1101,
    
    // OrderDetail
    [Description("Order item detail not found")] OrderDetailErrorNotFound = 1201,
    
    [Description("Please update order's details")] OrderDetailErrorInfoRequired = 1202,
    
    [Description("Order item detail existed")] OrderDetailExisted = 1203,

}
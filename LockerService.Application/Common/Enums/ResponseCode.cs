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
        
    [Description("Invalid email or password")] AuthErrorInvalidEmailOrPassword = 20,
    
    [Description("Invalid refresh token")] AuthErrorInvalidRefreshToken = 21,
    
    [Description("Invalid google ID token")] AuthErrorInvalidGoogleIdToken = 22,
    
    [Description("Account not found")] AuthErrorAccountNotFound = 23,
    
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

    [Description("Order item detail not found")] OrderErrorDetailNotFound = 410,

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

    // Store
    [Description("Store not found")] StoreErrorNotFound = 801,
    
    [Description("Store's status is not allowed to do this function")] StoreErrorInvalidStatus = 802,
    
}
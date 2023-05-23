using System.ComponentModel;

namespace LockerService.Application.Common.Enums;

public enum ResponseCode
{
    [Description("Common Error")] ErrorCommon = 1,

    [Description("Validation Error")] ErrorValidation = 2,

    [Description("Mapping Error")] ErrorMapping = 3,
    
    [Description("Unauthorized")] Unauthorized = 4,

    // File 
    
    [Description("File not found")] ErrorFileNotFound = 10,

    [Description("Delete file failed")] ErrorDeleteFileFailed = 11,
    
    [Description("Upload file failed")] ErrorUploadFileFailed = 12,
    
    // Auth
        
    [Description("Invalid email or password")] AuthErrorInvalidEmailOrPassword = 20,
    
    [Description("Invalid refresh token")] AuthErrorInvalidRefreshToken = 21,
    
    [Description("Invalid google ID token")] AuthErrorInvalidGoogleIdToken = 22,

    
    // Account
    [Description("Username existed")] AccountErrorEmailExisted = 30,
    
    [Description("Password and Confirm password are not matched")] AccountErrorPasswordNotMatched = 31,

    [Description("Account not found")] AccountErrorNotFound = 32,

    // Review
    [Description("You had reviewed this account before")] ReviewErrorExisted = 40,

    [Description("Review not found")] ReviewErrorNotFound = 41,
    
    [Description("Can not review yourself")] ReviewErrorCannotReviewYourself = 42

    
}
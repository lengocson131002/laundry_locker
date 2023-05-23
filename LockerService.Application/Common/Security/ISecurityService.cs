namespace LockerService.Application.Common.Security;

public interface ISecurityService
{
    Task<string> EncryptToBase64Async(string message, string secretKey);
    
    Task<byte[]> EncryptToByteArrayAsync(string message, string secretKey);
    
    Task<string> DecryptAsync(string secretMessage, string secretKey);
    
    Task<string> DecryptAsync(byte[] encryptedMessage, string secretKey);
}
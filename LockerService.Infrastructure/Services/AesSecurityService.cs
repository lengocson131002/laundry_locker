using System.Security.Cryptography;
using System.Text;
using LockerService.Application.Common.Security;

namespace LockerService.Infrastructure.Services;

public class AesSecurityService : ISecurityService
{
    private byte[] _iv =
    {
        0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
        0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
    };
    
    public async Task<string> EncryptToBase64Async(string message, string secretKey)
    {
        return Convert.ToBase64String(await EncryptToByteArrayAsync(message, secretKey));
    }

    public async Task<byte[]> EncryptToByteArrayAsync(string message, string secretKey)
    {
        var key = Encoding.UTF8.GetBytes(secretKey);
        using var aesAlgorithm = Aes.Create();

        aesAlgorithm.Key = key;
        aesAlgorithm.IV = _iv;
        
        using var encryptor = aesAlgorithm.CreateEncryptor();
        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        await cryptoStream.WriteAsync(Encoding.UTF8.GetBytes(message));
        await cryptoStream.FlushFinalBlockAsync();
        return memoryStream.ToArray();
    }

    public async Task<string> DecryptAsync(string encryptedMessage, string secretKey)
    {
        return await DecryptAsync(Convert.FromBase64String(encryptedMessage), secretKey);
    }

    public async Task<string> DecryptAsync(byte[] encryptedMessage, string secretKey)
    {
        var key = Encoding.UTF8.GetBytes(secretKey);
        using var aesAlgorithm = Aes.Create();
        
        aesAlgorithm.Key = key;
        aesAlgorithm.IV = _iv;
        
        using var decryptor = aesAlgorithm.CreateDecryptor();
        using var memoryInputStream = new MemoryStream(encryptedMessage);
        using var cryptoStream = new CryptoStream(memoryInputStream, decryptor, CryptoStreamMode.Read);

        using var memoryOutputStream = new MemoryStream();
        await cryptoStream.CopyToAsync(memoryOutputStream);
        return Encoding.UTF8.GetString(memoryOutputStream.ToArray());
    }
}
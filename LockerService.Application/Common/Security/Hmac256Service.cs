using System.Security.Cryptography;
using System.Text;

namespace LockerService.Application.Common.Security;

public class Hmac256Service
{
    public static string HashMessage(string message, string key)
    {
        var securedMessage = $"message:{message}|secret_key:{key}";
        string hash;
        var encoder = new UTF8Encoding();
        var code = encoder.GetBytes(key);
        using var hmac = new HMACSHA256(code);
        var hmBytes = hmac.ComputeHash(encoder.GetBytes(securedMessage));
        hash = ToHexString(hmBytes);
        return hash;
    }
    
    private static string ToHexString(byte[] array)
    {
        var hex = new StringBuilder(array.Length * 2);
        foreach (byte b in array)
        {
            hex.AppendFormat("{0:x2}", b);
        }
        return hex.ToString();
    }
    
    public static bool Verify(string message, string signature, string key)
    {
        var hashedMessage = HashMessage(message, key);
        return hashedMessage.Equals(signature);
    }
}
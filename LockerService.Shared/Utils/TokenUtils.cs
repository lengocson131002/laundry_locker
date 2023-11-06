namespace LockerService.Shared.Utils;

public class TokenUtils
{
    private const string AllowedCharacters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static string GeneratePinCode(int length)
    {
        var rand = new Random();

        var otp = string.Empty;

        for (var i = 0; i < length; i++)
        {
            otp += AllowedCharacters[rand.Next(0, AllowedCharacters.Length)];
        }

        return otp;
    }

    public static string GenerateRandomToken()
    {
        return Guid.NewGuid().ToString();
    }
}
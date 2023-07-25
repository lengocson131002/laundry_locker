namespace LockerService.Application.Common.Utils;

public class GeneratorUtils
{
    private const string OnlyNumberCharacters = "0123456789";
    private const string FullCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public static string GenerateToken(int length, bool? onlyNumber = true)
    {
        var rand = new Random();

        var otp = string.Empty;

        var allowedCharacters = onlyNumber == true ? OnlyNumberCharacters : FullCharacters;
        for (var i = 0; i < length; i++)
        {
            otp += allowedCharacters[rand.Next(0, allowedCharacters.Length)];
        }

        return otp;
    }
}
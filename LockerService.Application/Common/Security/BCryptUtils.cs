namespace LockerService.Application.Common.Security;

public static class BCryptUtils
{
    public static string Hash(string input)
    {
        return BCrypt.Net.BCrypt.HashPassword(input);
    }

    public static bool Verify(string input, string hashedValue)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(input, hashedValue);
        }
        catch (Exception ignore)
        {
            // ignore
            return false;
        }
    }
}
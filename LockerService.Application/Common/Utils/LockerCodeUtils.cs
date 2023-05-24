namespace LockerService.Application.Common.Utils;

public class LockerCodeUtils
{
    private const string LockerCodePrefix = "LOCKER_";

    public static string GenerateLockerCode(int id)
    {
        return string.Concat(LockerCodePrefix, id);
    }

    public static string GenerateLockerCode()
    {
        return string.Concat(LockerCodePrefix, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    }
    
}
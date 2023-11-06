namespace LockerService.Shared.Utils;

public class RedisUtils
{
    public static string GenerateOpenBoxTokenKey(long lockerId, string token)
    {
        return $"LOCKER_{lockerId}_OPEN_BOXES_{token}";
    }
}
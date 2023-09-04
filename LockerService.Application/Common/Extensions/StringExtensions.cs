using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace LockerService.Application.Common.Extensions;

public static class StringExtensions
{
    public static string ToNormalize(this string input)
    {
        var regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
        var temp = input.Normalize(NormalizationForm.FormD);
        return regex.Replace(temp, string.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D').Trim();
    }
    
    public static string ToCode(this string input)
    {
        return Regex.Replace(input.ToNormalize().ToLower().Trim(), "\\s+", "-");
    }

    public static bool ContainsSubstring(this string src, string subString, bool isRelative = false)
    {
        return !isRelative ? src.Contains(subString) : src.ToNormalize().Contains(subString.ToNormalize());
    }

    /**
     * Convert phone number to string starts with "+84"
     */
    public static string ToVietnamesePhoneNumber(this string src)
    {
        return Regex.Replace(src.Trim(), "^(84|0)", "+84");
    }

    /**
     * Convert phone number to string starts with "0"
     */
    public static string NormalizePhoneNumber(this string src)
    {
        return Regex.Replace(src.Trim(), "^(84|\\+84)", "0");
    }
    
    public static bool IsValidIpAddress(this string src)
    {
        if (src.Split(".").Length != 4)
        {
            return false;
        }
        return IPAddress.TryParse(src, out IPAddress _);
    }

    public static bool IsValidTimeZone(this string tz)
    {
        try
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(tz);
            return true;
        }
        catch (Exception ex)
        {
            // ignored
        }

        return false;
    }
}
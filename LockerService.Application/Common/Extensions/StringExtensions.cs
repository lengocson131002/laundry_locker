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

    public static string ToVietnamesePhoneNumber(this string src)
    {
        if (!src.IsValidPhoneNumber())
        {
            return string.Empty;
        }

        return Regex.Replace(src, "^(84|0)", "+84");
    }

}
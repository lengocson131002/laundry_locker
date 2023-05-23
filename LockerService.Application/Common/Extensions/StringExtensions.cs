using System.Text;
using System.Text.RegularExpressions;

namespace LockerService.Application.Common.Extensions;

public static class StringExtensions
{
    public static string ToNormalize(this string input)
    {
        var regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
        var temp = input.Normalize(NormalizationForm.FormD);
        return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
    }
    
    public static string ToCode(this string input)
    {
        return Regex.Replace(input.ToNormalize().ToLower().Trim(), "\\s+", "-");
    }
    
}
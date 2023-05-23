using System.Net.Mail;
using System.Text.RegularExpressions;

namespace LockerService.Application.Common.Extensions;

public static class RegexExtensions
{
    
    private static readonly string PhoneRegex = "^(\\+84|84|0)[35789][0-9]{8}$";
    
    public static bool IsValidEmail(this string email)
    {
        try
        {
            var m = new MailAddress(email);
            return true;
        }
        catch (FormatException ex)
        {
            return false;
        }
    }

    public static bool IsValidPhoneNumber(this string phone)
    {
        try
        {
            return Regex.IsMatch(phone, PhoneRegex);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }
}
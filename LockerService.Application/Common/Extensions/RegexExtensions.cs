using System.Net.Mail;
using System.Text.RegularExpressions;

namespace LockerService.Application.Common.Extensions;

public static class RegexExtensions
{
    
    private static readonly string PhoneRegex = "^(\\+84|84|0)[35789][0-9]{8}$";
    
    private const string MacRegex = "^([0-9A-Fa-f]{2}[:]){5}([0-9A-Fa-f]{2})$";

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
    
    public static bool IsValidMacAddress(this string macAddress)
    {
        return !string.IsNullOrWhiteSpace(macAddress) && Regex.IsMatch(macAddress, MacRegex);
    }
}
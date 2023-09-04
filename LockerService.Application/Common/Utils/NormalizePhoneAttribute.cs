namespace LockerService.Application.Common.Utils;

/**
 * Normalize phone number to string starts with "0"
 * return null if it is not a phone number
 */
[AttributeUsage(AttributeTargets.Property)]
public class NormalizePhoneAttribute : JsonConverterAttribute
{
    private readonly bool _nullIfEmpty;

    public NormalizePhoneAttribute() => _nullIfEmpty = false;
    
    public NormalizePhoneAttribute(bool nullIfEmpty) => _nullIfEmpty = nullIfEmpty; 
    
    public override JsonConverter? CreateConverter(Type typeToConvert)
    {
        if (typeToConvert != typeof(string))
        {
            throw new ArgumentException($"This converter only work with string, and it was provided {typeToConvert.Name}.");
        }

        return new NormalizePhoneConverter(_nullIfEmpty);
    }
}
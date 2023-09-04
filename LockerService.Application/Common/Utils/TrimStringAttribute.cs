namespace LockerService.Application.Common.Utils;

[AttributeUsage(AttributeTargets.Property)]
public class TrimStringAttribute : JsonConverterAttribute
{
    private readonly bool _nullIfEmpty;

    public TrimStringAttribute() => _nullIfEmpty = false;
    
    public TrimStringAttribute(bool nullIfEmpty) => _nullIfEmpty = nullIfEmpty; 
    
    public override JsonConverter? CreateConverter(Type typeToConvert)
    {
        if (typeToConvert != typeof(string))
        {
            throw new ArgumentException($"This converter only work with string, and it was provided {typeToConvert.Name}.");
        }

        return new TrimStringConverter(_nullIfEmpty);
    }
}
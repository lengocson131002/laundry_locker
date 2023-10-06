using System.ComponentModel;

namespace LockerService.Shared.Extensions;

public static class EnumsExtensions
{
    public static string GetDescription<T>(this T enumValue) where T : struct
    {
        var type = enumValue.GetType();
        if (!type.IsEnum) throw new ArgumentException($"{nameof(enumValue)} is not Enum type");

        var memberInfos = type.GetMember(enumValue.ToString()!);
        if (memberInfos.Length > 0)
        {
            var attributes = memberInfos[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0) return ((DescriptionAttribute)attributes[0]).Description;
        }

        return enumValue.ToString() ?? string.Empty;
    }
}
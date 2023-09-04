using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace LockerService.Domain.Entities;

[Table("Address")]
public class Address
{
    [Key] 
    public long Id { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? ParentCode { get; set; }

    public override string ToString()
    {
        if (Name == null)
        {
            return string.Empty;
        }

        var regex = new Regex("^(Xã|Phường|Thị xã|Thị trấn|Huyện|Quận|Tỉnh|Thành phố)", RegexOptions.IgnoreCase);
        return regex.Replace(Name, "");
    }
}
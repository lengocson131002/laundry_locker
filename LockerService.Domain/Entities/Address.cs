using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LockerService.Domain.Entities;

[Table("Address")]
public class Address
{
    [Key] 
    public long Id { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? ParentCode { get; set; }
}
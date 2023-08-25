using System.ComponentModel;

namespace LockerService.Domain.Enums;

public enum OrderType
{
    [Description("Giặt sấy")] 
    Laundry,
    
    [Description("Giữ đồ")] 
    Storage
}
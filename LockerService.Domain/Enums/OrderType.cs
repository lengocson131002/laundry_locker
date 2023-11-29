using System.ComponentModel;

namespace LockerService.Domain.Enums;

public enum OrderType
{
    [Description("Giặt sấy")] 
    Laundry = 0,
    
    [Description("Giữ đồ")] 
    Storage = 1
}
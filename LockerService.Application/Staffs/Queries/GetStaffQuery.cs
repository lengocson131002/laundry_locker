namespace LockerService.Application.Staffs.Queries;

public class GetStaffQuery : IRequest<AccountDetailResponse>
{
    public int Id { get; init; }
    public int StoreId { get; init; }
}
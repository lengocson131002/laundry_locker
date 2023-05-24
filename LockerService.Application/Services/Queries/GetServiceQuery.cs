namespace LockerService.Application.Services.Queries;

public class GetServiceQuery : IRequest<ServiceDetailResponse>
{
    public int LockerId { get; set; }
    public int ServiceId { get; set; }
}
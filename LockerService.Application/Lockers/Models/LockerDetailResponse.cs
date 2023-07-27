namespace LockerService.Application.Lockers.Models;

public class LockerDetailResponse : LockerResponse
{
   public IList<BoxResponse> Boxes { get; set; } = new List<BoxResponse>();
}
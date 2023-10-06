namespace LockerService.Application.Common.Models.Response;

public class ListResponse<TResponse> : List<TResponse> where TResponse : class
{
    public ListResponse()
    {
    }

    public ListResponse(IList<TResponse> items)
    {
        AddRange(items);
    }
}
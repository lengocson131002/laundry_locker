namespace LockerService.Application.Hardwares.Queries;

public class GetAllHardwareQuery : PaginationRequest<Hardware>, IRequest<PaginationResponse<Hardware, HardwareResponse>>
{
    [BindNever] 
    public int? LockerId { get; set; }
        
    public string? Query { get; set; }
    
    public bool? IsActive { get; set; }

    public override Expression<Func<Hardware, bool>> GetExpressions()
    {
        if (LockerId != null)
        {
            Expression = Expression.And(hardware => hardware.LockerId == LockerId);
        }

        if (!string.IsNullOrWhiteSpace(Query))
        {
            Expression = Expression.And(hardware => hardware.Name.ToLower().Contains(Query.ToLower().Trim()));
        }

        if (IsActive != null)
        {
            Expression = Expression.And(hardware => IsActive == true ? hardware.DeletedAt == null : hardware.DeletedAt != null);
        }
        
        return Expression;
    }
}
using LockerService.Application.Audits.Models;

namespace LockerService.Application.Audits.Queries;

public class GetAuditsQuery : PaginationRequest<Audit>, IRequest<PaginationResponse<Audit, AuditResponse>>
{
    public string? Search { get; set; }

    public DateTimeOffset? From { get; set; }
    
    public DateTimeOffset? To { get; set; }
    
    public override Expression<Func<Audit, bool>> GetExpressions()
    {
        if (!string.IsNullOrWhiteSpace(Search))
        {
            Search = Search.Trim().ToLower();
            Expression = Expression.And(audit =>
                audit.Type.ToLower().Contains(Search) 
                || audit.TableName.ToLower().Contains(Search) 
                || audit.Type.ToLower().Contains(Search) 
                || (audit.CreatedByUsername != null && audit.CreatedByUsername.ToLower().Contains(Search)));
        }

        if (From != null)
        {
            Expression = Expression.And(audit => audit.CreatedAt >= From);
        }

        if (To != null)
        {
            Expression = Expression.And(audit => audit.CreatedAt <= To);
        }
        
        return Expression;

    }
}
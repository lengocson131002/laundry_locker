using LockerService.Application.Settings.Models;

namespace LockerService.Application.Settings.Queries;

public class GetAllSettingsQuery : FilterRequest<Setting>, IRequest<ListResponse<SettingResponse>>
{
    public string? Search { get; set; }
    
    public SettingGroup? Group { get; set; }

    public override Expression<Func<Setting, bool>> GetExpressions()
    {
        Expression = Expression.And(setting => Group == null || Equals(Group, setting.Group));

        if (!string.IsNullOrWhiteSpace(Search))
        {
            Search = Search.Trim().ToLower();
            var queryExpression =  PredicateBuilder.New<Setting>(true);
            queryExpression.Or(setting => setting.Key.ToLower().Contains(Search));
            queryExpression.Or(setting => setting.Name.ToLower().Contains(Search));
            Expression = Expression.And(queryExpression);
        }

        throw new NotImplementedException();
    }
}
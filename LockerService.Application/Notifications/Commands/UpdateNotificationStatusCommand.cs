using LockerService.Application.Notifications.Models;

namespace LockerService.Application.Notifications.Commands;

public class UpdateNotificationStatusCommandValidator : AbstractValidator<UpdateNotificationStatusCommand> {
    
    public UpdateNotificationStatusCommandValidator()
    {
        RuleFor(model => model.IsRead)
            .NotNull();
    }
}
public class UpdateNotificationStatusCommand : IRequest<NotificationModel>
{
    [JsonIgnore]
    public long Id { get; set; }
    
    public bool? IsRead { get; set; }
}
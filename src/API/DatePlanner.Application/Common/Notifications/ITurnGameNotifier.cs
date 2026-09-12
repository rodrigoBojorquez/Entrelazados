namespace DatePlanner.Application.Common.Notifications;

public interface ITurnGameNotifier
{
    Task NotifyGameScheduledAsync(
        Guid turnGameId,
        Guid partnerOneId,
        Guid partnerTwoId,
        CancellationToken cancellationToken);
}

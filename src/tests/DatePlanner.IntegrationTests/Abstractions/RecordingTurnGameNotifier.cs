using System.Collections.Concurrent;
using DatePlanner.Application.Common.Notifications;

namespace DatePlanner.IntegrationTests.Abstractions;

public sealed class RecordingTurnGameNotifier : ITurnGameNotifier
{
    private readonly ConcurrentQueue<ScheduledGameNotification> _notifications = [];

    public IReadOnlyCollection<ScheduledGameNotification> Notifications =>
        _notifications.ToArray();

    public Task NotifyGameScheduledAsync(
        Guid turnGameId,
        Guid partnerOneId,
        Guid partnerTwoId,
        CancellationToken cancellationToken)
    {
        _notifications.Enqueue(new ScheduledGameNotification(
            turnGameId,
            partnerOneId,
            partnerTwoId));

        return Task.CompletedTask;
    }
}

public sealed record ScheduledGameNotification(
    Guid TurnGameId,
    Guid PartnerOneId,
    Guid PartnerTwoId);

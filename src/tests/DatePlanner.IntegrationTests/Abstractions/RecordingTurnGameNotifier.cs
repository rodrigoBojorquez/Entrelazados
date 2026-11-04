using System.Collections.Concurrent;
using DatePlanner.Application.Common.Notifications;
using Npgsql;

namespace DatePlanner.IntegrationTests.Abstractions;

public sealed class RecordingTurnGameNotifier(NpgsqlDataSource dataSource) : ITurnGameNotifier
{
    private readonly ConcurrentQueue<ScheduledGameNotification> _notifications = [];

    public IReadOnlyCollection<ScheduledGameNotification> Notifications =>
        _notifications.ToArray();

    public async Task NotifyGameScheduledAsync(
        Guid turnGameId,
        Guid partnerOneId,
        Guid partnerTwoId,
        CancellationToken cancellationToken)
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using NpgsqlCommand command = new(
            "SELECT EXISTS (SELECT 1 FROM dateplanner.turn_games WHERE id = @id)", connection);
        command.Parameters.AddWithValue("id", turnGameId);
        bool wasCommitted = (bool)(await command.ExecuteScalarAsync(cancellationToken))!;

        _notifications.Enqueue(new ScheduledGameNotification(
            turnGameId,
            partnerOneId,
            partnerTwoId,
            wasCommitted));
    }
}

public sealed record ScheduledGameNotification(
    Guid TurnGameId,
    Guid PartnerOneId,
    Guid PartnerTwoId,
    bool WasCommitted);

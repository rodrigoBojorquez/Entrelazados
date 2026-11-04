using DatePlanner.Application.Common.Data;
using DatePlanner.Application.Common.Events;
using DatePlanner.Application.Common.Notifications;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Authentication;
using DatePlanner.Domain.Couples;
using DatePlanner.Domain.Dates;
using DatePlanner.Domain.TurnGames;
using DatePlanner.Domain.TurnGames.Rounds;
using DatePlanner.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DatingPreferencesEntity = DatePlanner.Domain.Users.DatingPreferences;

namespace DatePlanner.Infrastructure.Database;

public class DatePlannerDbContext(
    DbContextOptions<DatePlannerDbContext> options,
    IDomainEventsDispatcher domainEventsDispatcher,
    ITurnGameNotifier turnGameNotifier,
    ILogger<DatePlannerDbContext> logger) : DbContext(options), IUnitOfWork
{
    public DbSet<User> Users => Set<User>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<DatingPreferencesEntity> DatingPreferences => Set<DatingPreferencesEntity>();

    public DbSet<Couple> Couples => Set<Couple>();

    public DbSet<DatePlan> DatePlans => Set<DatePlan>();

    public DbSet<ItineraryItem> ItineraryItems => Set<ItineraryItem>();

    public DbSet<TurnGame> TurnGames => Set<TurnGame>();

    public DbSet<TurnGameRound> TurnGameRounds => Set<TurnGameRound>();

    public DbSet<TurnGameMove> TurnGameMoves => Set<TurnGameMove>();

    public override int SaveChanges(bool acceptAllChangesOnSuccess) =>
        SaveChangesAsync(acceptAllChangesOnSuccess).GetAwaiter().GetResult();

    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        await DispatchDomainEventsAsync(cancellationToken);

        TurnGame[] scheduledGames = ChangeTracker.Entries<TurnGame>()
            .Where(entry => entry.State == EntityState.Added &&
                            entry.Entity.Purpose == TurnGamePurpose.PlanningDecision)
            .Select(entry => entry.Entity)
            .ToArray();

        int saved = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

        foreach (TurnGame game in scheduledGames)
        {
            try
            {
                await turnGameNotifier.NotifyGameScheduledAsync(
                    game.Id, game.PlayerOneId, game.PlayerTwoId, cancellationToken);
            }
            catch (Exception exception)
            {
                // shortcut: notifications are best effort; add durable delivery if clients need replay.
                logger.LogError(exception, "Could not notify players of scheduled game {TurnGameId}", game.Id);
            }
        }

        return saved;
    }

    public async Task<bool> TrySaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            ChangeTracker.Clear();
            return false;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(Schemas.DefaultSchema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatePlannerDbContext).Assembly);
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        while (GetDomainEvents() is { Count: > 0 } domainEvents)
        {
            await domainEventsDispatcher.DispatchAsync(domainEvents, cancellationToken);
        }
    }

    private IReadOnlyCollection<IDomainEvent> GetDomainEvents()
    {
        Entity[] entities = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .Where(entity => entity.DomainEvents.Count > 0)
            .ToArray();
        IDomainEvent[] domainEvents = entities
            .SelectMany(entity => entity.DomainEvents)
            .ToArray();

        foreach (Entity entity in entities)
        {
            entity.ClearDomainEvents();
        }

        return domainEvents;
    }
}

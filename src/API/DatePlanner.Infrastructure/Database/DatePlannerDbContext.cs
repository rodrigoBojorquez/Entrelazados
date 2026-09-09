using DatePlanner.Application.Common.Data;
using DatePlanner.Application.Common.Events;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Authentication;
using DatePlanner.Domain.Couples;
using DatePlanner.Domain.Dates;
using DatePlanner.Domain.TurnGames;
using DatePlanner.Domain.TurnGames.Rounds;
using DatePlanner.Domain.Users;
using DatePlanner.Infrastructure.DateIdeas;
using Microsoft.EntityFrameworkCore;
using DatingPreferencesEntity = DatePlanner.Domain.Users.DatingPreferences;

namespace DatePlanner.Infrastructure.Database;

public class DatePlannerDbContext(
    DbContextOptions<DatePlannerDbContext> options,
    IDomainEventsDispatcher domainEventsDispatcher) : DbContext(options), IUnitOfWork
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

    public DbSet<DateIdea> DateIdeas => Set<DateIdea>();

    public DbSet<DateIdeaItineraryItem> DateIdeaItineraryItems => Set<DateIdeaItineraryItem>();

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        DispatchDomainEventsAsync(CancellationToken.None).GetAwaiter().GetResult();

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        await DispatchDomainEventsAsync(cancellationToken);

        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
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

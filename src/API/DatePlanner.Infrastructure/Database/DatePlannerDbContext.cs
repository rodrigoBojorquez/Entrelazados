using DatePlanner.Application.Common.Data;
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

public class DatePlannerDbContext(DbContextOptions<DatePlannerDbContext> options) : DbContext(options), IUnitOfWork
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(Schemas.DefaultSchema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatePlannerDbContext).Assembly);
    }
}

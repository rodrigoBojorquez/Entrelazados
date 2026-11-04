using DatePlanner.Application.Common.Notifications;
using DatePlanner.Application.Couples.BeginRelationship;
using DatePlanner.Domain.Couples;
using DatePlanner.Domain.TurnGames;
using DatePlanner.Infrastructure.Database;
using DatePlanner.IntegrationTests.Abstractions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DatePlanner.IntegrationTests.Database;

public sealed class DatePlannerDbContextTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    private readonly IntegrationTestWebAppFactory _factory = factory;

    [Fact]
    public async Task SaveChanges_ShouldNotifyOnlyAfterCommit_AndNotRepeatOnNextSave()
    {
        RegisteredCouple couple = await CreateCoupleAsync();
        RecordingTurnGameNotifier notifier = Services.GetRequiredService<RecordingTurnGameNotifier>();
        ScheduledGameNotification notification = notifier.Notifications.Should()
            .ContainSingle(item => item.PartnerOneId == couple.PartnerOne.Id).Subject;

        notification.WasCommitted.Should().BeTrue();

        await Services.GetRequiredService<DatePlannerDbContext>().SaveChangesAsync();

        notifier.Notifications.Should().ContainSingle(item => item.TurnGameId == notification.TurnGameId);
    }

    [Fact]
    public async Task SaveChanges_ShouldNotNotify_WhenTheTransactionFails()
    {
        DatePlannerDbContext dbContext = Services.GetRequiredService<DatePlannerDbContext>();
        TurnGame game = TurnGame.Schedule(Couple.Create(Guid.NewGuid(), Guid.NewGuid()).Value).Value;
        dbContext.TurnGames.Add(game);

        Func<Task> save = () => dbContext.SaveChangesAsync();

        await save.Should().ThrowAsync<DbUpdateException>();
        Services.GetRequiredService<RecordingTurnGameNotifier>().Notifications
            .Should().NotContain(item => item.TurnGameId == game.Id);
    }

    [Fact]
    public async Task SaveChanges_ShouldStillSucceed_WhenTheNotificationFails()
    {
        RegisteredUser partnerOne = await RegisterUserAsync();
        RegisteredUser partnerTwo = await RegisterUserAsync();
        await using var app = _factory.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
        {
            services.RemoveAll<ITurnGameNotifier>();
            services.AddSingleton<ITurnGameNotifier, FailingTurnGameNotifier>();
        }));
        using IServiceScope scope = app.Services.CreateScope();

        var result = await scope.ServiceProvider.GetRequiredService<MediatR.ISender>().Send(
            new BeginRelationshipCommand(partnerOne.Id, partnerTwo.PartnerCode));

        result.IsSuccess.Should().BeTrue();
        DatePlannerDbContext dbContext = scope.ServiceProvider.GetRequiredService<DatePlannerDbContext>();
        (await dbContext.TurnGames.AsNoTracking().SingleAsync(game => game.PlayerOneId == partnerOne.Id))
            .Status.Should().Be(TurnGameStatus.AwaitingSelection);
    }

    [Fact]
    public async Task RetireDateIdeas_ShouldPreserveExistingIdeasAndItinerary()
    {
        RegisteredCouple couple = await CreateCoupleAsync();
        DatePlannerDbContext dbContext = Services.GetRequiredService<DatePlannerDbContext>();
        IMigrator migrator = dbContext.GetService<IMigrator>();
        await migrator.MigrateAsync("20260911232314_AddCoupleWorkflow");
        Guid ideaId = Guid.NewGuid();
        Guid itemId = Guid.NewGuid();
        await dbContext.Database.ExecuteSqlInterpolatedAsync($"""
            INSERT INTO dateplanner.date_ideas (id, couple_id, preferred_budget, city, created_at_utc)
            VALUES ({ideaId}, {couple.Id}, 100, 'Cancun', now());
            INSERT INTO dateplanner.date_idea_itinerary_items (id, date_idea_id, "order", name, tags)
            VALUES ({itemId}, {ideaId}, 1, 'Dinner', ARRAY[]::text[]);
            """);

        await migrator.MigrateAsync();

        int remaining = await dbContext.Database.SqlQuery<int>($"""
            SELECT count(*)::int AS "Value"
            FROM dateplanner.date_ideas idea
            JOIN dateplanner.date_idea_itinerary_items item ON item.date_idea_id = idea.id
            WHERE idea.id = {ideaId} AND item.id = {itemId}
            """).SingleAsync();
        remaining.Should().Be(1);
        dbContext.Database.HasPendingModelChanges().Should().BeFalse();
        dbContext.Model.GetEntityTypes().Should()
            .NotContain(entity => entity.Name.Contains("DateIdea"));
    }

    private sealed class FailingTurnGameNotifier : ITurnGameNotifier
    {
        public Task NotifyGameScheduledAsync(
            Guid turnGameId, Guid partnerOneId, Guid partnerTwoId, CancellationToken cancellationToken) =>
            throw new InvalidOperationException("Notification unavailable");
    }
}

using DatePlanner.Application.Dates.AddItineraryItem;
using DatePlanner.Application.Dates.AttendDatePlan;
using DatePlanner.Application.Dates.CancelDatePlan;
using DatePlanner.Application.Dates.CreateDatePlan;
using DatePlanner.Application.Dates.GetDatePlan;
using DatePlanner.Application.Dates.ListDatePlans;
using DatePlanner.Application.Dates.RemoveItineraryItem;
using DatePlanner.Application.Dates.ReturnDatePlanToPlanning;
using DatePlanner.Application.Dates.ScheduleDatePlan;
using DatePlanner.Application.Dates.SetCollaborativeEditing;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Dates;
using DatePlanner.Infrastructure.DatePlans;
using DatePlanner.IntegrationTests.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace DatePlanner.IntegrationTests.Dates;

public sealed class DatePlanTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task DatePlan_ShouldCompletePlanningSchedulingAndAttendanceWorkflow()
    {
        RegisteredCouple couple = await CreateCoupleAsync();

        Result<Guid> creationResult = await Sender.Send(
            new CreateDatePlanCommand(couple.PartnerOne.Id));

        creationResult.IsSuccess.Should().BeTrue();

        Result addResult = await Sender.Send(new AddItineraryItemCommand(
            couple.PartnerOne.Id,
            creationResult.Value,
            "Dinner by the sea",
            21.1619m,
            -86.8515m,
            new TimeOnly(19, 30)));

        DateOnly scheduledFor = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
        Result scheduleResult = await Sender.Send(new ScheduleDatePlanCommand(
            couple.PartnerOne.Id,
            creationResult.Value,
            scheduledFor));
        Result attendResult = await Sender.Send(new AttendDatePlanCommand(
            couple.PartnerTwo.Id,
            creationResult.Value));

        Result<DatePlanResponse> queryResult = await Sender.Send(
            new GetDatePlanQuery(couple.PartnerOne.Id, creationResult.Value));

        addResult.IsSuccess.Should().BeTrue();
        scheduleResult.IsSuccess.Should().BeTrue();
        attendResult.IsSuccess.Should().BeTrue();
        queryResult.IsSuccess.Should().BeTrue();
        queryResult.Value.CoupleId.Should().Be(couple.Id);
        queryResult.Value.PartnerWhoPlannedId.Should().Be(couple.PartnerOne.Id);
        queryResult.Value.IsCollaborative.Should().BeFalse();
        queryResult.Value.ScheduledFor.Should().Be(scheduledFor);
        queryResult.Value.Status.Should().Be(DateStatus.Attended);

        ItineraryItemResponse item = queryResult.Value.Itinerary.Should().ContainSingle().Subject;
        item.Order.Should().Be(1);
        item.Name.Should().Be("Dinner by the sea");
        item.Latitude.Should().Be(21.1619m);
        item.Longitude.Should().Be(-86.8515m);
        item.StartsAt.Should().Be(new TimeOnly(19, 30));
    }

    [Fact]
    public async Task RemoveItineraryItem_ShouldPersistRemovalAndKeepPlanInPlanning()
    {
        RegisteredCouple couple = await CreateCoupleAsync();
        Result<Guid> creationResult = await Sender.Send(
            new CreateDatePlanCommand(couple.PartnerOne.Id));
        await Sender.Send(new AddItineraryItemCommand(
            couple.PartnerOne.Id,
            creationResult.Value,
            "Museum",
            null,
            null,
            new TimeOnly(16, 0)));

        Result<DatePlanResponse> beforeRemoval = await Sender.Send(
            new GetDatePlanQuery(couple.PartnerTwo.Id, creationResult.Value));
        Guid itemId = beforeRemoval.Value.Itinerary.Single().Id;

        Result removalResult = await Sender.Send(new RemoveItineraryItemCommand(
            couple.PartnerOne.Id,
            creationResult.Value,
            itemId));
        Result<DatePlanResponse> afterRemoval = await Sender.Send(
            new GetDatePlanQuery(couple.PartnerOne.Id, creationResult.Value));

        removalResult.IsSuccess.Should().BeTrue();
        afterRemoval.Value.Status.Should().Be(DateStatus.Planning);
        afterRemoval.Value.Itinerary.Should().BeEmpty();
    }

    [Fact]
    public async Task ReturnDatePlanToPlanning_ShouldPersistEditableState()
    {
        RegisteredCouple couple = await CreateCoupleAsync();
        Result<Guid> creationResult = await Sender.Send(
            new CreateDatePlanCommand(couple.PartnerOne.Id));
        await Sender.Send(new AddItineraryItemCommand(
            couple.PartnerOne.Id,
            creationResult.Value,
            "Museum",
            null,
            null,
            null));
        await Sender.Send(new ScheduleDatePlanCommand(
            couple.PartnerOne.Id,
            creationResult.Value,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7))));

        Result returnResult = await Sender.Send(
            new ReturnDatePlanToPlanningCommand(
                couple.PartnerOne.Id,
                creationResult.Value));
        Result<DatePlanResponse> queryResult = await Sender.Send(
            new GetDatePlanQuery(couple.PartnerOne.Id, creationResult.Value));

        returnResult.IsSuccess.Should().BeTrue();
        queryResult.Value.Status.Should().Be(DateStatus.Planning);
        queryResult.Value.ScheduledFor.Should().BeNull();
        queryResult.Value.Itinerary.Should().ContainSingle();
    }

    [Fact]
    public async Task CancelDatePlan_ShouldPersistCanceledStatus()
    {
        RegisteredCouple couple = await CreateCoupleAsync();
        Result<Guid> creationResult = await Sender.Send(
            new CreateDatePlanCommand(couple.PartnerOne.Id));

        Result cancelResult = await Sender.Send(new CancelDatePlanCommand(
            couple.PartnerOne.Id,
            creationResult.Value));
        Result<DatePlanResponse> queryResult = await Sender.Send(
            new GetDatePlanQuery(couple.PartnerOne.Id, creationResult.Value));

        cancelResult.IsSuccess.Should().BeTrue();
        queryResult.Value.Status.Should().Be(DateStatus.Canceled);
    }

    [Fact]
    public async Task AddItineraryItem_ShouldHidePlanFromUserOutsideCouple()
    {
        RegisteredCouple couple = await CreateCoupleAsync();
        RegisteredUser outsider = await RegisterUserAsync();
        Result<Guid> creationResult = await Sender.Send(
            new CreateDatePlanCommand(couple.PartnerOne.Id));

        Result addResult = await Sender.Send(new AddItineraryItemCommand(
            outsider.Id,
            creationResult.Value,
            "Private plan",
            null,
            null,
            null));
        Result<DatePlanResponse> queryResult = await Sender.Send(
            new GetDatePlanQuery(outsider.Id, creationResult.Value));

        addResult.IsFailure.Should().BeTrue();
        addResult.Error.Code.Should().Be("DatePlan.NotFound");
        queryResult.IsFailure.Should().BeTrue();
        queryResult.Error.Code.Should().Be("DatePlan.NotFound");
    }

    [Fact]
    public async Task DatePlan_ShouldRequirePlannerUnlessCollaborationIsEnabled()
    {
        RegisteredCouple couple = await CreateCoupleAsync();
        Result<Guid> creationResult = await Sender.Send(
            new CreateDatePlanCommand(couple.PartnerOne.Id));

        Result privateEdit = await Sender.Send(new AddItineraryItemCommand(
            couple.PartnerTwo.Id,
            creationResult.Value,
            "Secret dinner",
            null,
            null,
            null));
        Result enableResult = await Sender.Send(new SetCollaborativeEditingCommand(
            couple.PartnerOne.Id,
            creationResult.Value,
            true));
        Result collaborativeEdit = await Sender.Send(new AddItineraryItemCommand(
            couple.PartnerTwo.Id,
            creationResult.Value,
            "Shared dinner",
            null,
            null,
            null));

        privateEdit.IsFailure.Should().BeTrue();
        privateEdit.Error.Code.Should().Be("DatePlan.OnlyPlannerCanEdit");
        enableResult.IsSuccess.Should().BeTrue();
        collaborativeEdit.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ListDatePlans_ShouldReturnCurrentAndHistoricalPlans()
    {
        RegisteredCouple couple = await CreateCoupleAsync();
        Result<Guid> creationResult = await Sender.Send(
            new CreateDatePlanCommand(couple.PartnerOne.Id));

        await Sender.Send(new AddItineraryItemCommand(
            couple.PartnerOne.Id,
            creationResult.Value,
            "Picnic al atardecer",
            null,
            null,
            null));

        Result<IReadOnlyCollection<DatePlanSummaryResponse>> result =
            await Sender.Send(new ListDatePlansQuery(couple.PartnerTwo.Id));

        result.IsSuccess.Should().BeTrue();
        DatePlanSummaryResponse plan = result.Value
            .Should().ContainSingle(item => item.Id == creationResult.Value).Subject;
        plan.Title.Should().Be("Picnic al atardecer");
        plan.ItineraryItemCount.Should().Be(1);
        plan.PlannerName.Should().Be(
            $"{couple.PartnerOne.FirstName} {couple.PartnerOne.LastName}");
    }

    [Fact]
    public async Task AttendanceProcessor_ShouldAttendDatesFromPreviousDays()
    {
        RegisteredCouple couple = await CreateCoupleAsync();
        Result<Guid> creationResult = await Sender.Send(
            new CreateDatePlanCommand(couple.PartnerOne.Id));
        await Sender.Send(new AddItineraryItemCommand(
            couple.PartnerOne.Id,
            creationResult.Value,
            "Breakfast",
            null,
            null,
            null));
        await Sender.Send(new ScheduleDatePlanCommand(
            couple.PartnerOne.Id,
            creationResult.Value,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1))));

        DatePlanAttendanceProcessor processor =
            Services.GetRequiredService<DatePlanAttendanceProcessor>();
        int processed = await processor.ProcessAsync(CancellationToken.None);
        Result<DatePlanResponse> queryResult = await Sender.Send(
            new GetDatePlanQuery(couple.PartnerOne.Id, creationResult.Value));

        processed.Should().BeGreaterThanOrEqualTo(1);
        queryResult.Value.Status.Should().Be(DateStatus.Attended);
    }
}

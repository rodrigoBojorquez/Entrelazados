using DatePlanner.UnitTests.Abstractions;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Dates;
using DatePlanner.Domain.Dates.Events;
using FluentAssertions;

namespace DatePlanner.UnitTests.Dates;

public class DateTests : BaseTest
{
    private readonly Guid _coupleId = Guid.NewGuid();
    private readonly Guid _plannerId = Guid.NewGuid();

    [Fact]
    public void Create_ShouldCreateEmptyPlan_InPlanningState()
    {
        Result<DatePlan> result = DatePlan.Create(_coupleId, _plannerId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().NotBeEmpty();
        result.Value.CoupleId.Should().Be(_coupleId);
        result.Value.PartnerWhoPlannedId.Should().Be(_plannerId);
        result.Value.Status.Should().Be(DateStatus.Planning);
        result.Value.IsCollaborative.Should().BeFalse();
        result.Value.CreatedAtUtc.Should().NotBe(default);
        result.Value.ScheduledFor.Should().BeNull();
        result.Value.Itinerary.Should().BeEmpty();
    }

    [Fact]
    public void SetCollaborativeEditing_ShouldOptInWhilePlanning()
    {
        DatePlan plan = CreatePlan();

        Result result = plan.SetCollaborativeEditing(true);

        result.IsSuccess.Should().BeTrue();
        plan.IsCollaborative.Should().BeTrue();
    }

    [Fact]
    public void AddItineraryItem_ShouldTrimName_AndAppendWithSequentialOrder()
    {
        DatePlan plan = CreatePlan();
        Location location = Location.Create(21.1619m, -86.8515m).Value;
        TimeOnly startsAt = new(19, 30);

        Result firstResult = plan.AddItineraryItem("  Dinner  ", location, startsAt);
        Result secondResult = plan.AddItineraryItem("Walk", null, null);

        firstResult.IsSuccess.Should().BeTrue();
        secondResult.IsSuccess.Should().BeTrue();
        plan.Itinerary.Should().HaveCount(2);
        plan.Itinerary.Should().SatisfyRespectively(
            first =>
            {
                first.Id.Should().NotBeEmpty();
                first.Order.Should().Be(1);
                first.Name.Should().Be("Dinner");
                first.Location.Should().Be(location);
                first.StartsAt.Should().Be(startsAt);
            },
            second =>
            {
                second.Order.Should().Be(2);
                second.Name.Should().Be("Walk");
                second.Location.Should().BeNull();
                second.StartsAt.Should().BeNull();
            });
    }

    [Fact]
    public void AddItineraryItem_ShouldUseHighestExistingOrder_WhenAppendingThirdItem()
    {
        DatePlan plan = CreatePlan();
        plan.AddItineraryItem("First", null, null);
        plan.AddItineraryItem("Second", null, null);

        Result result = plan.AddItineraryItem("Third", null, null);

        result.IsSuccess.Should().BeTrue();
        plan.Itinerary.Select(x => x.Order).Should().Equal(1, 2, 3);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void AddItineraryItem_ShouldRejectBlankName_WithoutChangingItinerary(string name)
    {
        DatePlan plan = CreatePlan();

        Result result = plan.AddItineraryItem(name, null, null);

        result.Error.ShouldHaveCode("DatePlan.ItineraryItemNameIsRequired", ErrorType.Problem);
        plan.Itinerary.Should().BeEmpty();
    }

    [Fact]
    public void AddItineraryItem_ShouldFailWithoutChangingPlan_WhenNotPlanning()
    {
        DatePlan plan = CreateBookedPlan();
        int originalCount = plan.Itinerary.Count;

        Result result = plan.AddItineraryItem("Late addition", null, null);

        result.Error.ShouldHaveCode("DatePlan.MustBePlanning", ErrorType.Conflict);
        plan.Itinerary.Should().HaveCount(originalCount);
        plan.Status.Should().Be(DateStatus.Booked);
    }

    [Fact]
    public void RemoveItineraryItem_ShouldRemoveOnlyRequestedItem_WhenPlanning()
    {
        DatePlan plan = CreatePlan();
        plan.AddItineraryItem("Dinner", null, null);
        plan.AddItineraryItem("Walk", null, null);
        Guid itemToRemove = plan.Itinerary.First().Id;

        Result result = plan.RemoveItineraryItem(itemToRemove);

        result.IsSuccess.Should().BeTrue();
        plan.Itinerary.Should().ContainSingle().Which.Name.Should().Be("Walk");
    }

    [Fact]
    public void RemoveItineraryItem_ShouldRejectEmptyId_WithoutChangingItinerary()
    {
        DatePlan plan = CreatePlan();
        plan.AddItineraryItem("Dinner", null, null);

        Result result = plan.RemoveItineraryItem(Guid.Empty);

        result.Error.ShouldHaveCode("DatePlan.ItineraryItemIdIsRequired", ErrorType.Problem);
        plan.Itinerary.Should().ContainSingle();
    }

    [Fact]
    public void RemoveItineraryItem_ShouldReturnNotFound_WithoutChangingItinerary_WhenItemDoesNotExist()
    {
        DatePlan plan = CreatePlan();
        plan.AddItineraryItem("Dinner", null, null);
        Guid missingId = Guid.NewGuid();

        Result result = plan.RemoveItineraryItem(missingId);

        result.Error.ShouldHaveCode("DatePlan.ItineraryItemNotFound", ErrorType.NotFound);
        plan.Itinerary.Should().ContainSingle();
    }

    [Fact]
    public void RemoveItineraryItem_ShouldFailWithoutChangingItinerary_WhenNotPlanning()
    {
        DatePlan plan = CreateBookedPlan();
        Guid existingId = plan.Itinerary.Single().Id;

        Result result = plan.RemoveItineraryItem(existingId);

        result.Error.ShouldHaveCode("DatePlan.MustBePlanning", ErrorType.Conflict);
        plan.Itinerary.Should().ContainSingle();
    }

    [Fact]
    public void Schedule_ShouldBookPlanForSpecifiedDate_WhenItineraryExists()
    {
        DatePlan plan = CreatePlan();
        plan.AddItineraryItem("Dinner", null, null);
        DateOnly scheduledFor = new(2026, 9, 20);

        Result result = plan.Schedule(scheduledFor);

        result.IsSuccess.Should().BeTrue();
        plan.Status.Should().Be(DateStatus.Booked);
        plan.ScheduledFor.Should().Be(scheduledFor);
    }

    [Fact]
    public void Schedule_ShouldRejectDefaultDate_WithoutChangingState()
    {
        DatePlan plan = CreatePlan();
        plan.AddItineraryItem("Dinner", null, null);

        Result result = plan.Schedule(default);

        result.Error.ShouldHaveCode("DatePlan.ScheduledDateIsRequired", ErrorType.Problem);
        plan.Status.Should().Be(DateStatus.Planning);
        plan.ScheduledFor.Should().BeNull();
    }

    [Fact]
    public void Schedule_ShouldRejectEmptyItinerary_WithoutChangingState()
    {
        DatePlan plan = CreatePlan();

        Result result = plan.Schedule(new DateOnly(2026, 9, 20));

        result.Error.ShouldHaveCode("DatePlan.EmptyItinerary", ErrorType.Conflict);
        plan.Status.Should().Be(DateStatus.Planning);
        plan.ScheduledFor.Should().BeNull();
    }

    [Fact]
    public void Schedule_ShouldRejectRepeatedScheduling_WithoutChangingBookedDate()
    {
        DatePlan plan = CreateBookedPlan();
        DateOnly originalDate = plan.ScheduledFor!.Value;

        Result result = plan.Schedule(originalDate.AddDays(1));

        result.Error.ShouldHaveCode("DatePlan.MustBePlanning", ErrorType.Conflict);
        plan.Status.Should().Be(DateStatus.Booked);
        plan.ScheduledFor.Should().Be(originalDate);
    }

    [Fact]
    public void ReturnToPlanning_ShouldReopenBookedPlan_AndClearScheduledDate()
    {
        DatePlan plan = CreateBookedPlan();

        Result result = plan.ReturnToPlanning();

        result.IsSuccess.Should().BeTrue();
        plan.Status.Should().Be(DateStatus.Planning);
        plan.ScheduledFor.Should().BeNull();
        plan.Itinerary.Should().ContainSingle();
    }

    [Fact]
    public void ReturnToPlanning_ShouldRejectPlanThatIsNotBooked()
    {
        DatePlan plan = CreatePlan();

        Result result = plan.ReturnToPlanning();

        result.Error.ShouldHaveCode(
            "DatePlan.MustBeBookedToReturnToPlanning",
            ErrorType.Conflict);
        plan.Status.Should().Be(DateStatus.Planning);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Cancel_ShouldCancelPlan_WhenPlanningOrBooked(bool bookFirst)
    {
        DatePlan plan = bookFirst ? CreateBookedPlan() : CreatePlan();

        Result result = plan.Cancel();

        result.IsSuccess.Should().BeTrue();
        plan.Status.Should().Be(DateStatus.Canceled);
    }

    [Fact]
    public void Cancel_ShouldRejectRepeatedCancellation_WithoutChangingState()
    {
        DatePlan plan = CreatePlan();
        plan.Cancel();

        Result result = plan.Cancel();

        result.Error.ShouldHaveCode("DatePlan.CannotCancel", ErrorType.Conflict);
        plan.Status.Should().Be(DateStatus.Canceled);
    }

    [Fact]
    public void Cancel_ShouldRejectAttendedPlan_WithoutChangingState()
    {
        DatePlan plan = CreateBookedPlan();
        plan.Attend();

        Result result = plan.Cancel();

        result.Error.ShouldHaveCode("DatePlan.CannotCancel", ErrorType.Conflict);
        plan.Status.Should().Be(DateStatus.Attended);
    }

    [Fact]
    public void Attend_ShouldMarkBookedPlanAsAttended()
    {
        DatePlan plan = CreateBookedPlan();

        Result result = plan.Attend();

        result.IsSuccess.Should().BeTrue();
        plan.Status.Should().Be(DateStatus.Attended);
        DatePlanAttendedDomainEvent domainEvent =
            AssertDomainEventWasPublished<DatePlanAttendedDomainEvent>(plan);
        domainEvent.DatePlanId.Should().Be(plan.Id);
        domainEvent.CoupleId.Should().Be(plan.CoupleId);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Attend_ShouldRejectPlanThatIsNotBooked_WithoutChangingState(bool cancelFirst)
    {
        DatePlan plan = CreatePlan();
        if (cancelFirst)
        {
            plan.Cancel();
        }
        DateStatus originalStatus = plan.Status;

        Result result = plan.Attend();

        result.Error.ShouldHaveCode("DatePlan.MustBeBookedToAttend", ErrorType.Conflict);
        plan.Status.Should().Be(originalStatus);
    }

    [Fact]
    public void Attend_ShouldRejectRepeatedAttendance_WithoutChangingState()
    {
        DatePlan plan = CreateBookedPlan();
        plan.Attend();

        Result result = plan.Attend();

        result.Error.ShouldHaveCode("DatePlan.MustBeBookedToAttend", ErrorType.Conflict);
        plan.Status.Should().Be(DateStatus.Attended);
    }

    private DatePlan CreatePlan() => DatePlan.Create(_coupleId, _plannerId).Value;

    private DatePlan CreateBookedPlan()
    {
        DatePlan plan = CreatePlan();
        plan.AddItineraryItem("Dinner", null, null);
        plan.Schedule(new DateOnly(2026, 9, 20));
        return plan;
    }
}

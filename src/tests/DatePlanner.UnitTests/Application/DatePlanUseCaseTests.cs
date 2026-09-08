using DatePlanner.Application.Dates.AddItineraryItem;
using DatePlanner.Application.Dates.CreateDatePlan;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Couples;
using DatePlanner.Domain.Dates;
using FluentAssertions;

namespace DatePlanner.UnitTests.Application;

public sealed class DatePlanUseCaseTests
{
    [Fact]
    public async Task CreateDatePlan_ShouldUseCurrentCoupleAndPersistPlan()
    {
        Guid userId = Guid.NewGuid();
        Couple couple = Couple.Create(userId, Guid.NewGuid()).Value;
        var datePlans = new FakeDatePlanRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new CreateDatePlanCommandHandler(
            new FakeCoupleRepository(couple),
            datePlans,
            unitOfWork);

        Result<Guid> result = await handler.Handle(
            new CreateDatePlanCommand(userId),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        DatePlan datePlan = datePlans.DatePlans.Should().ContainSingle().Subject;
        datePlan.Id.Should().Be(result.Value);
        datePlan.CoupleId.Should().Be(couple.Id);
        datePlan.PartnerWhoPlannedId.Should().Be(userId);
        unitOfWork.SaveCalls.Should().Be(1);
    }

    [Fact]
    public async Task AddItineraryItem_ShouldRejectNonMemberWithoutMutatingOrSaving()
    {
        Couple couple = Couple.Create(Guid.NewGuid(), Guid.NewGuid()).Value;
        DatePlan datePlan = DatePlan.Create(couple.Id, couple.PartnerOneId).Value;
        var unitOfWork = new FakeUnitOfWork();
        var handler = new AddItineraryItemCommandHandler(
            new FakeDatePlanRepository(datePlan),
            new FakeCoupleRepository(couple),
            unitOfWork);

        Result result = await handler.Handle(
            new AddItineraryItemCommand(
                Guid.NewGuid(),
                datePlan.Id,
                "Dinner",
                null,
                null,
                null),
            CancellationToken.None);

        result.Error.Code.Should().Be("DatePlan.NotFound");
        datePlan.Itinerary.Should().BeEmpty();
        unitOfWork.SaveCalls.Should().Be(0);
    }

    [Fact]
    public async Task AddItineraryItem_ShouldCreateLocationAndSave_WhenUserIsMember()
    {
        Couple couple = Couple.Create(Guid.NewGuid(), Guid.NewGuid()).Value;
        DatePlan datePlan = DatePlan.Create(couple.Id, couple.PartnerOneId).Value;
        var unitOfWork = new FakeUnitOfWork();
        var handler = new AddItineraryItemCommandHandler(
            new FakeDatePlanRepository(datePlan),
            new FakeCoupleRepository(couple),
            unitOfWork);

        Result result = await handler.Handle(
            new AddItineraryItemCommand(
                couple.PartnerTwoId,
                datePlan.Id,
                "Dinner",
                21.1619m,
                -86.8515m,
                new TimeOnly(19, 30)),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        ItineraryItem item = datePlan.Itinerary.Should().ContainSingle().Subject;
        item.Location.Should().Be(Location.Create(21.1619m, -86.8515m).Value);
        unitOfWork.SaveCalls.Should().Be(1);
    }
}

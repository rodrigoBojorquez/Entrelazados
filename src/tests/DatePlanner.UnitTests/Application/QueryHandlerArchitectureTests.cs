using DatePlanner.Application.Common.Data;
using DatePlanner.Application.Couples.GetCurrentCouple;
using DatePlanner.Application.Dates.GetDatePlan;
using DatePlanner.Application.TurnGames.GetTurnGame;
using DatePlanner.Application.Users.GetUserProfile;
using FluentAssertions;

namespace DatePlanner.UnitTests.Application;

public sealed class QueryHandlerArchitectureTests
{
    public static TheoryData<Type> QueryHandlerTypes => new()
    {
        typeof(GetUserProfileQueryHandler),
        typeof(GetCurrentCoupleQueryHandler),
        typeof(GetDatePlanQueryHandler),
        typeof(GetTurnGameQueryHandler)
    };

    [Theory]
    [MemberData(nameof(QueryHandlerTypes))]
    public void QueryHandler_ShouldDependOnlyOnDbConnectionFactory(Type handlerType)
    {
        var constructor = handlerType.GetConstructors().Should().ContainSingle().Subject;

        constructor.GetParameters()
            .Select(parameter => parameter.ParameterType)
            .Should()
            .Equal(typeof(IDbConnectionFactory));
    }
}

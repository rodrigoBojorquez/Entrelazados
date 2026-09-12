using System.Data.Common;
using Dapper;
using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Dates;
using MediatR;

namespace DatePlanner.Application.Dates.GetDatePlan;

public sealed class GetDatePlanQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IRequestHandler<GetDatePlanQuery, Result<DatePlanResponse>>
{
    public async Task<Result<DatePlanResponse>> Handle(
        GetDatePlanQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            $"""
             SELECT
                 dp.id AS {nameof(DatePlanRow.Id)},
                 dp.couple_id AS {nameof(DatePlanRow.CoupleId)},
                 dp.partner_who_planned_id AS {nameof(DatePlanRow.PartnerWhoPlannedId)},
                 dp.is_collaborative AS {nameof(DatePlanRow.IsCollaborative)},
                 dp.created_at_utc AS {nameof(DatePlanRow.CreatedAtUtc)},
                 dp.scheduled_for AS {nameof(DatePlanRow.ScheduledFor)},
                 dp.status AS {nameof(DatePlanRow.Status)}
             FROM dateplanner.date_plans dp
             INNER JOIN dateplanner.couples c ON c.id = dp.couple_id
             WHERE dp.id = @DatePlanId
               AND (c.partner_one_id = @UserId OR c.partner_two_id = @UserId);

             SELECT
                 i.id AS {nameof(ItineraryItemResponse.Id)},
                 i."order" AS {nameof(ItineraryItemResponse.Order)},
                 i.name AS {nameof(ItineraryItemResponse.Name)},
                 i.starts_at AS {nameof(ItineraryItemResponse.StartsAt)},
                 i.latitude AS {nameof(ItineraryItemResponse.Latitude)},
                 i.longitude AS {nameof(ItineraryItemResponse.Longitude)}
             FROM dateplanner.itinerary_items i
             INNER JOIN dateplanner.date_plans dp ON dp.id = i.date_plan_id
             INNER JOIN dateplanner.couples c ON c.id = dp.couple_id
             WHERE i.date_plan_id = @DatePlanId
               AND (c.partner_one_id = @UserId OR c.partner_two_id = @UserId)
             ORDER BY i."order";
             """;

        var command = new CommandDefinition(sql, request, cancellationToken: cancellationToken);
        using SqlMapper.GridReader gridReader = await connection.QueryMultipleAsync(command);

        DatePlanRow? datePlan = await gridReader.ReadSingleOrDefaultAsync<DatePlanRow>();

        if (datePlan is null)
        {
            return Result.Failure<DatePlanResponse>(DatePlanErrors.NotFound(request.DatePlanId));
        }

        ItineraryItemResponse[] itinerary =
            (await gridReader.ReadAsync<ItineraryItemResponse>()).ToArray();

        return new DatePlanResponse(
            datePlan.Id,
            datePlan.CoupleId,
            datePlan.PartnerWhoPlannedId,
            datePlan.IsCollaborative,
            datePlan.CreatedAtUtc,
            datePlan.ScheduledFor,
            Enum.Parse<DateStatus>(datePlan.Status),
            itinerary);
    }

    private sealed class DatePlanRow
    {
        public Guid Id { get; init; }
        public Guid CoupleId { get; init; }
        public Guid PartnerWhoPlannedId { get; init; }
        public bool IsCollaborative { get; init; }
        public DateTime CreatedAtUtc { get; init; }
        public DateOnly? ScheduledFor { get; init; }
        public string Status { get; init; } = string.Empty;
    }
}

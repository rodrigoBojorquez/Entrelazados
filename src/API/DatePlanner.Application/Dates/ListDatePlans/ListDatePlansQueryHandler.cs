using System.Data.Common;
using Dapper;
using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Dates;
using MediatR;

namespace DatePlanner.Application.Dates.ListDatePlans;

public sealed class ListDatePlansQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IRequestHandler<ListDatePlansQuery, Result<IReadOnlyCollection<DatePlanSummaryResponse>>>
{
    public async Task<Result<IReadOnlyCollection<DatePlanSummaryResponse>>> Handle(
        ListDatePlansQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            $"""
             SELECT
                 dp.id AS {nameof(DatePlanRow.Id)},
                 dp.partner_who_planned_id AS {nameof(DatePlanRow.PartnerWhoPlannedId)},
                 planner.first_name || ' ' || planner.last_name AS {nameof(DatePlanRow.PlannerName)},
                 dp.is_collaborative AS {nameof(DatePlanRow.IsCollaborative)},
                 dp.created_at_utc AS {nameof(DatePlanRow.CreatedAtUtc)},
                 dp.scheduled_for AS {nameof(DatePlanRow.ScheduledFor)},
                 dp.status AS {nameof(DatePlanRow.Status)},
                 COALESCE(
                     (SELECT i.name
                      FROM dateplanner.itinerary_items i
                      WHERE i.date_plan_id = dp.id
                      ORDER BY i."order"
                      LIMIT 1),
                     'Una cita por descubrir') AS {nameof(DatePlanRow.Title)},
                 (SELECT COUNT(*)::int
                  FROM dateplanner.itinerary_items i
                  WHERE i.date_plan_id = dp.id) AS {nameof(DatePlanRow.ItineraryItemCount)}
             FROM dateplanner.date_plans dp
             INNER JOIN dateplanner.couples c ON c.id = dp.couple_id
             INNER JOIN dateplanner.users planner ON planner.id = dp.partner_who_planned_id
             WHERE c.partner_one_id = @UserId OR c.partner_two_id = @UserId
             ORDER BY dp.created_at_utc DESC;
             """;

        var command = new CommandDefinition(sql, request, cancellationToken: cancellationToken);
        DatePlanRow[] rows = (await connection.QueryAsync<DatePlanRow>(command)).ToArray();

        return rows.Select(row => new DatePlanSummaryResponse(
                row.Id,
                row.PartnerWhoPlannedId,
                row.PlannerName,
                row.IsCollaborative,
                row.CreatedAtUtc,
                row.ScheduledFor,
                Enum.Parse<DateStatus>(row.Status),
                row.Title,
                row.ItineraryItemCount))
            .ToArray();
    }

    private sealed class DatePlanRow
    {
        public Guid Id { get; init; }
        public Guid PartnerWhoPlannedId { get; init; }
        public string PlannerName { get; init; } = string.Empty;
        public bool IsCollaborative { get; init; }
        public DateTime CreatedAtUtc { get; init; }
        public DateOnly? ScheduledFor { get; init; }
        public string Status { get; init; } = string.Empty;
        public string Title { get; init; } = string.Empty;
        public int ItineraryItemCount { get; init; }
    }
}

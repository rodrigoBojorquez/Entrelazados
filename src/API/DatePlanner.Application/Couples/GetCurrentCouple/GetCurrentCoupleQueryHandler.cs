using System.Data.Common;
using Dapper;
using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Couples;
using MediatR;

namespace DatePlanner.Application.Couples.GetCurrentCouple;

public sealed class GetCurrentCoupleQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IRequestHandler<GetCurrentCoupleQuery, Result<CurrentCoupleResponse>>
{
    public async Task<Result<CurrentCoupleResponse>> Handle(
        GetCurrentCoupleQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            $"""
             SELECT
                 c.id AS {nameof(CurrentCoupleResponse.Id)},
                 c.partner_one_id AS {nameof(CurrentCoupleResponse.PartnerOneId)},
                 c.partner_two_id AS {nameof(CurrentCoupleResponse.PartnerTwoId)},
                 c.begin_relationship_at AS {nameof(CurrentCoupleResponse.BeginRelationshipAt)},
                 c.is_active AS {nameof(CurrentCoupleResponse.IsActive)}
             FROM dateplanner.couples c
             WHERE c.is_active
               AND (c.partner_one_id = @UserId OR c.partner_two_id = @UserId)
             """;

        var command = new CommandDefinition(sql, request, cancellationToken: cancellationToken);
        CurrentCoupleResponse? couple =
            await connection.QuerySingleOrDefaultAsync<CurrentCoupleResponse>(command);

        if (couple is null)
        {
            return Result.Failure<CurrentCoupleResponse>(CoupleErrors.NotFound(request.UserId));
        }

        return couple;
    }
}

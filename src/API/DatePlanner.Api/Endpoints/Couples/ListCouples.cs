using FastEndpoints;

namespace DatePlanner.Api.Endpoints.Couples;

public class ListCouples : Endpoint<EmptyRequest, ListCouplesResponse>
{
    public override void Configure()
    {
        Get("/couples");
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var couples = new List<CoupleDto>
        {
            new CoupleDto(Guid.NewGuid()),
            new CoupleDto(Guid.NewGuid()),
        };

        await Send.OkAsync(new ListCouplesResponse(couples), cancellation: ct);
    }
}

public record ListCouplesResponse(List<CoupleDto> Couples);

public record CoupleDto(Guid Id);
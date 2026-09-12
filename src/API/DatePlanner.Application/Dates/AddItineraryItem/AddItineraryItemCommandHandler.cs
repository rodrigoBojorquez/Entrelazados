using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Couples;
using DatePlanner.Domain.Dates;
using MediatR;

namespace DatePlanner.Application.Dates.AddItineraryItem;

public sealed class AddItineraryItemCommandHandler(
    IDatePlanRepository datePlanRepository,
    ICoupleRepository coupleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AddItineraryItemCommand, Result>
{
    public async Task<Result> Handle(
        AddItineraryItemCommand request,
        CancellationToken cancellationToken)
    {
        Result<DatePlan> accessResult = await DatePlanAccess.GetForEditorAsync(
            request.DatePlanId,
            request.UserId,
            datePlanRepository,
            coupleRepository);

        if (accessResult.IsFailure)
        {
            return Result.Failure(accessResult.Error);
        }

        Result<Location>? locationResult = CreateLocation(request.Latitude, request.Longitude);

        if (locationResult?.IsFailure == true)
        {
            return Result.Failure(locationResult.Error);
        }

        Result result = accessResult.Value.AddItineraryItem(
            request.Name,
            locationResult?.Value,
            request.StartsAt);

        if (result.IsFailure)
        {
            return result;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static Result<Location>? CreateLocation(decimal? latitude, decimal? longitude)
    {
        if (latitude.HasValue != longitude.HasValue)
        {
            return Result.Failure<Location>(DatePlanErrors.IncompleteLocation);
        }

        return latitude.HasValue
            ? Location.Create(latitude.Value, longitude!.Value)
            : null;
    }
}

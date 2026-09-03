using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.Dates;

public static class DatePlanErrors
{

    public static Error MustBePlanning => Error.Conflict(
        "DatePlan.MustBePlanning",
        "El plan debe estar en estado de planeación para realizar esta operación.");

    public static Error ScheduledDateIsRequired => Error.Problem(
        "DatePlan.ScheduledDateIsRequired",
        "La fecha programada es obligatoria.");

    public static Error EmptyItinerary => Error.Conflict(
        "DatePlan.EmptyItinerary",
        "No se puede programar una cita sin elementos en el itinerario.");

    public static Error ItineraryItemNameIsRequired => Error.Problem(
        "DatePlan.ItineraryItemNameIsRequired",
        "El nombre del elemento del itinerario es obligatorio.");

    public static Error ItineraryItemIdIsRequired => Error.Problem(
        "DatePlan.ItineraryItemIdIsRequired",
        "El identificador del elemento del itinerario es obligatorio.");

    public static Error ItineraryItemNotFound(Guid itineraryItemId) => Error.NotFound(
        "DatePlan.ItineraryItemNotFound",
        $"No existe el elemento de itinerario {itineraryItemId}.");

    public static Error CannotCancel => Error.Conflict(
        "DatePlan.CannotCancel",
        "Una cita cancelada o atendida no se puede cancelar nuevamente.");

    public static Error MustBeBookedToAttend => Error.Conflict(
        "DatePlan.MustBeBookedToAttend",
        "Solo una cita programada puede marcarse como atendida.");

    public static Error
        EndTimePrecedesStartTime => Error.Problem(
        "TimeRange.EndTimePrecedesStartTime", 
        "La hora de fin es menor a la hora de inicio");
}

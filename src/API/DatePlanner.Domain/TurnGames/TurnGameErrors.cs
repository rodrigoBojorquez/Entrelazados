using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.TurnGames;

public static class TurnGameErrors
{
    public static Error NotFound(Guid turnGameId) => Error.NotFound(
        "TurnGame.NotFound",
        $"No existe la partida {turnGameId} o el usuario no tiene acceso a ella.");

    public static Error SamePlayerGame => Error.Problem(
        "TurnGame.SamePlayerGame",
        "Los participantes deben ser diferentes.");

    public static Error CoupleMustBeActive => Error.Conflict(
        "TurnGame.CoupleMustBeActive",
        "La pareja debe estar activa para iniciar una partida.");

    public static Error UnsupportedGameType => Error.Problem(
        "TurnGame.UnsupportedGameType",
        "El tipo de juego no está soportado.");

    public static Error GameAlreadyFinished => Error.Conflict(
        "TurnGame.AlreadyFinished",
        "La partida ya terminó.");

    public static Error GameMustBeInProgress => Error.Conflict(
        "TurnGame.NotInProgress",
        "La partida no está en progreso.");

    public static Error ActiveRoundInProgress => Error.Conflict(
        "TurnGame.ActiveRoundExists",
        "Ya existe una ronda activa.");

    public static Error NoActiveRound => Error.Conflict(
        "TurnGame.NoActiveRound",
        "No existe una ronda activa.");

    public static Error PlayerIsNotParticipant => Error.Problem(
        "TurnGame.NotParticipant",
        "El usuario no participa en esta partida.");

    public static Error IncompatibleMove => Error.Problem(
        "TurnGame.IncompatibleMove",
        "El movimiento no pertenece al tipo de juego.");

    public static Error MoveIsRequired => Error.Problem(
        "TurnGame.MoveIsRequired",
        "El movimiento es obligatorio.");

    public static Error InvalidMove => Error.Problem(
        "TurnGame.InvalidMove",
        "El movimiento no es válido.");

    public static Error PlayerAlreadyMoved => Error.Conflict(
        "TurnGame.PlayerAlreadyMoved",
        "El jugador ya realizó todos sus movimientos permitidos.");

    public static Error RoundAlreadyFinished => Error.Conflict(
        "TurnGame.RoundAlreadyFinished",
        "No se puede jugar en una ronda terminada.");
}

using Briscola.Domain.Entities;
using Briscola.Domain.Enums;

namespace Briscola.Persistence.Entities;

/// <summary>
/// Represents a log of a Briscola game, containing all necessary information to reconstruct the match history.
/// </summary>
/// <param name="Teams">List of teams that participated in the match</param>
/// <param name="Players">List of players that participated in the match</param>
/// <param name="Timestamp">Date and time when the match was played</param>
/// <param name="BriscolaCard">The card that was designated as the Briscola for the match</param>
/// <param name="WinningTeam">The team that won the match</param>
public record GameLog(List<TeamLog> Teams, List<PlayerLog> Players,
    DateTime Timestamp, Card BriscolaCard,
    List<TrickLog> Tricks, GameResult GameResult);
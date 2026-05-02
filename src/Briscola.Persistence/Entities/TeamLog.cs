namespace Briscola.Persistence.Entities;

/// <summary>
/// Represents a log of a team during a match
/// </summary>
/// <param name="Id">Team identifier</param>
/// <param name="Players">List of players</param>
/// <param name="Score">Score of the team</param>
public record TeamLog(string Id, List<PlayerLog> Players, int Score);
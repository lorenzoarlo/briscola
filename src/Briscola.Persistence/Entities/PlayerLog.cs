namespace Briscola.Persistence.Entities;


/// <summary>
/// Represents a log entry for a player's performance in a match
/// </summary>
/// <param name="Id">Player id</param>
/// <param name="Name">Player name</param>
/// <param name="TeamId">Team id</param>
public record PlayerLog(string Id, string Name, string TeamId);
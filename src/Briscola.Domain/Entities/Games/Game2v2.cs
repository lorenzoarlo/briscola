using Briscola.Domain.Enums;

namespace Briscola.Domain.Entities.Games;

/// <summary>Represents a 2v2 Briscola game.</summary>
public class Game2v2(Team teamA, Team teamB) : Game([teamA, teamB], OrderPlayers(teamA, teamB))
{
    /// <summary>Orders players in alternating team order.</summary>
    private static IEnumerable<Player> OrderPlayers(Team teamA, Team teamB)
    {
        var aPlayers = teamA.Players.ToList();
        var bPlayers = teamB.Players.ToList();

        if (aPlayers.Count != 2 || bPlayers.Count != 2)
            throw new ArgumentException("Each team must have exactly 2 players.");

        return [aPlayers[0], bPlayers[0], aPlayers[1], bPlayers[1]];
    }

    /// <summary>Validates the players and teams for a 2v2 game.</summary>
    protected override void ValidatePlayers(IReadOnlyList<Player> players, IReadOnlyList<Team> teams)
    {
        if (players.Count != 4)
            throw new ArgumentException("Game2v2 requires exactly 4 players.");

        if (teams.Count != 2)
            throw new ArgumentException("Game2v2 requires exactly 2 teams.");

        if (teams[0].Id == teams[1].Id)
            throw new ArgumentException("Game2v2 requires 2 distinct teams.");

        foreach (var team in teams)
        {
            if (team.Players.Count != 2)
                throw new ArgumentException($"Team {team.Id} must have exactly 2 players.");
        }

        // Every player must belong to one of the provided teams
        var teamIds = teams.Select(t => t.Id).ToHashSet();
        foreach (var p in players)
        {
            if (p.TeamId is null || !teamIds.Contains(p.TeamId))
                throw new ArgumentException($"Player {p.Id} is not assigned to any of the game's teams.");
        }

        // Cross-check: union of teams' players matches the players list
        var playersFromTeams = teams.SelectMany(t => t.Players).ToHashSet();
        if (playersFromTeams.Count != players.Count || !players.All(playersFromTeams.Contains))
            throw new ArgumentException("Players list does not match the union of team rosters.");
    }
}

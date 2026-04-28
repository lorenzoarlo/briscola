using Briscola.Domain.Enums;

namespace Briscola.Domain.Entities.Games;

public class Game1v1 : Game
{
    /// <summary>Initializes a new 1v1 game.</summary>
    public Game1v1(Player a, Player b) : this(BuildTeams(a, b), a, b) { }

    /// <summary>Internal constructor for 1v1 game.</summary>
    private Game1v1((Team t1, Team t2) teams, Player a, Player b)
        : base([teams.t1, teams.t2], [a, b]) { }

    /// <summary>Builds synthetic teams for 1v1.</summary>
    private static (Team, Team) BuildTeams(Player a, Player b) =>
        (new Team(id: $"team-{a.Id}", name: a.Name, [a]),
         new Team(id: $"team-{b.Id}", name: b.Name, [b]));

    /// <summary>Validates the players and teams for a 1v1 game.</summary>
    protected override void ValidatePlayers(IReadOnlyList<Player> players, IReadOnlyList<Team> teams)
    {
        if (players.Count != 2)
            throw new ArgumentException("Game1v1 requires exactly 2 players.");

        if (teams.Count != 2)
            throw new ArgumentException("Game1v1 requires exactly 2 teams.");

        foreach (var team in teams)
        {
            if (team.Players.Count != 1)
                throw new ArgumentException($"Team {team.Id} must have exactly 1 player in a 1v1 game.");
        }

        // Every player must belong to one of the provided teams
        var teamIds = teams.Select(t => t.Id).ToHashSet();
        foreach (var p in players)
        {
            if (p.TeamId is null || !teamIds.Contains(p.TeamId))
            {
                throw new ArgumentException($"Player {p.Id} is not assigned to any of the game's teams.");
            }
        }
    }
}

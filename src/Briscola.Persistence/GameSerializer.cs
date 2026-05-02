using Briscola.Domain.Entities;
using Briscola.Persistence.Entities;

namespace Briscola.Persistence;

/// <summary>
/// Handles serialization and deserialization of game state for saving and loading matches
/// </summary>
public class GameSerializer
{
    public static GameLog Write(Game game)
    {
        var teams = game.Teams.Select(t => new TeamLog(t.Id, [.. t.Players.Select(p => new PlayerLog(p.Id, p.Name, t.Id))], t.Score))
            .ToList();
        var players = game.Teams.SelectMany(t => t.Players)
                                .Select(p => new PlayerLog(p.Id, p.Name, p.TeamId!))
                                .ToList();
        var timestamp = DateTime.Now;
        var briscolaCard = game.BriscolaCard;
        var result = game.GameResult;
        return new GameLog(teams, players, timestamp, briscolaCard, result);
    }
}
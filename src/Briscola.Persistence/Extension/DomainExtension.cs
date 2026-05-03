using Briscola.Domain.Entities;
using Briscola.Persistence.Entities;

namespace Briscola.Persistence.Extension;


/// <summary>
/// Provides extension methods to convert domain entities to persistence logs for saving game state
/// </summary>
public static class DomainPersistenceExtension
{
    /// <summary>
    /// Converts a domain player to a persistence player log
    /// </summary>
    public static PlayerLog ToPlayerLog(this Player player) => player.TeamId is not null
        ? new PlayerLog(player.Id, player.Name, player.TeamId)
        : throw new ArgumentException("Player must have a team id to be converted to PlayerLog");

    /// <summary>
    /// Converts a domain team to a persistence team log
    /// </summary>
    public static TeamLog ToTeamLog(this Team team) => new(team.Id, [.. team.Players.Select(p => p.ToPlayerLog())], team.Score);

    /// <summary>
    /// Converts a domain trick to a persistence trick log
    /// </summary>
    /// <param name="id">Number of the trick</param>
    public static TrickLog ToTrickLog(this Trick trick, int id)
    {
        if (trick.CurrentPlayer is not null)
        {
            throw new ArgumentException("Trick must be completed to be converted to TrickLog");
        }
        var hands = trick.Players.ToDictionary(p => p.Id, p => new HandLog([.. p.Hand.Cards]));
        var cardsPlayed = trick.Players.Select(p => (p.Id, trick.CardOfPlayer(p) ?? throw new InvalidOperationException("Trick must have a card for each player")));
        var winner = trick.Winner();
        return new TrickLog(id, hands, [.. cardsPlayed], winner.ToPlayerLog());
    }

    /// <summary>
    /// Converts a domain game to a persistence game log
    /// </summary>
    /// <param name="game"></param>
    /// <returns></returns>
    public static GameLog ToGameLog(this Game game)
    {
        var teams = game.Teams.Select(t => t.ToTeamLog()).ToList();
        var players = game.Teams.SelectMany(t => t.Players)
                                .Select(p => p.ToPlayerLog())
                                .ToList();
        var timestamp = DateTime.Now;
        var briscolaCard = game.BriscolaCard;
        var tricks = game.Tricks.Select((t, i) => t.ToTrickLog(i)).ToList();
        var result = game.GameResult;
        return new GameLog(teams, players, timestamp, briscolaCard, tricks, result);
    }

}
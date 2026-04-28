using Briscola.Domain.Entities;
using Briscola.Domain.Entities.Games;
using Briscola.Tests.Domain.Entities.Utils;

namespace Briscola.Tests.Domain.Entities.Games;

public class Game2v2Tests
{
    [Fact]
    public void Game2v2_Initialization_ValidatesPlayers()
    {
        var p1 = new Player("p1", "Player 1", new TestPlayerStrategy());
        var p2 = new Player("p2", "Player 2", new TestPlayerStrategy());
        var p3 = new Player("p3", "Player 3", new TestPlayerStrategy());
        var p4 = new Player("p4", "Player 4", new TestPlayerStrategy());

        var teamA = new Team("t1", "Team 1", [p1, p2]);
        var teamB = new Team("t2", "Team 2", [p3, p4]);

        var game = new Game2v2(teamA, teamB);

        Assert.Equal(4, game.Players.Count);
        Assert.Equal(p1, game.Players[0]);
        Assert.Equal(p3, game.Players[1]);
        Assert.Equal(p2, game.Players[2]);
        Assert.Equal(p4, game.Players[3]);
    }

    [Fact]
    public void Game2v2_Initialization_InvalidTeams_ThrowsException()
    {
        var p1 = new Player("p1", "Player 1", new TestPlayerStrategy());
        var p2 = new Player("p2", "Player 2", new TestPlayerStrategy());
        var p3 = new Player("p3", "Player 3", new TestPlayerStrategy());

        var teamA = new Team("t1", "Team 1", [p1, p2]);
        var teamB = new Team("t2", "Team 2", [p3]); // Only 1 player

        Assert.Throws<ArgumentException>(() => new Game2v2(teamA, teamB));
    }
}
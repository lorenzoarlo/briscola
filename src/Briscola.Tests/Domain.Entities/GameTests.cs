using Briscola.Domain.Entities;
using Briscola.Domain.Entities.Games;
using Briscola.Domain.Enums;
using Briscola.Tests.Domain.Entities.Utils;

namespace Briscola.Tests.Domain.Entities;

public class GameTests
{
    [Fact]
    public async Task TwoPlayerMatch_DealsCorrectly()
    {
        var player1 = new Player("p1", "Player 1", new TestPlayerStrategy());
        var player2 = new Player("p2", "Player 2", new TestPlayerStrategy());
        var game = new Game1v1(player1, player2);

        await game.DealAsync();

        Assert.Equal(GameState.PlayerTurn, game.State);
        Assert.Equal(3, player1.Hand.Cards.Count);
        Assert.Equal(3, player2.Hand.Cards.Count);
        Assert.NotNull(game.BriscolaCard);
        // Deck is protected, so we can't easily assert on it, but we can check if tricks are initialized
        Assert.NotNull(game.CurrentTrick);
    }

    [Fact]
    public async Task Trick_Evaluation_AssignsPointsCorrectly()
    {
        var player1 = new Player("p1", "Player 1", new TestPlayerStrategy());
        var player2 = new Player("p2", "Player 2", new TestPlayerStrategy());
        var game = new Game1v1(player1, player2);

        await game.DealAsync();

        var p1 = game.CurrentPlayer!;
        var p2 = game.Players.First(p => p.Id != p1.Id);

        // We override hands and briscola for deterministic testing
        while (!p1.Hand.IsEmpty) p1.Play(p1.Hand.Cards.First());
        while (!p2.Hand.IsEmpty) p2.Play(p2.Hand.Cards.First());

        p1.Draw(new Card(Suit.Coins, CardValue.Three)); // 10 points
        p2.Draw(new Card(Suit.Cups, CardValue.Ace)); // 11 points, but off-suit

        // Cheat: let's assume Briscola is Swords
        var briscolaProp = typeof(Game).GetProperty("BriscolaSuit");
        briscolaProp?.SetValue(game, Suit.Swords);

        await game.PlayCardAsync(p1.Hand.Cards.First());
        await game.PlayCardAsync(p2.Hand.Cards.First());

        // Coins lead, Cups off-suit. Coins wins.
        Assert.Equal(10 + 11, p1.Score);
        Assert.Equal(0, p2.Score);

        // p1 won the trick, they should be the current player for the next trick
        Assert.Equal(p1.Id, game.CurrentPlayer!.Id);
    }
}

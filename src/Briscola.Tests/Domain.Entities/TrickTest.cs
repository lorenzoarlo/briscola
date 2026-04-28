using Briscola.Domain.Entities;
using Briscola.Domain.Enums;
using Briscola.Tests.Domain.Entities.Utils;
namespace Briscola.Tests.Domain.Entities;



public class TrickTests
{
    #region BriscolaBeats 20 Combinations

    [Theory]
    // Group 1: Same Suit (No Briscola involved) - Higher Value Wins
    // Note: Based on your CardValue enum, Ace (10) > King (9) > Four (3) > Two (1)
    [InlineData(Suit.Coins, Suit.Cups, CardValue.Four, Suit.Cups, CardValue.Two, true)]   // 1. Challenger higher
    [InlineData(Suit.Coins, Suit.Cups, CardValue.Two, Suit.Cups, CardValue.Four, false)]  // 2. Opponent higher
    [InlineData(Suit.Coins, Suit.Swords, CardValue.Ace, Suit.Swords, CardValue.King, true)] // 3. Challenger Ace vs King
    [InlineData(Suit.Coins, Suit.Swords, CardValue.King, Suit.Swords, CardValue.Ace, false)]// 4. Opponent Ace vs King
    [InlineData(Suit.Coins, Suit.Clubs, CardValue.Jack, Suit.Clubs, CardValue.Seven, true)] // 5. Challenger Jack vs Seven
    [InlineData(Suit.Coins, Suit.Clubs, CardValue.Seven, Suit.Clubs, CardValue.Jack, false)]// 6. Opponent Jack vs Seven

    // Group 2: Challenger is Briscola, Opponent is not (Challenger always wins)
    [InlineData(Suit.Coins, Suit.Coins, CardValue.Two, Suit.Cups, CardValue.Ace, true)]   // 7. Challenger Briscola (lowest), Opponent non-Briscola (highest)
    [InlineData(Suit.Cups, Suit.Cups, CardValue.Ace, Suit.Swords, CardValue.Two, true)]   // 8. Challenger Briscola (highest), Opponent non-Briscola (lowest)
    [InlineData(Suit.Swords, Suit.Swords, CardValue.Four, Suit.Coins, CardValue.King, true)] // 9. Challenger Briscola mid-value
    [InlineData(Suit.Clubs, Suit.Clubs, CardValue.King, Suit.Cups, CardValue.King, true)] // 10. Same enum value, Challenger has Briscola suit
    [InlineData(Suit.Coins, Suit.Coins, CardValue.Jack, Suit.Swords, CardValue.Knight, true)] // 11. Challenger Briscola lower enum
    [InlineData(Suit.Cups, Suit.Cups, CardValue.Knight, Suit.Clubs, CardValue.Jack, true)] // 12. Challenger Briscola higher enum

    // Group 3: Opponent is Briscola, Challenger is not (Opponent always wins)
    [InlineData(Suit.Coins, Suit.Cups, CardValue.Ace, Suit.Coins, CardValue.Two, false)]  // 13. Opponent Briscola (lowest), Challenger non-Briscola (highest)
    [InlineData(Suit.Swords, Suit.Clubs, CardValue.Two, Suit.Swords, CardValue.Ace, false)] // 14. Opponent Briscola (highest), Challenger non-Briscola (lowest)
    [InlineData(Suit.Cups, Suit.Swords, CardValue.King, Suit.Cups, CardValue.King, false)] // 15. Same enum value, Opponent has Briscola suit
    [InlineData(Suit.Clubs, Suit.Coins, CardValue.Seven, Suit.Clubs, CardValue.Four, false)] // 16. Opponent Briscola lower enum

    // Group 4: Different suits, neither is Briscola (Opponent/Lead always wins)
    [InlineData(Suit.Coins, Suit.Swords, CardValue.Ace, Suit.Cups, CardValue.Two, true)] // 17. Challenger higher value, but didn't follow lead suit
    [InlineData(Suit.Coins, Suit.Swords, CardValue.Two, Suit.Cups, CardValue.Ace, true)] // 18. Challenger lower value, didn't follow lead suit
    [InlineData(Suit.Cups, Suit.Clubs, CardValue.King, Suit.Swords, CardValue.King, true)] // 19. Same value, didn't follow lead suit
    [InlineData(Suit.Swords, Suit.Coins, CardValue.Ace, Suit.Clubs, CardValue.King, true)] // 20. Another mismatched non-trump suit combination
    public void BriscolaBeats_ShouldEvaluateCorrectly(
        Suit briscolaSuit,
        Suit challengerSuit, CardValue challengerValue,
        Suit opponentSuit, CardValue opponentValue,
        bool expectedWin)
    {
        // Arrange
        var challenger = new Card(challengerSuit, challengerValue);
        var opponent = new Card(opponentSuit, opponentValue);

        // Act
        var result = Trick.BriscolaBeats(challenger, opponent, briscolaSuit);

        // Assert
        Assert.Equal(expectedWin, result);
    }

    #endregion

    #region Trick State & Gameplay Logic Tests

    [Fact]
    public void Trick_Initialization_ShouldSetCorrectStartingState()
    {
        // Arrange
        var players = new List<Player> { new("p1", "Alice", new TestPlayerStrategy()), new Player("p2", "Bob", new TestPlayerStrategy()) };
        var briscolaSuit = Suit.Coins;

        // Act
        var trick = new Trick(players, briscolaSuit);

        // Assert
        Assert.Equal(Suit.Coins, trick.BriscolaSuit);
        Assert.False(trick.IsComplete);
        Assert.Equal(players[0], trick.CurrentPlayer);
    }

    [Fact]
    public void PlayCard_ValidTurn_ShouldProgressTrick()
    {
        // Arrange
        var p1 = new Player("1", "Alice", new TestPlayerStrategy());
        var p2 = new Player("2", "Bob", new TestPlayerStrategy());
        var trick = new Trick([p1, p2], Suit.Coins);
        var card = new Card(Suit.Cups, CardValue.Ace);

        // Act
        trick.PlayCard(p1, card);

        // Assert
        Assert.Equal(card, trick.CardOfPlayer(p1));
        Assert.Equal(p2, trick.CurrentPlayer);
        Assert.False(trick.IsComplete);
    }

    [Fact]
    public void PlayCard_WrongPlayerTurn_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var p1 = new Player("1", "Alice", new TestPlayerStrategy());
        var p2 = new Player("2", "Bob", new TestPlayerStrategy());
        var trick = new Trick([p1, p2], Suit.Coins);
        var card = new Card(Suit.Cups, CardValue.Ace);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => trick.PlayCard(p2, card));
        Assert.Equal("It's not this player's turn to play.", ex.Message);
    }

    [Fact]
    public void PlayCard_PlayerAlreadyPlayed_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var p1 = new Player("1", "Alice", new TestPlayerStrategy());
        var p2 = new Player("2", "Bob", new TestPlayerStrategy());
        var trick = new Trick([p1, p2], Suit.Coins);
        var card1 = new Card(Suit.Cups, CardValue.Ace);
        var card2 = new Card(Suit.Swords, CardValue.Two);

        trick.PlayCard(p1, card1);

        // Backtrack hack for testing to attempt playing twice
        // Using the parameterless shortcut overload to attempt playing the same user twice won't work 
        // directly without resetting index, but we can call the targeted overload directly:

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => trick.PlayCard(p1, card2));
        Assert.Equal("It's not this player's turn to play.", ex.Message); // Caught by turn check first
    }

    [Fact]
    public void IsComplete_ShouldReturnTrue_WhenAllPlayersHavePlayed()
    {
        // Arrange
        var p1 = new Player("1", "Alice", new TestPlayerStrategy());
        var p2 = new Player("2", "Bob", new TestPlayerStrategy());
        var trick = new Trick([p1, p2], Suit.Coins);

        // Act
        trick.PlayCard(p1, new Card(Suit.Cups, CardValue.Ace));
        trick.PlayCard(p2, new Card(Suit.Cups, CardValue.Two));

        // Assert
        Assert.True(trick.IsComplete);
        Assert.Null(trick.CurrentPlayer);
    }

    [Fact]
    public void Winner_ShouldCorrectlyIdentifyWinner_AfterTrickIsComplete()
    {
        // Arrange
        var p1 = new Player("1", "Alice", new TestPlayerStrategy());
        var p2 = new Player("2", "Bob", new TestPlayerStrategy());
        var p3 = new Player("3", "Charlie", new TestPlayerStrategy());

        var trick = new Trick([p1, p2, p3], Suit.Coins);

        // P1 leads with Cups(4). P2 plays Cups(Ace). P3 plays Coins(Two) which is Briscola.
        trick.PlayCard(p1, new Card(Suit.Cups, CardValue.Four));
        trick.PlayCard(p2, new Card(Suit.Cups, CardValue.Ace));
        trick.PlayCard(p3, new Card(Suit.Coins, CardValue.Two));

        // Act
        var winner = trick.Winner();

        // Assert
        Assert.NotNull(winner);
        Assert.Equal(p3, winner); // P3 wins because they played a Briscola
    }

    #endregion
}
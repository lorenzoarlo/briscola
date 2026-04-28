using Briscola.Domain.Enums;
using Briscola.Domain.Entities;

namespace Briscola.Tests.Domain.Entities;

public class CardTests
{
    [Theory]
    [InlineData(CardValue.Ace, 11)]
    [InlineData(CardValue.Three, 10)]
    [InlineData(CardValue.King, 4)]
    [InlineData(CardValue.Knight, 3)]
    [InlineData(CardValue.Jack, 2)]
    [InlineData(CardValue.Seven, 0)]
    [InlineData(CardValue.Six, 0)]
    [InlineData(CardValue.Five, 0)]
    [InlineData(CardValue.Four, 0)]
    [InlineData(CardValue.Two, 0)]
    public void Card_Has_Correct_Points(CardValue value, int expectedPoints)
    {
        Assert.Equal(expectedPoints, new Card(Suit.Coins, value).Points);
        Assert.Equal(expectedPoints, new Card(Suit.Clubs, value).Points);
        Assert.Equal(expectedPoints, new Card(Suit.Cups, value).Points);
        Assert.Equal(expectedPoints, new Card(Suit.Swords, value).Points);
    }
}

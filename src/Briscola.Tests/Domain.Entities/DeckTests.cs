using Briscola.Domain.Entities;

namespace Briscola.Tests.Domain.Entities;

public class DeckTests
{
    [Fact]
    public void Deck_Has_Exactly_40_Cards()
    {
        var deck = new Deck();
        Assert.Equal(40, deck.Count);
    }

    [Fact]
    public void Deck_Draw_Reduces_Count()
    {
        var deck = new Deck();
        var card = deck.Draw();
        Assert.NotNull(card);
        Assert.Equal(39, deck.Count);
    }

    [Fact]
    public void Deck_Draw_Throws_Exception_When_Empty()
    {
        var deck = new Deck();
        for (int i = 0; i < 40; i++)
        {
            deck.Draw();
        }

        Assert.Throws<InvalidOperationException>(() => deck.Draw());
        Assert.True(deck.IsEmpty);
    }
}

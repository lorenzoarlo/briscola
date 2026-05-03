using Briscola.Domain.Enums;

namespace Briscola.Domain.Entities;

/// <summary>Represents a deck of cards.</summary>
public class Deck
{
    // Internal stack to manage the cards in the deck
    // Shuffle immediately
    private readonly Stack<Card> _cards = new(Shuffle(Create()));

    public const int TotalCards = 40;


    /// <summary>Number of cards remaining.</summary>
    public int Count => _cards.Count;

    /// <summary>Whether the deck is empty.</summary>
    public bool IsEmpty => _cards.Count == 0;

    /// <summary>Draws a card from the deck.</summary>
    public Card Draw()
    {
        if (_cards.TryPop(out var card))
        {
            return card;
        }
        throw new InvalidOperationException("Cannot draw a card: deck is empty.");
    }


    /// <summary>Creates a standard 40-card deck.</summary>
    public static IEnumerable<Card> Create()
    {
        var suits = Enum.GetValues<Suit>();
        var values = Enum.GetValues<CardValue>();
        List<Card> cards = new(suits.Length * values.Length);

        // Generate all 40 cards in a standard Briscola deck
        foreach (Suit suit in suits)
        {
            foreach (CardValue value in values)
            {
                cards.Add(new Card(suit, value));
            }
        }
        return cards;
    }

    /// <summary>Shuffles the collection of cards.</summary>
    public static IEnumerable<Card> Shuffle(IEnumerable<Card> cards) => cards.OrderBy(_ => Random.Shared.Next());

}

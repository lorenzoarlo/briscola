
namespace Briscola.Domain.Entities;

/// <summary>Represents a player's hand.</summary>
public class Hand
{

    // Internal list to manage the cards in the player's hand
    private readonly Card?[] _hand = new Card[ImmutableHand.MAX_HAND_SIZE];

    /// <summary> Number of cards currently in the hand. </summary>
    public int Count => AsReadonly().Count;

    /// <summary>Whether the hand is empty.</summary>
    public bool IsEmpty => AsReadonly().IsEmpty;

    /// <summary>Whether the hand is full.</summary>
    public bool IsFull => AsReadonly().IsFull;

    /// <summary>Adds a card to the hand.</summary>
    /// <param name="card">The card to be added to the player's hand, which should be drawn from the deck during the game.  </param>
    /// <exception cref="InvalidOperationException"> Thrown when attempting to add a card to the hand when it is already full </exception>
    public void Draw(Card card)
    {
        if (IsFull) { throw new InvalidOperationException("Cannot draw a card: hand is already full."); }
        int index = Array.IndexOf(_hand, null);
        _hand[index] = card;
    }

    /// <summary>Removes a card from the hand.</summary>
    /// <param name="card">The card to be removed from the player's hand </param>
    /// <exception cref="InvalidOperationException"> Thrown when attempting to play a card that is not in the hand </exception>
    public void Play(Card card)
    {
        int index = Array.IndexOf(_hand, card);
        if (index == -1) { throw new InvalidOperationException("Player does not have the specified card in hand."); }
        _hand[index] = null;
    }

    /// <summary>
    /// Returns a read-only snapshot of the current hand, which can be safely shared with player strategies without risking unintended modifications to the hand's state. 
    /// </summary>
    /// <returns>ImmutableHanf</returns>
    public ImmutableHand AsReadonly() => new([.. _hand.Where(c => c is not null).Select(c => c!)]);
}

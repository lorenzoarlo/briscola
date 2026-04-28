using Briscola.Domain.Entities;

namespace Briscola.View.CLI.Utils;

/// <summary>
/// Represents the player's hand in the CLI interface, allowing for drawing and playing cards while maintaining the state of each slot.
/// </summary>
internal sealed class HandSlots
{
    private readonly Card?[] _slots = new Card?[ImmutableHand.MAX_HAND_SIZE];

    public IReadOnlyList<Card?> View => _slots;

    /// <summary>
    /// Draws a card into the first available slot in the hand. If the hand is full, the card will not be added.
    /// </summary>
    public void Draw(Card c)
    {
        int index = Array.IndexOf(_slots, null);
        if (index == -1) { throw new InvalidOperationException("Cannot draw a card: hand is already full."); }
        _slots[index] = c;
    }

    /// <summary>
    /// Plays a card from the hand, removing it from the corresponding slot.
    /// </summary>
    public void Play(Card c)
    {
        int index = Array.IndexOf(_slots, c);
        if (index == -1) { throw new InvalidOperationException("Cannot play the specified card: it is not in the hand."); }
        _slots[index] = null;
    }
}
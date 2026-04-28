using System.Collections.Immutable;

namespace Briscola.Domain.Entities;

/// <summary>
/// Represents an immutable snapshot of a player's hand, providing a read-only view of the cards currently held by the player.
/// </summary>
public record ImmutableHand(IReadOnlyCollection<Card> Cards)
{
    /// <summary>Maximum cards in hand.</summary>
    public const int MAX_HAND_SIZE = 3;

    /// <summary> Number of cards currently in the hand. </summary>
    public int Count => Cards.Count;

    /// <summary>Whether the hand is empty.</summary>
    public bool IsEmpty => Count == 0;

    /// <summary>Whether the hand is full.</summary>
    public bool IsFull => Count >= MAX_HAND_SIZE;

    public ImmutableHand(IEnumerable<Card> cards) : this([.. cards])
    {
    }
}
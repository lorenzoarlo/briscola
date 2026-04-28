namespace Briscola.Domain.Entities;

using Briscola.Domain.Enums;

/// <summary>Read-only snapshot of the game state.</summary>
public record GameSnapshot(
    Suit BriscolaSuit,
    Card? BriscolaCard,
    int DeckCount,
    IReadOnlyList<ImmutableTrick> Tricks,
    IReadOnlyList<Team> Teams
)
{
    /// <summary>
    /// Helper properties for easier access to current state
    /// </summary>
    public ImmutableTrick? CurrentTrick => Tricks.Count > 0 ? Tricks[^1] : null;
}

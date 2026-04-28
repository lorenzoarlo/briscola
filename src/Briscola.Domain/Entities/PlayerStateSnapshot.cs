namespace Briscola.Domain.Entities;


/// <summary>
/// Represents a snapshot of a state of a player during the game
/// </summary>
public record PlayerStateSnapshot(ImmutableHand Hand, IReadOnlyCollection<Card> CapturedCards)
{
}
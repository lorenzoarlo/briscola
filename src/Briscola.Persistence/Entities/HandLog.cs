using Briscola.Domain.Entities;

namespace Briscola.Persistence.Entities;

/// <summary>
/// Represents a log of a player's hand during a match
/// </summary>
/// <param name="Cards">List of cards in the player's hand</param>
public record HandLog(List<Card> Cards);
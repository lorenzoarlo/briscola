using Briscola.Domain.Entities;

namespace Briscola.Persistence.Entities;

/// <summary>
/// Represents a log of a trick played during a match
/// </summary>
/// <param name="SequentialId">Round's number</param>
/// <param name="Hands">Dictionary that associates each player to each hand</param>
/// <param name="CardsPlayed">Cards played in the trick</param>
/// <param name="Winner">Winner of the trick</param>
public record TrickLog(
    int SequentialId,
    Dictionary<string, HandLog> Hands,
    List<(string, Card)> CardsPlayed,
    PlayerLog Winner);
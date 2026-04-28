using System.Collections.Immutable;
using Briscola.Domain.Enums;

namespace Briscola.Domain.Entities;

/// <summary>
/// Represents an immutable snapshot of a trick, providing a read-only view of its current state.
/// </summary>
/// <param name="BriscolaSuit">The suit that is the briscola for this trick.</param>
/// <param name="PlayOrder">The order of players in this trick.</param>
/// <param name="CardsPlayed">The cards that have been played by each player so far.</param>
/// <param name="IsComplete">Indicates whether the trick is complete (all players have played their cards).</param>
/// <param name="CurrentPlayer">The player whose turn it is to play, or null if the trick is complete.</param>  
/// <param name="Winner">The player who won the trick, or null if the trick is not yet complete.</param>
public record ImmutableTrick(Suit BriscolaSuit, IReadOnlyCollection<Player> PlayOrder, IReadOnlyDictionary<Player, Card?> CardsPlayed, bool IsComplete, Player? CurrentPlayer, Player? Winner)
{

    /// <summary>Card played by a player.</summary>
    public Card? CardOfPlayer(Player player) => CardsPlayed.TryGetValue(player, out var card) ? card : null;
    
    public IReadOnlyList<Card> TrickCards => CardsPlayed.Values.Where(c => c is not null).Select(c => c!).ToList().AsReadOnly();


}
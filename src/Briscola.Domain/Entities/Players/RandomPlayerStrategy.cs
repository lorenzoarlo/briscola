namespace Briscola.Domain.Entities.Players;

/// <summary>
/// A simple player implementation that chooses a random card from their hand.
/// </summary>
public class RandomPlayerStrategy : PlayerStrategy
{
    public override string Type => "Random";

    /// <summary> Selects a random card from the player's hand to play. </summary>
    public override Task<Card> ChooseCardAsync(Player self, PlayerStateSnapshot state, GameSnapshot context, CancellationToken ct = default) => Task.FromResult(state.Hand.Cards.OrderBy(_ => Guid.NewGuid()).First());
}
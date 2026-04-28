using Briscola.Domain.Enums;

namespace Briscola.Domain.Entities;

/// <summary>
/// Player strategy
/// </summary>
public abstract class PlayerStrategy
{
    /// <summary>
    /// Type of the player strategy (e.g., "Human", "RandomAI", "MinimaxAI").
    /// </summary>
    public abstract string Type { get; }

    /// <summary>Selects a card to play from the hand.</summary>
    public abstract Task<Card> ChooseCardAsync(Player self, PlayerStateSnapshot state, GameSnapshot context, CancellationToken ct = default);

    /// <summary>Called when a trick starts.</summary>
    public virtual Task OnTrickStartedAsync(Player self, GameSnapshot context, CancellationToken ct = default) => Task.CompletedTask;

    /// <summary>Called when any player plays a card.</summary>
    public virtual Task OnCardPlayedAsync(Player self, Player player, Card card, GameSnapshot context, CancellationToken ct = default) => Task.CompletedTask;

    /// <summary>Called when a trick ends.</summary>
    public virtual Task OnTrickCompletedAsync(Player self, Player winner, IEnumerable<Card> cards, GameSnapshot context, CancellationToken ct = default) => Task.CompletedTask;

    /// <summary>Called when this player draws a card.</summary>
    public virtual Task OnCardDrawnAsync(Card card, GameSnapshot context, CancellationToken ct = default) => Task.CompletedTask;

    /// <summary>Called when the match ends.</summary>
    /// <param name="result">The result of the match</param>
    /// <param name="teamIndex">The index of this player's team (to identify which is the team 1 or team 2</param>
    public virtual Task OnMatchEndedAsync(
        GameResult result,
        int teamIndex,
        IReadOnlyDictionary<Player, int> scores,
        CancellationToken ct = default) => Task.CompletedTask;
}
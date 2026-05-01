using Briscola.Domain.Enums;

namespace Briscola.Domain.Entities;

/// <summary>
/// Represents a player in a Briscola match.
/// </summary>
/// <remarks>Initializes a new player.</remarks>
public sealed class Player(string id, string name, PlayerStrategy strategy) : IEquatable<Player>
{
    #region Properties

    /// <summary>Player's unique identifier.</summary>
    public string Id { get; } = id;

    /// <summary>Player's display name.</summary>
    public string Name { get; } = name;


    /// <summary>Identifier of the team the player belongs to.</summary>
    public string? TeamId { get; private set; } = null;

    /// <summary>
    /// Player's configuration, which may include preferences or settings for the match.
    /// </summary>
    public PlayerStrategy Strategy { get; } = strategy;

    /// <summary>Cards currently held by the player.</summary>
    private readonly Hand _hand = new();

    /// <summary> Exposes the player's hand as an immutable snapshot </summary>
    public ImmutableHand Hand => _hand.AsReadonly();

    /// <summary> Cards captured by the player during the match </summary>
    private readonly List<Card> _capturedCards = [];

    /// <summary> Current state of the player returned as immutable snapshot for use in strategy methods </summary>
    public PlayerStateSnapshot StateSnapshot => new(_hand.AsReadonly(), _capturedCards);

    /// <summary> Total points from captured cards, calculated as the sum of the points of each captured card </summary>
    public int Score => _capturedCards.Sum(c => c.Points);

    #endregion

    #region Constructor

    /// <summary>Initializes a new player with a team.</summary>
    public Player(string id, string name, string teamId, PlayerStrategy strategy) : this(id, name, strategy)
    {
        TeamId = teamId;
    }

    #endregion

    # region Game methods

    /// <summary>Adds a card to the player's hand.</summary>
    public void Draw(Card card) => _hand.Draw(card);


    /// <summary>Removes a card from the hand and plays it.</summary>
    public void Play(Card card) => _hand.Play(card);

    /// <summary>Joins the player to a team.</summary>
    public void JoinTeam(string teamId)
    {
        if (TeamId is not null)
        {
            throw new InvalidOperationException("Player is already in a team.");
        }
        TeamId = teamId;
    }

    /// <summary>Adds cards to the captured pile.</summary>
    public void CaptureCards(IEnumerable<Card> cards) => _capturedCards.AddRange(cards);

    #endregion

    #region Player interaction methods

    /// <summary>Selects a card to play from the hand.</summary>
    public Task<Card> ChooseCardAsync(GameSnapshot context, CancellationToken ct = default) => Strategy.ChooseCardAsync(this, StateSnapshot, context, ct);

    /// <summary>Called when a trick starts.</summary>
    public Task OnTrickStartedAsync(GameSnapshot context, CancellationToken ct = default) => Strategy.OnTrickStartedAsync(this, context, ct);

    /// <summary>Called when any player plays a card.</summary>
    public Task OnCardPlayedAsync(Player player, Card card, GameSnapshot context, CancellationToken ct = default) => Strategy.OnCardPlayedAsync(this, player, card, context, ct);

    /// <summary>Called when a trick ends.</summary>
    public Task OnTrickCompletedAsync(Player winner, IEnumerable<Card> cards, GameSnapshot context, CancellationToken ct = default) => Strategy.OnTrickCompletedAsync(this, winner, cards, context, ct);

    /// <summary>Called when this player draws a card.</summary>
    public Task OnCardDrawnAsync(Card card, GameSnapshot context, CancellationToken ct = default) => Strategy.OnCardDrawnAsync(card, context, ct);

    /// <summary>Called when the match ends.</summary>
    public Task OnMatchEndedAsync(
        GameResult result,
        int teamIndex,
        IReadOnlyDictionary<Player, int> scores,
        CancellationToken ct = default) => Strategy.OnMatchEndedAsync(result, teamIndex, scores, ct);

    #endregion

    #region Equality
    /// <summary>Compares players by their unique identifier.</summary>
    public bool Equals(Player? other) => other is not null && Id == other.Id;

    /// <summary>Compares players by their unique identifier.</summary>
    public override bool Equals(object? o)
    {
        // if other is null or not a Player, return false
        if (o == null) { return false; }
        return o is Player player && Equals(player);
    }

    /// <summary>Returns the hash code of the player's identifier.</summary>
    public override int GetHashCode() => Id.GetHashCode();

    # endregion


}

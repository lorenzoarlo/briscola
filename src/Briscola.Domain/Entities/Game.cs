using Briscola.Domain.Enums;

namespace Briscola.Domain.Entities;

/// <summary>
/// Orchestrates a full Briscola match: dealing, turn management, trick resolution, and scoring.
/// Supports 2 or 4 players (4-player variant uses teams).
/// </summary>
public abstract class Game
{
    // List of teams
    protected readonly List<Team> _teams;
    // List of players 
    protected readonly List<Player> _players;

    // Deck of cards for the match
    protected readonly Deck _deck;

    // List of tricks
    protected List<Trick> _tricks = [];

    // Index of the player who leads the current trick
    private int _firstPlayerIndex = 0;

    /// <summary>The trump suit for this match, determined by the last card drawn.</summary>
    public Suit BriscolaSuit { get; private set; }

    /// <summary>The trump card. Is null when it's picked up at the end of the game</summary>
    public Card? BriscolaCard { get; private set; }

    /// <summary>Current game state.</summary>
    public GameState State { get; private set; } = GameState.WaitingForPlayers;

    /// <summary>Teams in the match.</summary>
    public IReadOnlyList<Team> Teams => _teams.AsReadOnly();

    /// <summary>Players in turn order.</summary>
    public IReadOnlyList<Player> Players => _players.AsReadOnly();

    /// <summary>The trick currently in progress, or null at the beginning</summary>
    public Trick? CurrentTrick => _tricks.LastOrDefault();

    /// <summary>
    /// Returns the player whose turn it currently is, or null
    /// </summary>
    public Player? CurrentPlayer => CurrentTrick?.CurrentPlayer;

    /// <summary>Initializes a new match.</summary>
    protected Game(IEnumerable<Team> teams, IEnumerable<Player> players)
    {
        _teams = [.. teams];
        _players = [.. players];

        ValidatePlayers(_players, _teams);

        _deck = new Deck();
        BriscolaCard = _deck.Draw();
        BriscolaSuit = BriscolaCard.Suit;
    }

    /// <summary>Whether the match is over.</summary>
    public bool IsMatchOver => _deck.IsEmpty && _players.All(p => p.StateSnapshot.Hand.IsEmpty);


    /// <summary>Deals initial cards and starts the first trick.</summary>
    public async Task DealAsync(CancellationToken ct = default)
    {
        State = GameState.Dealing;
        for (int i = 0; i < ImmutableHand.MAX_HAND_SIZE; i++)
        {
            foreach (var player in _players)
            {
                var card = _deck.Draw();
                player.Draw(card);
                await player.OnCardDrawnAsync(card, Snapshot(), ct);
            }
        }

        State = GameState.PlayerTurn;
        await StartNewTrickAsync(ct);
    }

    /// <summary>Runs the main game loop.</summary>
    public async Task RunAsync(CancellationToken ct = default)
    {
        await DealAsync(ct);
        while (State != GameState.MatchEnd)
        {
            var player = CurrentPlayer!;
            var card = await player.ChooseCardAsync(Snapshot(), ct);
            await PlayCardAsync(card, ct);
        }
    }


    /// <summary>Plays a card for the current player.</summary>
    public async Task PlayCardAsync(Card card, CancellationToken ct = default)
    {
        EnsureState(GameState.PlayerTurn);

        var player = CurrentPlayer ?? throw new InvalidOperationException("No current player.");
        var trick = CurrentTrick ?? throw new InvalidOperationException("No current trick.");

        player.Play(card);
        trick.PlayCard(player, card);

        var context = Snapshot();
        foreach (var p in _players)
        {
            await p.OnCardPlayedAsync(player, card, context, ct);
        }

        if (trick.IsComplete)
        {
            await ResolveTrickAsync(ct);
        }
    }

    /// <summary>Resolves the current trick and awards points.</summary>
    private async Task ResolveTrickAsync(CancellationToken ct = default)
    {
        State = GameState.TrickEnd;

        var trick = CurrentTrick!;
        var winner = trick.Winner()!;
        var cards = trick.TrickCards;

        winner.CaptureCards(cards);

        var context = Snapshot();
        foreach (var p in _players)
        {
            await p.OnTrickCompletedAsync(winner, cards, context, ct);
        }

        _firstPlayerIndex = _players.IndexOf(winner);

        await DrawPhaseAsync(ct);

        if (IsMatchOver)
        {
            await EndMatchAsync(ct);
        }
        else
        {
            State = GameState.PlayerTurn;
            await StartNewTrickAsync(ct);
        }
    }

    /// <summary>Handles drawing cards after a trick.</summary>
    private async Task DrawPhaseAsync(CancellationToken ct = default)
    {
        if (_deck.IsEmpty) return;

        var drawOrder = OrderedPlayersForTrick();
        foreach (var player in drawOrder)
        {
            var card = _deck.IsEmpty ? DrawBriscolaCard() : _deck.Draw();
            player.Draw(card);
            await player.OnCardDrawnAsync(card, Snapshot(), ct);
        }
    }


    /// <summary>Starts a new trick.</summary>
    private async Task StartNewTrickAsync(CancellationToken ct = default)
    {
        var ordered = OrderedPlayersForTrick();
        _tricks.Add(new Trick(ordered, BriscolaSuit));

        var snapshot = Snapshot();
        foreach (var p in _players)
        {
            await p.OnTrickStartedAsync(snapshot, ct);
        }
    }



    /// <summary>Ends the match and notifies players.</summary>
    private async Task EndMatchAsync(CancellationToken ct = default)
    {
        State = GameState.MatchEnd;
        var result = Result();
        var scores = _players.ToDictionary(p => p, p => p.Score);
        foreach (var p in _players)
        {
            await p.OnMatchEndedAsync(result, TeamIndexOf(p), scores, ct);
        }
    }

    /// <summary>Calculates the result of the match.</summary>
    public GameResult Result()
    {
        EnsureState(GameState.MatchEnd);
        var s1 = _teams[0].Score;
        var s2 = _teams[1].Score;
        if (s1 > s2) return GameResult.WinTeam1;
        if (s2 > s1) return GameResult.WinTeam2;
        return GameResult.Tie;
    }

    /// <summary>Gets the index of the team a player belongs to.</summary>
    protected int TeamIndexOf(Player p) => _teams.FindIndex(t => t.Players.Contains(p));

    /// <summary>Validates that the player set and teams set is well-formed for this variant.</summary>
    protected abstract void ValidatePlayers(IReadOnlyList<Player> players, IReadOnlyList<Team> teams);

    /// <summary>Draws the face-up briscola card.</summary>
    private Card DrawBriscolaCard()
    {
        if (BriscolaCard is null)
            throw new InvalidOperationException("Briscola card has already been drawn.");

        var card = BriscolaCard;
        BriscolaCard = null;
        return card;
    }

    /// <summary>Orders players for the next trick.</summary>
    private List<Player> OrderedPlayersForTrick() => [.. _players.Skip(_firstPlayerIndex).Concat(_players).Take(_players.Count)];

    /// <summary>Creates a state snapshot.</summary>
    private GameSnapshot Snapshot() => new(
        BriscolaSuit,
        BriscolaCard,
        _deck.Count,
        _tricks.Select(t => t.AsReadonly()).ToList().AsReadOnly(),
        _teams
    );

    /// <summary>Ensures the game is in the expected state.</summary>
    protected void EnsureState(GameState expected)
    {
        if (State != expected)
            throw new InvalidOperationException(
                $"Expected state {expected}, but current state is {State}.");
    }
}
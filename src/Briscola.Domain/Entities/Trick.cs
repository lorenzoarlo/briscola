using Briscola.Domain.Enums;

namespace Briscola.Domain.Entities;

/// <summary>Represents a trick in a match.</summary>
public class Trick
{

    /// <summary>Players participating in the trick.</summary>
    private readonly List<Player> _players;

    /// <summary>Cards played by each player.</summary>
    private readonly Dictionary<Player, Card?> _cardsPlayed;

    /// <summary>Current player index.</summary>
    private int _currentPlayerIndex = 0;

    /// <summary>Briscola suit for the trick.</summary>
    public Suit BriscolaSuit { get; }

    /// <summary>Initializes a new trick.</summary>
    public Trick(IEnumerable<Player> players, Suit briscolaSuit)
    {
        // Convert the IEnumerable to a List for easier indexing and management of players during the trick
        _players = [.. players];
        // Initialize the dictionary to track cards played by each player, starting with null for each player since no cards have been played yet
        _cardsPlayed = _players.ToDictionary(player => player, player => (Card?)null);
        BriscolaSuit = briscolaSuit;
    }

    /// <summary>Plays a card for a player.</summary>
    public void PlayCard(Player player, Card card)
    {
        if (!player.Equals(_players[_currentPlayerIndex]))
        {
            throw new InvalidOperationException("It's not this player's turn to play.");
        }
        if (_cardsPlayed[player] is not null)
        {
            throw new InvalidOperationException("Player has already played a card in this trick.");
        }
        _cardsPlayed[player] = card;
        _currentPlayerIndex++;
    }

    /// <summary>Current player whose turn it is.</summary>
    public Player? CurrentPlayer => IsComplete ? null : _players[_currentPlayerIndex];

    /// <summary>Whether the trick is complete.</summary>
    public bool IsComplete => _cardsPlayed.Values.All(c => c is not null);

    /// <summary>Cards played in the trick.</summary>
    public IReadOnlyList<Card> TrickCards => _cardsPlayed.Values.Where(c => c is not null).Select(c => c!).ToList().AsReadOnly();

    /// <summary>Plays a card for the current player.</summary>
    public void PlayCard(Card card) => PlayCard(_players[_currentPlayerIndex], card);

    /// <summary>Card played by a player.</summary>
    public Card? CardOfPlayer(Player player) => _cardsPlayed.TryGetValue(player, out var card) ? card : null;

    /// <summary>Determines the trick winner.</summary>
    public Player Winner()
    {
        if (!IsComplete)
        {
            throw new InvalidOperationException("Cannot determine winner of an incomplete trick.");
        }

        // Start by assuming the first player is the winner
        Player? winner = _players[0];
        Card winningCard = CardOfPlayer(winner)!;

        for (int i = 1; i < _currentPlayerIndex; i++)
        {
            Player currentPlayer = _players[i];
            Card currentCard = CardOfPlayer(currentPlayer)!;

            // If the current card beats the winning card according to Briscola rules, update the winner and winning card
            if (!BriscolaBeats(winningCard, currentCard, BriscolaSuit))
            {
                winner = currentPlayer;
                winningCard = currentCard;
            }
        }

        return winner;
    }

    public ImmutableTrick AsReadonly() => new(BriscolaSuit, _players.AsReadOnly(), _cardsPlayed.AsReadOnly(), IsComplete, CurrentPlayer, IsComplete ? Winner() : null);

    /// <summary>Determines if a card beats another.</summary>
    public static bool BriscolaBeats(Card first, Card second, Suit briscolaSuit)
    {
        // If they have the same suit, wins the one with the higher value
        if (first.Suit == second.Suit)
        {
            // we use >= to keep the first player as winner in case of tie (even if in Briscola there are no ties, this is just a safeguard)
            return first.Value >= second.Value; ;
        }
        // If the challenger is of the Briscola suit and the opponent is not, the challenger wins
        if (first.Suit == briscolaSuit)
        {
            return true;
        }
        // If the opponent is of the Briscola suit and the challenger is not, the opponent wins
        if (second.Suit == briscolaSuit)
        {
            return false;
        }
        //different suits, neither is briscola -> first (leading) card wins
        return true;

    }

}
namespace Briscola.Domain.Entities;

/// <summary>
/// Represents a team in the Briscola game, containing players and captured cards.
/// </summary>
/// <remarks>Initializes a new team.</remarks>
public class Team(string id, string name)
{

    /// <summary>Team unique identifier.</summary>
    public string Id { get; } = id;

    /// <summary>Team display name.</summary>
    public string Name { get; } = name;

    /// <summary> List of players in the team </summary>
    private readonly List<Player> _players = [];

    /// <summary>Initializes a new team with players.</summary>
    public Team(string id, string name, IEnumerable<Player> players) : this(id, name)
    {
        foreach (var player in players)
        {
            Join(player);
        }
    }

    /// <summary>Adds a player to the team.</summary>
    public void Join(Player player)
    {
        player.JoinTeam(Id);
        _players.Add(player);
    }


    /// <summary>Players in the team.</summary>
    public IReadOnlyCollection<Player> Players => _players.AsReadOnly();

    /// <summary>All cards captured by players in this team.</summary>
    public IReadOnlyCollection<Card> CapturedCards() => [.. Players.SelectMany(p => p.StateSnapshot.CapturedCards)];
    /// <summary>Total team score, summed from all players' captured cards.</summary>
    public int Score => _players.Sum(p => p.Score);

}

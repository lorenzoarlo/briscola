using Briscola.Domain.Entities;
using Briscola.Domain.Entities.Games;

namespace Briscola.Controller.GameHub;

public sealed class GameHub(PlayerStrategyFactory Factory)
{

    public PlayerStrategyFactory Factory { get; } = Factory;

    private int _playerCounter = 0;

    /// <summary>Builds and runs a 1v1 match. Returns the completed Game for inspection.</summary>
    public async Task<Game> StartGame1V1Async(
        PlayerConfig a, PlayerConfig b, CancellationToken ct = default)
    {
        var playerA = new Player(NextPlayerId(), a.DisplayName, Factory.Of(a.StrategyType));
        var playerB = new Player(NextPlayerId(), b.DisplayName, Factory.Of(b.StrategyType));
        var game = new Game1v1(playerA, playerB);
        await game.RunAsync(ct);
        return game;
    }

    /// <summary>Builds and runs a 2v2 match. Player order: teamA[0], teamB[0], teamA[1], teamB[1].</summary>
    public Task<Game> StartGame2V2Async((PlayerConfig, PlayerConfig) teamA, (PlayerConfig, PlayerConfig) teamB, CancellationToken ct = default)
    {
        return Task.FromException<Game>(new NotImplementedException("2v2 support is planned but not yet implemented."));
    }

    private string NextPlayerId() => $"player-{Interlocked.Increment(ref _playerCounter)}";
}

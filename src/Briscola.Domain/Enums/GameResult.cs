namespace Briscola.Domain.Enums;

/// <summary>
/// Represents the possible results of a Briscola game.
/// </summary>
public enum GameResult
{
    /// <summary>
    /// The match is still ongoing and has not yet reached a conclusion.
    /// </summary>
    NotEnded,
    /// <summary>Team 1 wins the match.</summary>
    WinTeam1,
    /// <summary>The match ends in a tie.</summary>
    Tie,
    /// <summary>Team 2 wins the match.</summary>
    WinTeam2
}

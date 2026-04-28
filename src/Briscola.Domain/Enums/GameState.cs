namespace Briscola.Domain.Enums;

/// <summary>
/// Represents the current state of the game. 
/// This can be used to determine what actions are allowed and what the next steps are in the game flow.
/// </summary>
public enum GameState
{
    /// <summary>
    /// Indicates that the game is waiting for players to join before it can start. 
    /// In this state, players can connect and register to participate in the match, but the game has not yet begun. 
    /// Once the required number of players have joined, the game can transition to the next state
    /// </summary>
    WaitingForPlayers,
    /// <summary>
    /// Indicates that the game is currently in the dealing phase, where players are being dealt their initial hands of cards from the deck.
    /// </summary>
    Dealing,
    /// <summary>
    /// Indicates that the game is currently in the player turn phase, where players are taking their turns to play cards from their hands and compete in tricks. 
    /// </summary>
    PlayerTurn,
    /// <summary>
    /// Indicates that the current trick has ended
    /// </summary>
    TrickEnd,

    /// <summary>
    /// Indicates that the match has ended
    /// </summary>
    MatchEnd
}

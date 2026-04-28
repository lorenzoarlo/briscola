using Briscola.Domain.Entities;
using Briscola.Domain.Entities.Players;

namespace Briscola.Controller.GameHub;

/// <summary>
/// Represents a type of player strategy, such as "Random", "MinimaxAI"
/// It is used as a "smart enum" to define the available strategies in a type-safe way and allow easy extension
/// Here the predefined strategy types are defined, but new ones can be added as needed without changing the existing code, just by creating new instances of StrategyType that inherit from it, and adding them to the factory's available strategies list.
/// With enums it is not possible inheritance  
/// </summary>
/// <param name="Name">Displayed name</param>
public record StrategyType(string Name, Func<PlayerStrategy> Factory)
{
    // Predefined strategy types
    public static readonly StrategyType Random = new("Random", () => new RandomPlayerStrategy());
}

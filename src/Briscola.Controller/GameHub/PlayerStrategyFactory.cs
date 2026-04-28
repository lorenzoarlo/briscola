using Briscola.Domain.Entities;

namespace Briscola.Controller.GameHub;

public class PlayerStrategyFactory
{

    public static IReadOnlyCollection<StrategyType> DefaultSupportedStrategies => [
        StrategyType.Random
    ];

    /// <summary>
    /// A list of all supported strategy types. Used for validation and UI purposes.
    /// </summary>
    public virtual IReadOnlyCollection<StrategyType> SupportedStrategies { get; } = DefaultSupportedStrategies;

    /// <summary>
    /// Creates a PlayerStrategy instance based on the provided strategy type.
    /// </summary>
    /// <param name="strategyType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public PlayerStrategy Of(StrategyType strategyType)
    {
        if (!SupportedStrategies.Contains(strategyType)) throw new ArgumentException($"Unsupported strategy type: {strategyType.Name}");
        return strategyType.Factory();
    }
};


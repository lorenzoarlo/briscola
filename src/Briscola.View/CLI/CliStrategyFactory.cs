using Briscola.Controller.GameHub;

namespace Briscola.View.CLI;

public class CliStrategyFactory : PlayerStrategyFactory
{
    private static IReadOnlyCollection<StrategyType> CliSupportedStrategies =>
    [
        CliStrategyType.CliHumanPlayer
    ];

    public sealed override IReadOnlyCollection<StrategyType> SupportedStrategies =>
        [.. base.SupportedStrategies.Concat(CliSupportedStrategies)];
}
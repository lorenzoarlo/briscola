using Briscola.Controller.GameHub;
using Briscola.Domain.Entities;
using Briscola.View.CLI.Players;


namespace Briscola.View.CLI;

public class CLIStrategyFactory : PlayerStrategyFactory
{
    public static IReadOnlyCollection<StrategyType> CLISupportedStrategies => [
        CLIStrategyType.CLIHumanPlayer
    ];
    public override IReadOnlyCollection<StrategyType> SupportedStrategies => [.. base.SupportedStrategies.Concat(CLISupportedStrategies)];

}
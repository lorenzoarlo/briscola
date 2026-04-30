using Briscola.Controller.GameHub;
using Briscola.Domain.Entities;
using Briscola.View.CLI.Players;


namespace Briscola.View.CLI;

public class CliStrategyFactory : PlayerStrategyFactory
{
    public static IReadOnlyCollection<StrategyType> CLISupportedStrategies => [
        CliStrategyType.CliHumanPlayer
    ];
    public override IReadOnlyCollection<StrategyType> SupportedStrategies => [.. base.SupportedStrategies.Concat(CLISupportedStrategies)];

}
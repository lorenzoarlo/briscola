using Briscola.Controller.GameHub;
using Briscola.Domain.Entities;
using Briscola.View.CLI.Players;

namespace Briscola.View.CLI;

public record CliStrategyType(string Name, Func<PlayerStrategy> Factory) : StrategyType(Name, Factory)
{
    public static readonly CliStrategyType CliHumanPlayer = new("CLIHumanPlayer", () => new CliHumanPlayerStrategy());
}
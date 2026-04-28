using Briscola.Controller.GameHub;
using Briscola.Domain.Entities;
using Briscola.View.CLI.Players;

namespace Briscola.View.CLI;

public record CLIStrategyType : StrategyType
{
    public static readonly CLIStrategyType CLIHumanPlayer = new("CLIHumanPlayer", () => new CLIHumanPlayerStrategy());

    public CLIStrategyType(string Name, Func<PlayerStrategy> Factory) : base(Name, Factory)
    {
    }
}
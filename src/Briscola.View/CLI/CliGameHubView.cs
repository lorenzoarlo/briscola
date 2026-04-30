using System.Text;
using Briscola.Controller.GameHub;
using Briscola.View.CLI.Io;
using Briscola.View.Resources;

namespace Briscola.View.CLI;

/// <summary>
/// CLI Application Hub that serves as the entry point for starting new Briscola matches.
/// </summary>
public class CliGameHubView
{
    private readonly GameHub _gameHub = new(new CliStrategyFactory());

    private bool _isRunning = true;

    /// <summary>
    /// Starts the main application loop, displaying the main menu and handling user choices.
    /// </summary>
    /// <param name="ct">Cancellation token to gracefully stop the application.</param>
    public async Task RunAsync(CancellationToken ct = default)
    {
        while (_isRunning && !ct.IsCancellationRequested)
        {
            Console.Clear();
            RenderHeader();
            
            List<(string, Func<CancellationToken, Task>)> options =
            [
                (Messages.Menu_Start1v1, Start1V1SetupAsync),
                (Messages.Menu_Quit, QuitAsync)
            ];
            
            var builder =  new StringBuilder();
            for (var i = 0; i < options.Count; i++)
            {
                builder.AppendLine($"{i + 1}. {options.ElementAt(i).Item1}");
            }
            builder.Append(Messages.Menu_ChooseOption);
            
            var choice = AskForChoice(builder.ToString(), 1,  options.Count);
            await options.ElementAt(choice - 1).Item2(ct);
        }
    }


    /// <summary>
    /// Handles the setup flow for a 1v1 match, prompting for player configurations and starting the game.
    /// </summary>
    private async Task Start1V1SetupAsync(CancellationToken ct)
    {
        Console.Clear();
        Console.WriteLine(IoUtilities.Colorize(Messages.Setup_1v1Header, AnsiColors.FgCyan));
        Console.WriteLine();

        var player1Config = PromptForPlayerConfig(1);
        Console.WriteLine();
        Console.Clear();
        var player2Config = PromptForPlayerConfig(2);

        Console.Clear();
        Console.WriteLine(IoUtilities.Colorize(Messages.Setup_StartingGame, AnsiColors.FgGreen));

        // Let the controller build and run the match
        await _gameHub.StartGame1v1Async(player1Config, player2Config, ct);

        // After the game loop is fully complete, wait for the user to acknowledge before returning to the main menu
        Console.WriteLine();
        Console.WriteLine(IoUtilities.Colorize(Messages.Msg_PressEnterToReturn, AnsiColors.FgCyan));
        Console.ReadLine();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ct"></param>
    private Task QuitAsync(CancellationToken ct)
    {
        _isRunning = false;
        Console.WriteLine(IoUtilities.Colorize(Messages.Msg_Goodbye, AnsiColors.FgGreen));
        return Task.CompletedTask;
    }

    /// <summary>
    /// Prompts the user to configure a player, requesting a name and a strategy type.
    /// </summary>
    /// <param name="playerNumber">The sequential number of the player being configured.</param>
    /// <returns>A configured PlayerConfig instance.</returns>
    private PlayerConfig PromptForPlayerConfig(int playerNumber)
    {
        Console.WriteLine(Messages.Setup_ConfiguringPlayer, playerNumber);

        // Ask for name
        Console.Write(Messages.Setup_EnterName);
        var name = Console.ReadLine() ?? string.Empty;
        if (string.IsNullOrEmpty(name))
        {
            name = string.Format(Messages.Setup_DefaultPlayerName, playerNumber);
        }

        // supported strategies
        var strategies = _gameHub.Factory.SupportedStrategies;

        var builder = new StringBuilder();
        builder.AppendLine(Messages.Setup_ChooseStrategy);
        for (var i = 0; i < strategies.Count; i++)
        {
            builder.AppendLine($"\t{i + 1}. {strategies.ElementAt(i).Name}");
        }

        var choice =
            AskForChoice(builder.ToString(), 1, strategies.Count); // Reuse the choice utility for better validation
        var selectedStrategy = strategies.ElementAt(choice - 1);

        return new PlayerConfig(name, selectedStrategy);
    }
    
    /// <summary>
    /// Renders the main menu header.
    /// </summary>
    private static void RenderHeader()
    {
        Console.WriteLine(IoUtilities.Colorize("Welcome to the Briscola CLI!", AnsiColors.FgBrightMagenta));
        Console.WriteLine();
    }
    
    /// <summary>
    /// Utility method to ask the user for a number within a specified range, with validation and error feedback.
    /// </summary>
    /// <param name="prompt"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    private static int AskForChoice(string prompt, int min, int max)
    {
        var ok = false;
        while (!ok)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();

            ok = int.TryParse(input, out var number);
            
            if (ok && number >= min && number <= max)
            {
                return number;
            }
            Console.WriteLine(IoUtilities.Colorize(Messages.Msg_InvalidChoice, AnsiColors.FgBrightRed));
        }

        return -1;
    }

}
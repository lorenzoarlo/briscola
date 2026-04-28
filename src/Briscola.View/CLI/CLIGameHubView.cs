using Briscola.Controller.GameHub;
using Briscola.View.CLI.Io;

namespace Briscola.View.CLI;

/// <summary>
/// CLI Application Hub that serves as the entry point for starting new Briscola matches.
/// </summary>
public class CLIGameHubView
{
    private readonly GameHub _gameHub;

    public CLIGameHubView()
    {

        _gameHub = new GameHub(new CLIStrategyFactory());
    }

    /// <summary>
    /// Starts the main application loop, displaying the main menu and handling user choices.
    /// </summary>
    /// <param name="ct">Cancellation token to gracefully stop the application.</param>
    public async Task RunAsync(CancellationToken ct = default)
    {
        bool isRunning = true;

        while (isRunning && !ct.IsCancellationRequested)
        {
            Console.Clear();
            RenderHeader();

            Console.WriteLine("1. Start 1v1 Match");
            Console.WriteLine("2. Start 2v2 Match");
            Console.WriteLine("3. Quit");
            Console.WriteLine();
            Console.Write("Choose an option: ");

            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    await Start1v1SetupAsync(ct);
                    break;
                case "2":
                    ShowNotImplemented("2v2 Match");
                    break;
                case "3":
                    isRunning = false;
                    Console.WriteLine(IoUtilities.Colorize("Goodbye! :)", AnsiColors.FgGreen));
                    break;
                default:
                    Console.WriteLine(IoUtilities.Colorize("Invalid choice. Please press Enter and try again.", AnsiColors.FgBrightRed));
                    Console.ReadLine();
                    break;
            }
        }
    }

    /// <summary>
    /// Handles the setup flow for a 1v1 match, prompting for player configurations and starting the game.
    /// </summary>
    private async Task Start1v1SetupAsync(CancellationToken ct)
    {
        Console.Clear();
        Console.WriteLine(IoUtilities.Colorize("--- 1v1 Match Setup ---", AnsiColors.FgCyan));
        Console.WriteLine();

        var player1Config = PromptForPlayerConfig(1);
        Console.WriteLine();
        var player2Config = PromptForPlayerConfig(2);

        Console.Clear();
        Console.WriteLine(IoUtilities.Colorize("Starting game...", AnsiColors.FgGreen));

        // Let the controller build and run the match
        var game = await _gameHub.StartGame1v1Async(player1Config, player2Config, ct);

        // After the game loop is fully complete, wait for the user to acknowledge before returning to the main menu
        Console.WriteLine();
        Console.WriteLine(IoUtilities.Colorize("Press Enter to return to the main menu...", AnsiColors.FgCyan));
        Console.ReadLine();
    }

    /// <summary>
    /// Prompts the user to configure a player, requesting a name and a strategy type.
    /// </summary>
    /// <param name="playerNumber">The sequential number of the player being configured.</param>
    /// <returns>A configured PlayerConfig instance.</returns>
    private PlayerConfig PromptForPlayerConfig(int playerNumber)
    {
        Console.WriteLine($"Configuring Player {playerNumber}:");

        // 1. Prompt for Name
        Console.Write("  Enter name (leave blank for default): ");
        string name = Console.ReadLine()?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(name))
        {
            name = $"Player {playerNumber}";
        }

        // 2. Prompt for Strategy
        var strategies = _gameHub.Factory.SupportedStrategies;
        var selectedStrategy = StrategyType.Random; // Fallback default
        bool validStrategy = false;

        while (!validStrategy)
        {
            Console.WriteLine("  Choose strategy:");
            for (int i = 0; i < strategies.Count; i++)
            {
                Console.WriteLine($"    {i + 1}. {strategies.ElementAt(i).Name}");
            }

            Console.Write("  Choice: ");
            var input = Console.ReadLine();

            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= strategies.Count)
            {
                selectedStrategy = strategies.ElementAt(choice - 1);
                validStrategy = true;
            }
            else
            {
                Console.WriteLine(IoUtilities.Colorize("  Invalid choice. Please select a valid strategy number.", AnsiColors.FgBrightRed));
            }
        }

        return new PlayerConfig(name, selectedStrategy);
    }

    /// <summary>
    /// Displays a generic "not implemented" message for features that are not yet available.
    /// </summary>
    private void ShowNotImplemented(string featureName)
    {
        Console.Clear();
        Console.WriteLine(IoUtilities.Colorize($"{featureName} is planned but not yet implemented.", AnsiColors.FgMagenta));
        Console.WriteLine("Press Enter to return to the main menu...");
        Console.ReadLine();
    }

    /// <summary>
    /// Renders the main menu header.
    /// </summary>
    private static void RenderHeader()
    {
        Console.WriteLine(IoUtilities.Colorize("====================================", AnsiColors.FgCyan));
        Console.WriteLine(IoUtilities.Colorize("           BRISCOLA CLI             ", AnsiColors.FgBrightGreen));
        Console.WriteLine(IoUtilities.Colorize("====================================", AnsiColors.FgCyan));
        Console.WriteLine();
    }
}
using System.Globalization;
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

    // Hold our current configuration
    private HubConfig _config = new(null);

    /// <summary>
    /// Starts the main application loop, displaying the main menu and handling user choices.
    /// </summary>
    public async Task RunAsync(CancellationToken ct = default)
    {
        while (_isRunning && !ct.IsCancellationRequested)
        {
            Console.Clear();
            RenderHeader();

            List<(string, Func<CancellationToken, Task>)> options =
            [
                (Messages.Menu_Start1v1, Start1V1SetupAsync),
                (Messages.Menu_StartHumanvCpuPreset,
                    token => Start1V1PresetAsync(CliStrategyType.CliHumanPlayer, StrategyType.Random, token)),
                (Messages.Menu_Settings, SettingsMenuAsync), // Settings Menu
                (Messages.Menu_Quit, QuitAsync)
            ];

            var builder = new StringBuilder();
            for (var i = 0; i < options.Count; i++)
            {
                builder.AppendLine($"{i + 1}. {options.ElementAt(i).Item1}");
            }

            builder.Append(Messages.Menu_ChooseOption);

            var choice = AskForChoice(builder.ToString(), 1, options.Count);
            await options.ElementAt(choice - 1).Item2(ct);
        }
    }

    /// <summary>
    /// Handles the Settings sub-menu loop.
    /// </summary>
    private async Task SettingsMenuAsync(CancellationToken ct)
    {
        var inSettings = true;
        while (inSettings && !ct.IsCancellationRequested)
        {
            Console.Clear();
            Console.WriteLine(IoUtilities.Colorize(Messages.Menu_Settings, AnsiColors.FgCyan));
            Console.WriteLine();

            List<(string, Func<Task>)> options =
            [
                (string.Format(Messages.Settings_LanguageFormat, _config.Language), ChangeLanguageAsync),
                (string.Format(Messages.Settings_FolderFormat, _config.FolderPath ?? Messages.Settings_FolderNotSet), ChangeFolderAsync),
                (Messages.Menu_Back, () =>
                {
                    inSettings = false;
                    return Task.CompletedTask;
                })
            ];

            var builder = new StringBuilder();
            for (var i = 0; i < options.Count; i++)
            {
                builder.AppendLine($"{i + 1}. {options.ElementAt(i).Item1}");
            }

            builder.Append(Messages.Menu_ChooseOption);

            var choice = AskForChoice(builder.ToString(), 1, options.Count);
            await options.ElementAt(choice - 1).Item2();
        }
    }

    /// <summary>
    /// Displays a combobox-style prompt to change the localization.
    /// </summary>
    private Task ChangeLanguageAsync()
    {
        Console.Clear();
        Console.WriteLine(IoUtilities.Colorize(Messages.Settings_SelectLanguage, AnsiColors.FgCyan));

        var languages = Enum.GetValues<AppLanguage>();
        var builder = new StringBuilder();

        for (int i = 0; i < languages.Length; i++)
        {
            builder.AppendLine($"{i + 1}. {languages[i]} {(languages[i] == _config.Language ? "•" : "")}");
        }

        builder.Append(Messages.Menu_ChooseOption);

        var choice = AskForChoice(builder.ToString(), 1, languages.Length);
        var selectedLanguage = languages[choice - 1];

        // Update config and apply the localization
        _config = _config with { Language = selectedLanguage };
        ApplyLanguage(selectedLanguage);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Applies the selected culture to the current thread and resource manager.
    /// </summary>
    private void ApplyLanguage(AppLanguage language)
    {
        var cultureCode = language switch
        {
            AppLanguage.Italian => "it",
            _ => "en" // Invariant/Default fallback
        };

        var culture = new CultureInfo(cultureCode);

        // Update thread cultures
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        // Explicitly update the auto-generated resource class
        Messages.Culture = culture;
    }

    /// <summary>
    /// Prompts for a new folder path, validates write permissions, and updates config if successful.
    /// </summary>
    private Task ChangeFolderAsync()
    {
        Console.Clear();
        Console.WriteLine(IoUtilities.Colorize(Messages.ChangeFolder_EditPrompt, AnsiColors.FgCyan));
        Console.Write(Messages.ChangeFolder_NewFolder,
            (_config.FolderPath is not null ? $" \"{_config.FolderPath}\"" : ""));

        var path = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(path))
        {
            _config = _config with { FolderPath = null };
            Console.WriteLine(IoUtilities.Colorize(Messages.ChangeFolder_FolderCleared, AnsiColors.FgGreen));
            PauseForUser();
            return Task.CompletedTask;
        }

        try
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // Test write permissions by creating and deleting a temporary file
            var testFile = Path.Combine(path, $".testwrite_{Guid.NewGuid()}");
            File.WriteAllText(testFile, "test");
            File.Delete(testFile);

            // If we reach here, we have write access
            _config = _config with { FolderPath = path };
            Console.WriteLine(IoUtilities.Colorize(Messages.ChangeFolder_Success, AnsiColors.FgGreen));
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or IOException or NotSupportedException
                                       or ArgumentException)
        {   
            Console.WriteLine(IoUtilities.Colorize(string.Format(Messages.ChangeFolder_ErrorFormat, path, ex.Message),
                AnsiColors.FgBrightRed));
        }

        PauseForUser();
        return Task.CompletedTask;
    }

    private static void PauseForUser()
    {
        Console.WriteLine();
        Console.WriteLine(IoUtilities.Colorize(Messages.Msg_PressEnterToReturn, AnsiColors.FgCyan));
        Console.ReadLine();
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
        await _gameHub.StartGame1V1Async(player1Config, player2Config, ct);

        PauseForUser();
    }

    /// <summary>
    /// Handles the setup flow for a 1v1 match, prompting for player configurations and starting the game.
    /// </summary>
    private async Task Start1V1PresetAsync(StrategyType a, StrategyType b, CancellationToken ct)
    {
        // Randomly assign first config
        var first = Random.Shared.Next(2) == 1;

        var player1Config = new PlayerConfig("Player 1", first ? a : b);
        var player2Config = new PlayerConfig("Player 2", first ? b : a);

        // Let the controller build and run the match
        await _gameHub.StartGame1V1Async(player1Config, player2Config, ct);

        PauseForUser();
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
namespace Briscola.Controller.GameHub;

/// <summary>
/// Available application languages.
/// </summary>
public enum AppLanguage
{
    Default = 1,
    Italiano = 2
}


/// <summary>
/// Configuration for the game hub.
/// </summary>
public record HubConfig(string? FolderPath = HubConfig.DefaultFolderPath, AppLanguage Language = AppLanguage.Default)
{
    public const string DefaultFolderPath = "logs";
}
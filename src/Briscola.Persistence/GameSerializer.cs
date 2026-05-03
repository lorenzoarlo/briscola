using System.Text.Json;
using Briscola.Domain.Entities;
using Briscola.Persistence.Entities;
using Briscola.Persistence.Extension;

namespace Briscola.Persistence;

public class GameSerializer
{
    // Constant for extension
    public const string FileExtension = ".json";

    /// <summary>
    /// Format for the file name of the game log, using folder, identifier and extension
    /// </summary>
    public static readonly Func<string, string, string, string> FileNameFormat =
            (folder, identifier, extension) => $"{folder}/gamelog-{identifier}{extension}";


    /// <summary>
    /// Writes the game log to a file in the specified folder
    /// </summary>
    public static GameLog Write(Game game, string folder)
    {
        var log = game.ToGameLog();
        var json = JsonSerializer.Serialize(log);
        var path = FileNameFormat(folder, log.Timestamp.ToString("yyyyMMddHHmmss"), FileExtension);
        File.WriteAllText(path, json);
        return log;
    }
}
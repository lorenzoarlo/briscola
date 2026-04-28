internal enum AnsiColors
{
    Reset,
    // Foreground
    FgBlack = 30, FgRed = 31, FgGreen = 32, FgYellow = 33,
    FgBlue = 34, FgMagenta = 35, FgCyan = 36, FgWhite = 37,
    FgGray = 90, FgBrightRed = 91, FgBrightGreen = 92, FgBrightYellow = 93,
    FgBrightBlue = 94, FgBrightMagenta = 95, FgBrightCyan = 96, FgBrightWhite = 97,

    // Background
    BgBlack = 40, BgRed = 41, BgGreen = 42, BgYellow = 43,
    BgBlue = 44, BgMagenta = 45, BgCyan = 46, BgWhite = 47,
    BgGray = 100, BgBrightRed = 101, BgBrightGreen = 102, BgBrightYellow = 103,
    BgBrightBlue = 104, BgBrightMagenta = 105, BgBrightCyan = 106, BgBrightWhite = 107
}

internal static class AnsiColorExtensions
{
    public static string ToAnsi(this AnsiColors color)
    {
        if (color == AnsiColors.Reset) return "\u001b[0m";

        // Sfruttiamo il valore numerico dell'enum per pulire lo switch
        return $"\u001b[{(int)color}m";
    }
}
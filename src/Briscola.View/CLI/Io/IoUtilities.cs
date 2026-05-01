namespace Briscola.View.CLI.Io;

internal static class IoUtilities
{
    /// <summary>
    /// Returns a string representation of a card with ANSI color codes based on its suit.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public static string Colorize(string s, AnsiColors color) => $"{color.ToAnsi()}{s}{AnsiColors.Reset.ToAnsi()}";

}
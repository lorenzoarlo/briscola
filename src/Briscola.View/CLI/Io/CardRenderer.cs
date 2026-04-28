using Briscola.Domain.Entities;
using Briscola.Domain.Enums;

namespace Briscola.View.CLI.Io;

/// <summary>
/// Utility class for rendering cards in the CLI interface.
/// </summary>
internal static class CardRenderer
{
    /// <summary>
    /// Writes a card to the console with color coding based on its suit.
    /// </summary>
    public static string Format(Card? c) => (c == null) ? IoUtilities.Colorize("-", AnsiColors.FgGray) : IoUtilities.Colorize($"{c}", GetSuitColor(c.Suit));

    /// <summary>
    /// Writes a card to the console with color coding based on its suit.
    /// </summary>
    public static void Write(Card? c) => Console.Write(Format(c));

    /// <summary>
    /// Writes a card to the console with color coding based on its suit, followed by a newline.
    /// If the card is null, it will display a gray dash ("-") instead.
    /// </summary>
    /// <param name="c"></param>
    public static void WriteLine(Card? c)
    {
        Write(c);
        Console.WriteLine();
    }

    /// <summary>
    /// Gets the console color associated with a card suit.
    /// </summary>
    private static AnsiColors GetSuitColor(Suit suit) => suit switch
    {
        Suit.Coins => AnsiColors.FgYellow,
        Suit.Cups => AnsiColors.FgRed,
        Suit.Swords => AnsiColors.FgCyan,
        Suit.Clubs => AnsiColors.FgGreen,
        _ => AnsiColors.FgWhite
    };
}
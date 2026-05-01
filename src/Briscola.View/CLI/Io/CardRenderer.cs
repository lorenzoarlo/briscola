using Briscola.Domain.Enums;
using Briscola.Domain.Entities;
using Briscola.View.Resources;

namespace Briscola.View.CLI.Io;

/// <summary>
/// Utility class for rendering cards in the CLI interface.
/// </summary>
internal static class CardRenderer
{

    public static string Format(Suit suit) => IoUtilities.Colorize(suit.ToLocalizedString(), GetSuitColor(suit));

    /// <summary>
    /// Writes a card to the console with color coding based on its suit.
    /// </summary>
    public static string Format(Card? c) => (c == null) ? IoUtilities.Colorize("-", AnsiColors.FgGray) : IoUtilities.Colorize($"{c.ToLocalizedString()}", GetSuitColor(c.Suit));

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

// Extension method to convert Suit enum values to their localized string representations.
internal static class SuitLocalizedExtensions
{
    public static string ToLocalizedString(this Suit suit) => suit switch
    {
        Suit.Coins => Messages.Suit_Coins,
        Suit.Cups => Messages.Suit_Cups,
        Suit.Swords => Messages.Suit_Swords,
        Suit.Clubs => Messages.Suit_Clubs,
        _ => throw new ArgumentOutOfRangeException(nameof(suit), @"Unexpected suit value")
    };
}


// Extension method to convert CardValue enum values to their localized string representations.
internal static class CardValueLocalizedExtensions
{
    public static string ToLocalizedString(this CardValue value) => value switch
    {
        CardValue.Ace => Messages.Value_Ace,
        CardValue.Two => Messages.Value_Two,
        CardValue.Three => Messages.Value_Three,
        CardValue.Four => Messages.Value_Four,
        CardValue.Five => Messages.Value_Five,
        CardValue.Six => Messages.Value_Six,
        CardValue.Seven => Messages.Value_Seven,
        CardValue.Jack => Messages.Value_Jack,
        CardValue.Knight => Messages.Value_Knight,
        CardValue.King => Messages.Value_King,
        _ => throw new ArgumentOutOfRangeException(nameof(value), @"Unexpected card value")
    };
}

internal static class CardLocalizedExtensions
{
    public static string ToLocalizedString(this Card card) => string.Format(Messages.CardFormat, card.Value.ToLocalizedString(), card.Suit.ToLocalizedString());
}
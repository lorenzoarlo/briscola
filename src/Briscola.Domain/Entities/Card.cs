using Briscola.Domain.Enums;

namespace Briscola.Domain.Entities;

/// <summary>Represents a card with a suit and a value.</summary>
public record Card(Suit Suit, CardValue Value)
{
    /// <summary>Points value of the card.</summary>
    public int Points => Value switch
    {
        CardValue.Ace => 11,
        CardValue.Three => 10,
        CardValue.Jack => 2,
        CardValue.Knight => 3,
        CardValue.King => 4,
        _ => 0
    };

    /// <summary>Returns a string representation of the card.</summary>
    public override string ToString()
    {
        return $"{Value} of {Suit}";
    }
}

// Since Player is abstract, we need a concrete implementation for testing
using Briscola.Domain.Entities;

namespace Briscola.Tests.Domain.Entities.Utils;

public class TestPlayerStrategy : PlayerStrategy
{
    public override string Type => "Test";

    public override Task<Card> ChooseCardAsync(Player self, PlayerStateSnapshot state, GameSnapshot context, CancellationToken ct = default) => throw new NotImplementedException();

}
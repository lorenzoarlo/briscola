namespace Briscola.Domain.Entities.Players;

public class SimpleEarnLogicPlayerStrategy : PlayerStrategy
{
    public override string Type => "SimpleEarnLogicPlayerStrategy";

    public override Task<Card> ChooseCardAsync(Player self, PlayerStateSnapshot state, GameSnapshot context,
        CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

using Briscola.Domain.Entities;
using Briscola.Domain.Enums;
using Briscola.View.CLI.Io;
using Briscola.View.CLI.Utils;

namespace Briscola.View.CLI.Players;

public class CLIHumanPlayerStrategy : PlayerStrategy
{
    public override string Type => "CLIHuman";

    private readonly HandSlots _slots = new();
    // Coda per gestire le notifiche nel footer (es. "Peschi X", "Mario gioca Y")
    private readonly List<string> _notifications = new();

    private void AddNotification(string message)
    {
        _notifications.Add(message);
        if (_notifications.Count > 3)
            _notifications.RemoveAt(0); // Manteniamo solo le ultime 3 notifiche per non riempire lo schermo
    }


    /// <summary>
    /// Renders the CLI view for the player, showing the current game state and any relevant notifications.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="self"></param>
    /// <param name="subheader"></param>
    private void RenderView(GameSnapshot context, Player self, string subheader)
    {
        // Clean the screen before rendering the new view
        Console.Clear();

        Header(context, self);
        Subheader(context, subheader);

        Body(context, self);

        Console.WriteLine(); // Space before footer

        Footer(_notifications);
    }

    public override Task<Card> ChooseCardAsync(Player self, PlayerStateSnapshot state, GameSnapshot context, CancellationToken ct = default)
    {
        RenderView(context, self, "It's your turn!");
        return Task.FromResult(PromptForCard(_slots)); ;
    }

    public override Task OnTrickStartedAsync(Player self, GameSnapshot context, CancellationToken ct = default)
    {
        _notifications.Clear();

        var activePlayer = context.CurrentTrick?.CurrentPlayer;
        if (activePlayer != null)
        {
            string subheader = activePlayer.Id == self.Id
                ? IoUtilities.Colorize("It's your turn!", AnsiColors.FgGreen)
                : IoUtilities.Colorize($"It's the turn of {activePlayer.Name}...", AnsiColors.FgYellow);

            RenderView(context, self, subheader);
        }

        return Task.CompletedTask;
    }

    public override async Task OnCardPlayedAsync(Player self, Player player, Card card, GameSnapshot context, CancellationToken ct = default)
    {
        if (player.Id == self.Id)
        {
            _slots.Play(card);
            AddNotification($"You played {CardRenderer.Format(card)}");
        }
        else
        {
            AddNotification($"{player.Name} played {CardRenderer.Format(card)}");
        }

        var trick = context.CurrentTrick;
        var activePlayer = trick?.CurrentPlayer;


        if (trick == null || activePlayer == null || trick.IsComplete)
        {
            return;
        }

        RenderView(context, self, $"It's the turn of {activePlayer.Name}...");
        await Task.Delay(1000, ct); // Delay to allow the player to see the opponent's move before the next prompt appears
    }

    public override async Task OnTrickCompletedAsync(Player self, Player winner, IEnumerable<Card> cards, GameSnapshot context, CancellationToken ct = default)
    {
        var pts = cards.Sum(c => c.Points);

        AddNotification($"Trick won by {winner.Name} (earned {pts} points)");
        RenderView(context, self, $"Trick won by {winner.Name}!");

        await Task.Delay(1500, ct); // Delay to allow the player to see the trick result before the next prompt appears
    }

    public override Task OnCardDrawnAsync(Card card, GameSnapshot context, CancellationToken ct = default)
    {
        _slots.Draw(card);
        AddNotification($"You've drawn {card.Value} of {card.Suit}");
        return Task.CompletedTask;
    }

    public static readonly string WIN_MESSAGE = IoUtilities.Colorize("You've won!", AnsiColors.FgGreen);

    public static readonly string LOSE_MESSAGE = IoUtilities.Colorize("You've lost!", AnsiColors.FgBrightRed);

    public static readonly string TIE_MESSAGE = IoUtilities.Colorize("It's a tie!", AnsiColors.FgYellow);


    public override Task OnMatchEndedAsync(GameResult result, int teamIndex, IReadOnlyDictionary<Player, int> scores, CancellationToken ct = default)
    {
        Console.Clear();

        Console.WriteLine(IoUtilities.Colorize("Match Ended! ", AnsiColors.FgBrightMagenta));

        Console.WriteLine(result switch
        {
            GameResult.Tie => TIE_MESSAGE,
            GameResult.WinTeam1 => teamIndex == 0 ? WIN_MESSAGE : LOSE_MESSAGE,
            GameResult.WinTeam2 => teamIndex == 1 ? WIN_MESSAGE : LOSE_MESSAGE,
            _ => throw new ArgumentOutOfRangeException(nameof(result), "Unexpected game result")
        });

        foreach (var kvp in scores)
        {
            var player = kvp.Key;
            var points = kvp.Value;
            Console.WriteLine($"- {player.Name}: {points} points");
        }

        return Task.CompletedTask;
    }


    private static void Header(GameSnapshot context, Player _)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"Briscola match (Turn number {context.Tricks.Count})");
        Console.ResetColor();
    }

    private static void Subheader(GameSnapshot context, string subheader)
    {
        // Last trick summary (if applicable)
        var last = context.Tricks.ElementAtOrDefault(-2);
        if (last != null && last.Winner != null)
        {
            var details = string.Join(',', last.PlayOrder.Select(p => CardRenderer.Format(last.CardOfPlayer(p))));

            int points = last.TrickCards.Sum(c => c.Points);

            // Costruisci la stringa di riepilogo
            string summary = $"> {details} and {last.Winner.Name} won {points} pts";

            Console.WriteLine(IoUtilities.Colorize(summary, AnsiColors.FgGray));
        }
        // Subheader
        Console.WriteLine(subheader);
    }


    private static void Body(GameSnapshot context, Player _)
    {
        Console.Write($"Deck ({context.DeckCount} cards), Briscola is ");
        if (context.BriscolaCard != null)
        {
            CardRenderer.Write(context.BriscolaCard);
            Console.WriteLine(" [✓]");
        }
        else
        {
            Console.WriteLine($"{context.BriscolaSuit} [x]");
        }

        Console.WriteLine("Table:");
        var trick = context.CurrentTrick;
        if (trick != null)
        {
            // Enum index
            int index = 1;
            foreach (var player in trick.PlayOrder)
            {
                Console.Write($"{index}. ");
                var cardOf = trick.CardOfPlayer(player);
                CardRenderer.WriteLine(cardOf);
                index++;
            }
        }
    }

    private static void Footer(IReadOnlyCollection<string> notifications)
    {
        foreach (var note in notifications)
        {
            Console.WriteLine(IoUtilities.Colorize($"> {note}", AnsiColors.FgWhite));
        }
    }


    private static Card PromptForCard(HandSlots _slots)
    {
        Console.WriteLine("Choose your card:");
        for (int i = 0; i < _slots.View.Count; i++)
        {
            Console.Write($"{i + 1}. ");
            var c = _slots.View[i];
            CardRenderer.Write(c);
            Console.WriteLine();
        }



        while (true)
        {
            Console.Write("Choice: ");
            var input = Console.ReadLine();
            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= _slots.View.Count)
            {
                var selectedCard = _slots.View[choice - 1];
                if (selectedCard != null)
                {
                    return selectedCard;
                }
            }

            // error
            Console.WriteLine(IoUtilities.Colorize("Invalid choice. Must be a valid slot number and non-empty.", AnsiColors.FgBrightRed));
        }
    }


}
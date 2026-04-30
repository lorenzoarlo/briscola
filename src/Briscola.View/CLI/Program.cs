using Briscola.View.CLI;
using Briscola.View.Resources;

// Setup of the cancellation token to handle graceful shutdown (e.g., Ctrl+C)
using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

try
{
    // Initialize the CLI Game Hub
    var hub = new CLIGameHubView();

    // Run the main application loop
    await hub.RunAsync(cts.Token);
}
catch (OperationCanceledException)
{
    // Clean exit when cancellation is requested
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(string.Format(Messages.Error_Unexpected, ex.Message));
    Console.ResetColor();
}
finally
{
    Console.WriteLine(Messages.Msg_Terminated);
    if (!cts.IsCancellationRequested) Console.ReadKey();
}
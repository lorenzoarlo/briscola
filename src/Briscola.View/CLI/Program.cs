using Briscola.View.CLI;
using Briscola.View.Resources;

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    // ReSharper disable once AccessToDisposedClosure
    cts.Cancel();
};

try
{
    // Initialize the CLI Game Hub
    var hub = new CliGameHubView();

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
    Console.WriteLine(Messages.Error_Unexpected, ex.Message);
    Console.ResetColor();
}
finally
{
    Console.WriteLine(Messages.Msg_Terminated);
    if (!cts.IsCancellationRequested) Console.ReadKey();
}
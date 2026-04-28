using Briscola.View.CLI;

// 1. Setup of the cancellation token to handle graceful shutdown (e.g., Ctrl+C)
using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

try
{
    // 2. Initialize the CLI Game Hub
    var hub = new CLIGameHubView();

    // 3. Run the main application loop
    await hub.RunAsync(cts.Token);
}
catch (OperationCanceledException)
{
    // Clean exit when cancellation is requested
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\nAn unexpected error occurred: {ex.Message}");
    Console.ResetColor();
}
finally
{
    Console.WriteLine("\nApplication terminated. Press any key to exit...");
    if (!cts.IsCancellationRequested) Console.ReadKey();
}
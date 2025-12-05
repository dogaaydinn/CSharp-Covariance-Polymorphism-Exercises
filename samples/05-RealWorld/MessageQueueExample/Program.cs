using System.Threading.Channels;

Console.WriteLine("=== Message Queue Example (using Channels) ===\n");

var channel = Channel.CreateUnbounded<OrderMessage>();

// Producer task
var producer = Task.Run(async () =>
{
    for (int i = 1; i <= 5; i++)
    {
        var message = new OrderMessage(i, $"Order #{i}", 100 * i);
        await channel.Writer.WriteAsync(message);
        Console.WriteLine($"[Producer] Sent: {message.OrderId}");
        await Task.Delay(500);
    }

    channel.Writer.Complete();
    Console.WriteLine("[Producer] Done sending");
});

// Consumer task
var consumer = Task.Run(async () =>
{
    await foreach (var message in channel.Reader.ReadAllAsync())
    {
        Console.WriteLine($"[Consumer] Processing: {message.OrderId} - ${message.Amount}");
        await Task.Delay(1000);  // Simulate work
        Console.WriteLine($"[Consumer] Completed: {message.OrderId}");
    }

    Console.WriteLine("[Consumer] Done processing");
});

await Task.WhenAll(producer, consumer);

Console.WriteLine("\n✅ Message queue demo complete!");

record OrderMessage(int OrderId, string Description, decimal Amount);

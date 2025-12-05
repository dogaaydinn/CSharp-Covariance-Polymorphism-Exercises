using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

/// <summary>
/// Order Processing with RabbitMQ Demo
///
/// Demonstrates:
/// - Producer/Consumer pattern with RabbitMQ
/// - Message serialization (JSON)
/// - Dead Letter Queue (DLQ) handling
/// - Consumer acknowledgment (manual ACK)
/// - Message retry logic
/// - Durable queues and persistent messages
/// </summary>

Console.WriteLine("=== Order Processing with RabbitMQ Demo ===\n");

// Check if RabbitMQ is available
var rabbitMQHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
var useRabbitMQ = await IsRabbitMQAvailableAsync(rabbitMQHost);

if (useRabbitMQ)
{
    Console.WriteLine($"✅ RabbitMQ available at {rabbitMQHost}\n");
    await RunRabbitMQExamplesAsync(rabbitMQHost);
}
else
{
    Console.WriteLine($"⚠️  RabbitMQ not available at {rabbitMQHost}");
    Console.WriteLine("Running Channel<T>-based examples...\n");
    await RunChannelExamplesAsync();
}

Console.WriteLine("\n=== Demo Complete ===");

// ============================================================================
// RabbitMQ Examples
// ============================================================================

static async Task RunRabbitMQExamplesAsync(string host)
{
    var factory = new ConnectionFactory
    {
        HostName = host,
        UserName = "guest",
        Password = "guest",
        VirtualHost = "/",
        AutomaticRecoveryEnabled = true,
        NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
    };

    using var connection = factory.CreateConnection("OrderProcessingDemo");
    using var channel = connection.CreateModel();

    Console.WriteLine(new string('=', 60));
    Console.WriteLine("RABBITMQ ORDER PROCESSING DEMONSTRATIONS");
    Console.WriteLine(new string('=', 60) + "\n");

    // Setup queues
    SetupQueues(channel);

    // 1. Basic Producer/Consumer
    await Demo1_BasicProducerConsumerAsync(channel);

    // 2. Dead Letter Queue
    await Demo2_DeadLetterQueueAsync(channel);

    // 3. Manual Acknowledgment
    await Demo3_ManualAcknowledgmentAsync(channel);

    // 4. Persistent Messages
    await Demo4_PersistentMessagesAsync(channel);

    // 5. Message Priority
    await Demo5_MessagePriorityAsync(channel);
}

static void SetupQueues(IModel channel)
{
    // Main order queue (durable)
    channel.QueueDeclare(
        queue: "orders",
        durable: true,
        exclusive: false,
        autoDelete: false,
        arguments: new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", "" },
            { "x-dead-letter-routing-key", "orders.dlq" },
            { "x-message-ttl", 300000 } // 5 minutes TTL
        });

    // Dead Letter Queue
    channel.QueueDeclare(
        queue: "orders.dlq",
        durable: true,
        exclusive: false,
        autoDelete: false,
        arguments: null);

    // Priority queue
    channel.QueueDeclare(
        queue: "orders.priority",
        durable: true,
        exclusive: false,
        autoDelete: false,
        arguments: new Dictionary<string, object>
        {
            { "x-max-priority", 10 }
        });

    Console.WriteLine("✅ Queues configured:");
    Console.WriteLine("  • orders (main queue with DLQ)");
    Console.WriteLine("  • orders.dlq (dead letter queue)");
    Console.WriteLine("  • orders.priority (priority queue)\n");
}

// ============================================================================
// Demonstrations
// ============================================================================

static async Task Demo1_BasicProducerConsumerAsync(IModel channel)
{
    Console.WriteLine("1️⃣  BASIC PRODUCER/CONSUMER PATTERN\n");

    // Producer: Send 3 orders
    Console.WriteLine("Producer: Publishing orders...");
    for (int i = 1; i <= 3; i++)
    {
        var order = new OrderMessage
        {
            OrderId = i,
            CustomerId = 100 + i,
            Items = new[] { $"Item{i}A", $"Item{i}B" },
            TotalAmount = 99.99m * i,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(order);
        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish(
            exchange: "",
            routingKey: "orders",
            basicProperties: null,
            body: body);

        Console.WriteLine($"  ✓ Published Order #{order.OrderId} | Amount: ${order.TotalAmount}");
        await Task.Delay(100);
    }

    Console.WriteLine("\nConsumer: Processing orders...");

    // Consumer: Process orders
    var consumer = new EventingBasicConsumer(channel);
    var processedCount = 0;

    consumer.Received += (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var json = Encoding.UTF8.GetString(body);
        var order = JsonSerializer.Deserialize<OrderMessage>(json);

        Console.WriteLine($"  → Processing Order #{order?.OrderId} | ${order?.TotalAmount}");
        Thread.Sleep(500); // Simulate processing
        Console.WriteLine($"  ✓ Completed Order #{order?.OrderId}");

        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        processedCount++;
    };

    channel.BasicConsume(queue: "orders", autoAck: false, consumer: consumer);

    // Wait for processing
    while (processedCount < 3)
    {
        await Task.Delay(100);
    }

    Console.WriteLine("\n✅ All orders processed successfully\n");
}

static async Task Demo2_DeadLetterQueueAsync(IModel channel)
{
    Console.WriteLine("2️⃣  DEAD LETTER QUEUE (DLQ) HANDLING\n");

    Console.WriteLine("Simulating failed message processing...");

    // Publish a message that will fail
    var failingOrder = new OrderMessage
    {
        OrderId = 999,
        CustomerId = 0, // Invalid customer
        Items = Array.Empty<string>(),
        TotalAmount = -100, // Invalid amount
        Status = OrderStatus.Pending,
        CreatedAt = DateTime.UtcNow
    };

    var json = JsonSerializer.Serialize(failingOrder);
    var body = Encoding.UTF8.GetBytes(json);

    channel.BasicPublish("", "orders", null, body);
    Console.WriteLine($"  ✓ Published invalid Order #{failingOrder.OrderId}");

    await Task.Delay(100);

    // Consumer that rejects invalid orders
    var consumer = new EventingBasicConsumer(channel);
    var receivedInvalid = false;

    consumer.Received += (model, ea) =>
    {
        var messageBody = ea.Body.ToArray();
        var messageJson = Encoding.UTF8.GetString(messageBody);
        var order = JsonSerializer.Deserialize<OrderMessage>(messageJson);

        if (order?.CustomerId == 0 || order?.TotalAmount < 0)
        {
            Console.WriteLine($"  ❌ Invalid order detected: #{order?.OrderId}");
            Console.WriteLine($"     Rejecting and sending to DLQ...");

            // Reject message → goes to Dead Letter Queue
            channel.BasicReject(deliveryTag: ea.DeliveryTag, requeue: false);
            receivedInvalid = true;
        }
        else
        {
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }
    };

    channel.BasicConsume(queue: "orders", autoAck: false, consumer: consumer);

    while (!receivedInvalid)
    {
        await Task.Delay(50);
    }

    await Task.Delay(200);

    // Check DLQ
    var dlqResult = channel.BasicGet(queue: "orders.dlq", autoAck: false);
    if (dlqResult != null)
    {
        var dlqJson = Encoding.UTF8.GetString(dlqResult.Body.ToArray());
        var dlqOrder = JsonSerializer.Deserialize<OrderMessage>(dlqJson);
        Console.WriteLine($"\n  ✓ Message found in DLQ: Order #{dlqOrder?.OrderId}");
        Console.WriteLine($"    Can be manually reviewed or reprocessed");

        channel.BasicAck(dlqResult.DeliveryTag, false);
    }

    Console.WriteLine("\n✅ DLQ handling demonstrated\n");
}

static async Task Demo3_ManualAcknowledgmentAsync(IModel channel)
{
    Console.WriteLine("3️⃣  MANUAL ACKNOWLEDGMENT (Reliable Processing)\n");

    Console.WriteLine("Publishing order for manual ACK demo...");

    var order = new OrderMessage
    {
        OrderId = 50,
        CustomerId = 150,
        Items = new[] { "Laptop", "Mouse" },
        TotalAmount = 1299.99m,
        Status = OrderStatus.Pending,
        CreatedAt = DateTime.UtcNow
    };

    var json = JsonSerializer.Serialize(order);
    var body = Encoding.UTF8.GetBytes(json);
    channel.BasicPublish("", "orders", null, body);

    Console.WriteLine($"  ✓ Published Order #{order.OrderId}\n");

    Console.WriteLine("Consumer: Processing with manual ACK...");

    var consumer = new EventingBasicConsumer(channel);
    var acknowledged = false;

    consumer.Received += (model, ea) =>
    {
        try
        {
            var messageBody = ea.Body.ToArray();
            var messageJson = Encoding.UTF8.GetString(messageBody);
            var receivedOrder = JsonSerializer.Deserialize<OrderMessage>(messageJson);

            Console.WriteLine($"  → Received Order #{receivedOrder?.OrderId}");
            Console.WriteLine("  → Simulating processing...");
            Thread.Sleep(500);

            // Simulate 30% chance of processing failure
            var random = new Random();
            if (random.Next(100) < 30)
            {
                Console.WriteLine("  ❌ Processing failed! Negative ACK (NACK)");
                // NACK with requeue - message will be redelivered
                channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
            }
            else
            {
                Console.WriteLine("  ✓ Processing successful! Sending ACK");
                // ACK - message removed from queue
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                acknowledged = true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ❌ Exception: {ex.Message}");
            // NACK on exception
            channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
        }
    };

    channel.BasicConsume(queue: "orders", autoAck: false, consumer: consumer);

    while (!acknowledged)
    {
        await Task.Delay(100);
    }

    Console.WriteLine("\n✅ Manual acknowledgment ensures reliable processing\n");
}

static async Task Demo4_PersistentMessagesAsync(IModel channel)
{
    Console.WriteLine("4️⃣  PERSISTENT MESSAGES (Survives Broker Restart)\n");

    var properties = channel.CreateBasicProperties();
    properties.Persistent = true; // Message survives broker restart
    properties.DeliveryMode = 2;  // Persistent

    var order = new OrderMessage
    {
        OrderId = 60,
        CustomerId = 160,
        Items = new[] { "Server", "Router" },
        TotalAmount = 5999.99m,
        Status = OrderStatus.Pending,
        CreatedAt = DateTime.UtcNow
    };

    var json = JsonSerializer.Serialize(order);
    var body = Encoding.UTF8.GetBytes(json);

    channel.BasicPublish(
        exchange: "",
        routingKey: "orders",
        basicProperties: properties,
        body: body);

    Console.WriteLine($"  ✓ Published persistent Order #{order.OrderId}");
    Console.WriteLine("  ✓ Message will survive RabbitMQ broker restart");
    Console.WriteLine("  ✓ Stored on disk (durable queue + persistent message)\n");

    await Task.Delay(100);

    // Consume and acknowledge
    var result = channel.BasicGet(queue: "orders", autoAck: false);
    if (result != null)
    {
        var receivedJson = Encoding.UTF8.GetString(result.Body.ToArray());
        var receivedOrder = JsonSerializer.Deserialize<OrderMessage>(receivedJson);
        Console.WriteLine($"  → Retrieved persistent Order #{receivedOrder?.OrderId}");
        channel.BasicAck(result.DeliveryTag, false);
    }

    Console.WriteLine("\n✅ Persistent messages ensure durability\n");
}

static async Task Demo5_MessagePriorityAsync(IModel channel)
{
    Console.WriteLine("5️⃣  MESSAGE PRIORITY (High-Priority First)\n");

    Console.WriteLine("Publishing orders with different priorities...");

    var properties = channel.CreateBasicProperties();

    // Low priority order
    properties.Priority = 1;
    var lowPriorityOrder = new OrderMessage
    {
        OrderId = 70,
        CustomerId = 170,
        Items = new[] { "Standard Item" },
        TotalAmount = 19.99m,
        Status = OrderStatus.Pending,
        CreatedAt = DateTime.UtcNow
    };
    channel.BasicPublish("", "orders.priority", properties, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(lowPriorityOrder)));
    Console.WriteLine($"  ✓ Low priority (1): Order #{lowPriorityOrder.OrderId}");

    // High priority order
    properties.Priority = 10;
    var highPriorityOrder = new OrderMessage
    {
        OrderId = 71,
        CustomerId = 171,
        Items = new[] { "Urgent Item" },
        TotalAmount = 999.99m,
        Status = OrderStatus.Pending,
        CreatedAt = DateTime.UtcNow
    };
    channel.BasicPublish("", "orders.priority", properties, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(highPriorityOrder)));
    Console.WriteLine($"  ✓ High priority (10): Order #{highPriorityOrder.OrderId}");

    await Task.Delay(100);

    Console.WriteLine("\nConsuming orders (high priority first)...");

    // First message should be high priority
    var result1 = channel.BasicGet(queue: "orders.priority", autoAck: false);
    if (result1 != null)
    {
        var order = JsonSerializer.Deserialize<OrderMessage>(Encoding.UTF8.GetString(result1.Body.ToArray()));
        Console.WriteLine($"  → First: Order #{order?.OrderId} (Priority: {result1.BasicProperties.Priority})");
        channel.BasicAck(result1.DeliveryTag, false);
    }

    var result2 = channel.BasicGet(queue: "orders.priority", autoAck: false);
    if (result2 != null)
    {
        var order = JsonSerializer.Deserialize<OrderMessage>(Encoding.UTF8.GetString(result2.Body.ToArray()));
        Console.WriteLine($"  → Second: Order #{order?.OrderId} (Priority: {result2.BasicProperties.Priority})");
        channel.BasicAck(result2.DeliveryTag, false);
    }

    Console.WriteLine("\n✅ High-priority messages processed first\n");
}

// ============================================================================
// Channel<T> Fallback Examples
// ============================================================================

static async Task RunChannelExamplesAsync()
{
    Console.WriteLine("1️⃣  CHANNEL<T> PRODUCER/CONSUMER (Fallback)\n");

    var channel = Channel.CreateBounded<OrderMessage>(new BoundedChannelOptions(100)
    {
        FullMode = BoundedChannelFullMode.Wait
    });

    // Producer task
    var producer = Task.Run(async () =>
    {
        Console.WriteLine("Producer: Creating orders...");
        for (int i = 1; i <= 5; i++)
        {
            var order = new OrderMessage
            {
                OrderId = i,
                CustomerId = 100 + i,
                Items = new[] { $"Item{i}" },
                TotalAmount = 50m * i,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await channel.Writer.WriteAsync(order);
            Console.WriteLine($"  ✓ Queued Order #{order.OrderId} | ${order.TotalAmount}");
            await Task.Delay(300);
        }

        channel.Writer.Complete();
        Console.WriteLine("  ✓ Producer completed\n");
    });

    // Consumer task
    var consumer = Task.Run(async () =>
    {
        Console.WriteLine("Consumer: Processing orders...");
        await foreach (var order in channel.Reader.ReadAllAsync())
        {
            Console.WriteLine($"  → Processing Order #{order.OrderId}");
            await Task.Delay(500); // Simulate processing
            Console.WriteLine($"  ✓ Completed Order #{order.OrderId}");
        }

        Console.WriteLine("  ✓ Consumer completed\n");
    });

    await Task.WhenAll(producer, consumer);

    Console.WriteLine("✅ All orders processed (in-memory queue)\n");
}

// ============================================================================
// Helper Methods
// ============================================================================

static async Task<bool> IsRabbitMQAvailableAsync(string host)
{
    try
    {
        var factory = new ConnectionFactory
        {
            HostName = host,
            RequestedConnectionTimeout = TimeSpan.FromSeconds(2),
            AutomaticRecoveryEnabled = false
        };

        using var connection = factory.CreateConnection();
        return connection.IsOpen;
    }
    catch
    {
        return false;
    }
}

// ============================================================================
// Models
// ============================================================================

public class OrderMessage
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public string[] Items { get; set; } = Array.Empty<string>();
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int RetryCount { get; set; }
}

public enum OrderStatus
{
    Pending,
    Processing,
    Completed,
    Failed,
    Cancelled
}

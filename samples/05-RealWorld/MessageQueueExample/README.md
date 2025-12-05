# Message Queue Example

> Asynchronous message processing with System.Threading.Channels.

## Pattern
- **Producer-Consumer** - Decouple message sending and processing
- **Channels** - High-performance queue (built-in to .NET)
- **Async/await** - Non-blocking processing

## Use Cases
- Order processing
- Email sending
- Event streaming
- Background jobs

## Run
```bash
dotnet run
```

## Production
For distributed systems, use RabbitMQ, Azure Service Bus, or Kafka. This example uses Channels for in-process demonstration.

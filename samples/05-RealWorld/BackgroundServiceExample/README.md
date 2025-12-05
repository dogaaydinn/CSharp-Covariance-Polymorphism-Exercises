# Background Service Example

> Long-running background task with IHostedService.

## Pattern
- **BackgroundService** - Base class for long-running tasks
- **Graceful Shutdown** - Handles cancellation tokens
- **Logging** - Integrated Microsoft.Extensions.Logging

## Use Cases
- Email queue processing
- Data synchronization
- Cache warming
- Scheduled jobs

## Run
```bash
dotnet run
# Press Ctrl+C to see graceful shutdown
```

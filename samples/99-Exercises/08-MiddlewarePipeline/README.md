# Middleware Pipeline

## Problem
Implement ASP.NET-style middleware pipeline.

## Example
```csharp
var pipeline = new Pipeline()
    .Use(LoggingMiddleware)
    .Use(AuthenticationMiddleware)
    .Use(RequestHandlerMiddleware);

await pipeline.ExecuteAsync(context);
```

## Pattern
```csharp
public async Task InvokeAsync(HttpContext context, RequestDelegate next)
{
    // Before logic
    await next(context);
    // After logic
}
```

## Use Cases
- Logging
- Authentication
- Error handling
- Request/Response transformation

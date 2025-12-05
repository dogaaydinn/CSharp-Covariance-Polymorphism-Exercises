using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Configure OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSource("OpenTelemetryDemo")
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService("OpenTelemetryDemo"))
            .AddConsoleExporter();  // Prints traces to console
    });

builder.Services.AddHttpClient();

var app = builder.Build();

app.MapGet("/", () => "OpenTelemetry Demo - Check /process for distributed tracing");

app.MapGet("/process", async (IHttpClientFactory factory) =>
{
    using var activity = System.Diagnostics.Activity.Current;
    activity?.SetTag("user.id", "123");
    activity?.AddEvent(new("Processing started"));

    // Simulate external API call
    var client = factory.CreateClient();
    await client.GetStringAsync("https://jsonplaceholder.typicode.com/todos/1");

    activity?.AddEvent(new("Processing completed"));

    return new
    {
        traceId = activity?.TraceId.ToString(),
        spanId = activity?.SpanId.ToString(),
        message = "Request processed with distributed tracing!"
    };
});

app.Run();

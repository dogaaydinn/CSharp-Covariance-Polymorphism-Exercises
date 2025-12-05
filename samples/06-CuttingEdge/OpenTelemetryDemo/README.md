# OpenTelemetry Distributed Tracing

> Observability standard for traces, metrics, and logs across microservices.

## Features
- **Distributed Tracing** - Track requests across services
- **Spans & Events** - Detailed operation tracking
- **Context Propagation** - TraceId flows through all services
- **Vendor-neutral** - Works with Jaeger, Zipkin, Azure Monitor

## Run
```bash
cd samples/06-CuttingEdge/OpenTelemetryDemo
dotnet run

# Make request
curl http://localhost:5000/process
```

**Console Output:**
```
Activity.TraceId:          80000000-0000-0000-0000-000000000001
Activity.SpanId:           8000000000000001
Activity.DisplayName:      GET /process
Activity.Kind:             Server
Activity.StartTime:        2024-01-01T10:00:00.0000000Z
Activity.Duration:         00:00:00.1234567
Activity.Tags:
    user.id: 123
Activity.Events:
    Processing started
    Processing completed
```

## Production Setup
Replace `AddConsoleExporter()` with:
- **Jaeger:** `AddJaegerExporter()`
- **Azure Monitor:** `AddAzureMonitorTraceExporter()`
- **OTLP:** `AddOtlpExporter()` (Grafana, Datadog)

**Use Cases:** Microservices debugging, performance analysis, SLA monitoring.

# gRPC Calculator Example

A production-ready demonstration of **gRPC** (Google Remote Procedure Call) with comprehensive examples of all four RPC communication patterns.

## What This Demonstrates

### Core Concepts
- **Protocol Buffers (Protobuf)** - Efficient binary serialization
- **HTTP/2 Transport** - Modern protocol with multiplexing
- **Four RPC Patterns** - Unary, Server Streaming, Client Streaming, Bidirectional
- **Type Safety** - Strongly-typed service contracts
- **High Performance** - 10-20x faster than REST/JSON

### Project Structure
```
gRPCExample/
├── Protos/
│   └── calculator.proto          # Protobuf service definition
├── Program.cs                     # Server + Client implementation
├── gRPCExample.csproj            # Project with gRPC packages
├── README.md                      # This file
└── WHY_THIS_PATTERN.md           # Design rationale
```

## Running the Example

### Prerequisites
- .NET 8 SDK
- Terminal/Command Prompt

### Build
```bash
dotnet build
```

### Run (Server + Client)
```bash
dotnet run
```

### Run Server Only
```bash
dotnet run server
```

### Run Client Only (requires server running)
```bash
# Terminal 1
dotnet run server

# Terminal 2
dotnet run client
```

## Expected Output

### 1️⃣ Unary RPC (Request-Response)
```
[Server] Add: 10 + 5 = 15
[Client] 10 + 5 = 15

[Server] Subtract: 10 - 5 = 5
[Client] 10 - 5 = 5

[Server] Multiply: 10 × 5 = 50
[Client] 10 × 5 = 50

[Server] Divide: 10 ÷ 5 = 2
[Client] 10 ÷ 5 = 2

[Server] Divide: 10 ÷ 0 = ERROR
[Client] 10 ÷ 0 = ERROR: Division by zero
```

**Pattern:** Client sends one request → Server returns one response
**Use Case:** Standard API calls (like REST)

### 2️⃣ Server Streaming RPC
```
[Server] Fibonacci: Generating 10 numbers
[Server] Fibonacci[0] = 0
[Client] Fibonacci[0] = 0
[Server] Fibonacci[1] = 1
[Client] Fibonacci[1] = 1
...
[Server] Fibonacci[9] = 34
[Client] Fibonacci[9] = 34
```

**Pattern:** Client sends one request → Server returns stream of responses
**Use Case:** Real-time data feeds, logs, notifications

### 3️⃣ Client Streaming RPC
```
[Client] Sent: 10.5
[Server] Sum: Received 10.5, running total = 10.5
[Client] Sent: 20.3
[Server] Sum: Received 20.3, running total = 30.8
...
[Server] Sum: Total = 66.8 (from 5 numbers)
[Client] Sum result: 66.8
```

**Pattern:** Client sends stream of requests → Server returns one response
**Use Case:** File uploads, batch data processing, sensor data

### 4️⃣ Bidirectional Streaming RPC
```
[Server] Calculate: Initial value = 100
[Server] Calculate: 100 + 50 = 150
[Client] Current result: 150
[Server] Calculate: 150 × 2 = 300
[Client] Current result: 300
[Server] Calculate: 300 - 100 = 200
[Client] Current result: 200
[Server] Calculate: 200 ÷ 5 = 40
[Client] Current result: 40
```

**Pattern:** Both client and server send streams simultaneously
**Use Case:** Chat applications, collaborative editing, gaming

## Protocol Buffer Definition

The `calculator.proto` file defines the service contract:

```protobuf
syntax = "proto3";

option csharp_namespace = "GrpcExample";

service Calculator {
  // Unary RPC
  rpc Add (BinaryOperation) returns (CalculationResult);
  rpc Subtract (BinaryOperation) returns (CalculationResult);
  rpc Multiply (BinaryOperation) returns (CalculationResult);
  rpc Divide (BinaryOperation) returns (CalculationResult);

  // Server Streaming RPC
  rpc Fibonacci (FibonacciRequest) returns (stream NumberResult);

  // Client Streaming RPC
  rpc Sum (stream Number) returns (CalculationResult);

  // Bidirectional Streaming RPC
  rpc Calculate (stream Operation) returns (stream CalculationResult);
}

message BinaryOperation {
  double a = 1;
  double b = 2;
}

message CalculationResult {
  double result = 1;
  string operation = 2;
  bool success = 3;
  string error_message = 4;
}
```

## Key Features

### 1. HTTP/2 Transport
- **Multiplexing**: Multiple requests over single connection
- **Header Compression**: Reduced overhead
- **Binary Protocol**: Faster than text-based protocols

### 2. Protobuf Serialization
- **Compact**: 3-10x smaller than JSON
- **Fast**: 5-100x faster serialization
- **Strongly Typed**: Compile-time type safety
- **Backward Compatible**: Schema evolution support

### 3. Server Implementation
```csharp
public class CalculatorService : Calculator.CalculatorBase
{
    // Unary RPC
    public override Task<CalculationResult> Add(BinaryOperation request, ServerCallContext context)
    {
        var result = request.A + request.B;
        return Task.FromResult(new CalculationResult
        {
            Result = result,
            Operation = "Add",
            Success = true
        });
    }

    // Server Streaming RPC
    public override async Task Fibonacci(FibonacciRequest request, IServerStreamWriter<NumberResult> responseStream, ServerCallContext context)
    {
        long a = 0, b = 1;
        for (int i = 0; i < request.Count; i++)
        {
            await responseStream.WriteAsync(new NumberResult { Value = a, Index = i });
            var temp = a;
            a = b;
            b = temp + b;
        }
    }

    // Client Streaming RPC
    public override async Task<CalculationResult> Sum(IAsyncStreamReader<Number> requestStream, ServerCallContext context)
    {
        double sum = 0;
        await foreach (var number in requestStream.ReadAllAsync())
        {
            sum += number.Value;
        }
        return new CalculationResult { Result = sum, Success = true };
    }

    // Bidirectional Streaming RPC
    public override async Task Calculate(IAsyncStreamReader<Operation> requestStream, IServerStreamWriter<CalculationResult> responseStream, ServerCallContext context)
    {
        double accumulator = 0;
        await foreach (var operation in requestStream.ReadAllAsync())
        {
            // Process operation and stream results
            await responseStream.WriteAsync(new CalculationResult { Result = accumulator });
        }
    }
}
```

### 4. Client Implementation
```csharp
using var channel = GrpcChannel.ForAddress("http://localhost:5000");
var client = new Calculator.CalculatorClient(channel);

// Unary RPC
var result = await client.AddAsync(new BinaryOperation { A = 10, B = 5 });

// Server Streaming RPC
using var call = client.Fibonacci(new FibonacciRequest { Count = 10 });
await foreach (var number in call.ResponseStream.ReadAllAsync())
{
    Console.WriteLine($"Fibonacci[{number.Index}] = {number.Value}");
}

// Client Streaming RPC
using var sumCall = client.Sum();
await sumCall.RequestStream.WriteAsync(new Number { Value = 10.5 });
await sumCall.RequestStream.CompleteAsync();
var sumResult = await sumCall;

// Bidirectional Streaming RPC
using var calcCall = client.Calculate();
var readTask = Task.Run(async () =>
{
    await foreach (var result in calcCall.ResponseStream.ReadAllAsync())
    {
        Console.WriteLine($"Result: {result.Result}");
    }
});
await calcCall.RequestStream.WriteAsync(new Operation { Operand = 100 });
await calcCall.RequestStream.CompleteAsync();
await readTask;
```

## Performance Characteristics

### gRPC vs REST/JSON
| Metric | REST/JSON | gRPC |
|--------|-----------|------|
| **Serialization** | ~1ms | ~0.05ms (20x faster) |
| **Payload Size** | ~500 bytes | ~50 bytes (10x smaller) |
| **Throughput** | ~10K req/sec | ~100K req/sec (10x) |
| **Latency** | ~50ms | ~5ms (10x faster) |

### Why gRPC is Faster
1. **Binary Protobuf** - No text parsing overhead
2. **HTTP/2** - Connection multiplexing, header compression
3. **Streaming** - Continuous data flow without connection overhead
4. **Code Generation** - Optimized serialization code

## When to Use gRPC

### ✅ Perfect For
- **Microservices Communication** - Fast, efficient inter-service calls
- **Real-time Applications** - Bidirectional streaming for chat, gaming
- **Mobile Apps** - Efficient binary protocol saves bandwidth
- **IoT Devices** - Low overhead for resource-constrained devices
- **High-Performance APIs** - When speed matters

### ❌ Not Ideal For
- **Browser-based Apps** - Limited browser support (use gRPC-Web)
- **Public APIs** - REST/JSON more accessible
- **Human-readable APIs** - Protobuf is binary
- **Legacy Systems** - May not support HTTP/2

## Production Considerations

### Security
```csharp
// Enable TLS/SSL
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5001, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
        listenOptions.UseHttps();
    });
});

// Add authentication
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<AuthenticationInterceptor>();
});
```

### Logging & Monitoring
```csharp
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<LoggingInterceptor>();
    options.EnableDetailedErrors = true;
});
```

### Error Handling
```csharp
public override Task<CalculationResult> Divide(BinaryOperation request, ServerCallContext context)
{
    if (request.B == 0)
    {
        return Task.FromResult(new CalculationResult
        {
            Success = false,
            ErrorMessage = "Division by zero"
        });

        // Or throw RpcException for more control
        // throw new RpcException(new Status(StatusCode.InvalidArgument, "Division by zero"));
    }
    // ...
}
```

### Load Balancing
```csharp
// Client-side load balancing
var channel = GrpcChannel.ForAddress("dns:///my-service:5000");

// Or use service mesh (Istio, Linkerd)
// Or use reverse proxy (Envoy, YARP)
```

## Tools & Debugging

### grpcurl - CLI tool for testing
```bash
# List services
grpcurl -plaintext localhost:5000 list

# Call method
grpcurl -plaintext -d '{"a": 10, "b": 5}' localhost:5000 Calculator/Add
```

### gRPC Reflection
```csharp
// Already enabled in this example
builder.Services.AddGrpcReflection();
app.MapGrpcReflectionService();
```

### Postman
- Supports gRPC requests
- Import `.proto` files
- Test streaming RPCs

## Learning Path

1. **Start Here** - Run this example to see all 4 RPC types
2. **Modify** - Change the calculator logic, add new operations
3. **Explore** - Try server-side streaming for real-time data
4. **Build** - Create your own microservice with gRPC
5. **Advanced** - Add authentication, monitoring, load balancing

## Common Issues

### Issue: "Connection refused"
**Solution:** Ensure server is running on http://localhost:5000

### Issue: "gRPC call failed"
**Solution:** Check that both client and server use same `.proto` version

### Issue: "HTTP/2 not supported"
**Solution:** Update to .NET 8+ and ensure Kestrel is configured for HTTP/2

## References

- [gRPC Official Documentation](https://grpc.io/docs/)
- [gRPC for .NET](https://learn.microsoft.com/aspnet/core/grpc/)
- [Protocol Buffers](https://protobuf.dev/)
- [HTTP/2 Specification](https://httpwg.org/specs/rfc7540.html)

## Next Steps

After mastering this example, explore:
- **gRPC-Web** - gRPC for browsers
- **gRPC Gateway** - REST/JSON to gRPC bridge
- **Service Mesh** - Istio, Linkerd for production
- **OpenTelemetry** - Distributed tracing for gRPC

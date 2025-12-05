using Grpc.Core;
using Grpc.Net.Client;
using GrpcExample;

/// <summary>
/// gRPC Calculator Demo
///
/// Demonstrates:
/// - Unary RPC (request-response)
/// - Server streaming RPC (Fibonacci sequence)
/// - Client streaming RPC (sum of numbers)
/// - Bidirectional streaming RPC (calculator)
/// - Protobuf serialization
/// - HTTP/2 transport
/// </summary>

var runMode = args.Length > 0 ? args[0] : "both";

if (runMode == "server" || runMode == "both")
{
    // Start gRPC server in background
    var serverTask = Task.Run(() => RunServerAsync(args));
    await Task.Delay(2000); // Wait for server to start
}

if (runMode == "client" || runMode == "both")
{
    // Run client demonstrations
    await RunClientAsync();
}

if (runMode == "server")
{
    // Keep server running
    await Task.Delay(Timeout.Infinite);
}

Console.WriteLine("\n=== Demo Complete ===");
return;

// ============================================================================
// Server
// ============================================================================

async Task RunServerAsync(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    // Configure gRPC
    builder.Services.AddGrpc();
    builder.Services.AddGrpcReflection(); // Enable reflection for tools like grpcurl

    // Configure Kestrel to listen on port 5000
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenLocalhost(5000, listenOptions =>
        {
            listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
        });
    });

    var app = builder.Build();

    // Map gRPC services
    app.MapGrpcService<CalculatorService>();
    app.MapGrpcReflectionService();

    app.MapGet("/", () => "gRPC Calculator Service running on HTTP/2. Use gRPC client to connect.");

    Console.WriteLine("=== gRPC Server Started ===");
    Console.WriteLine("Listening on: http://localhost:5000");
    Console.WriteLine("Service: Calculator\n");

    await app.RunAsync();
}

// ============================================================================
// Client
// ============================================================================

async Task RunClientAsync()
{
    Console.WriteLine("\n=== gRPC Client Started ===\n");

    using var channel = GrpcChannel.ForAddress("http://localhost:5000");
    var client = new Calculator.CalculatorClient(channel);

    Console.WriteLine(new string('=', 60));
    Console.WriteLine("GRPC CALCULATOR DEMONSTRATIONS");
    Console.WriteLine(new string('=', 60) + "\n");

    // 1. Unary RPC
    await Demo1_UnaryRpcAsync(client);

    // 2. Server Streaming RPC
    await Demo2_ServerStreamingAsync(client);

    // 3. Client Streaming RPC
    await Demo3_ClientStreamingAsync(client);

    // 4. Bidirectional Streaming RPC
    await Demo4_BidirectionalStreamingAsync(client);
}

async Task Demo1_UnaryRpcAsync(Calculator.CalculatorClient client)
{
    Console.WriteLine("1️⃣  UNARY RPC (Request-Response)\n");

    // Add
    var addResult = await client.AddAsync(new BinaryOperation { A = 10, B = 5 });
    Console.WriteLine($"[Client] 10 + 5 = {addResult.Result}\n");

    // Subtract
    var subtractResult = await client.SubtractAsync(new BinaryOperation { A = 10, B = 5 });
    Console.WriteLine($"[Client] 10 - 5 = {subtractResult.Result}\n");

    // Multiply
    var multiplyResult = await client.MultiplyAsync(new BinaryOperation { A = 10, B = 5 });
    Console.WriteLine($"[Client] 10 × 5 = {multiplyResult.Result}\n");

    // Divide
    var divideResult = await client.DivideAsync(new BinaryOperation { A = 10, B = 5 });
    Console.WriteLine($"[Client] 10 ÷ 5 = {divideResult.Result}\n");

    // Division by zero
    var errorResult = await client.DivideAsync(new BinaryOperation { A = 10, B = 0 });
    if (!errorResult.Success)
    {
        Console.WriteLine($"[Client] 10 ÷ 0 = ERROR: {errorResult.ErrorMessage}\n");
    }

    Console.WriteLine("✅ Unary RPC: Simple request-response pattern\n");
}

async Task Demo2_ServerStreamingAsync(Calculator.CalculatorClient client)
{
    Console.WriteLine("2️⃣  SERVER STREAMING RPC (Fibonacci Sequence)\n");

    Console.WriteLine("[Client] Requesting Fibonacci sequence (10 numbers)...\n");

    using var call = client.Fibonacci(new FibonacciRequest { Count = 10 });

    await foreach (var number in call.ResponseStream.ReadAllAsync())
    {
        Console.WriteLine($"[Client] Fibonacci[{number.Index}] = {number.Value}");
        await Task.Delay(50);
    }

    Console.WriteLine("\n✅ Server Streaming: Server sends multiple responses\n");
}

async Task Demo3_ClientStreamingAsync(Calculator.CalculatorClient client)
{
    Console.WriteLine("3️⃣  CLIENT STREAMING RPC (Sum of Numbers)\n");

    Console.WriteLine("[Client] Streaming numbers to server...\n");

    using var call = client.Sum();

    var numbers = new[] { 10.5, 20.3, 15.7, 8.2, 12.1 };

    foreach (var num in numbers)
    {
        await call.RequestStream.WriteAsync(new Number { Value = num });
        Console.WriteLine($"[Client] Sent: {num}");
        await Task.Delay(200);
    }

    await call.RequestStream.CompleteAsync();

    var result = await call;
    Console.WriteLine($"\n[Client] Sum result: {result.Result}");
    Console.WriteLine($"[Client] Operation: {result.Operation}\n");

    Console.WriteLine("✅ Client Streaming: Client sends multiple requests\n");
}

async Task Demo4_BidirectionalStreamingAsync(Calculator.CalculatorClient client)
{
    Console.WriteLine("4️⃣  BIDIRECTIONAL STREAMING RPC (Calculator)\n");

    Console.WriteLine("[Client] Starting calculator session...\n");

    using var call = client.Calculate();

    // Start reading responses in background
    var readTask = Task.Run(async () =>
    {
        await foreach (var result in call.ResponseStream.ReadAllAsync())
        {
            Console.WriteLine($"[Client] Current result: {result.Result}");
        }
    });

    // Send operations
    var operations = new[]
    {
        new Operation { Operand = 100 },  // Initial value
        new Operation { Type = Operation.Types.OperationType.Add, Operand = 50 },
        new Operation { Type = Operation.Types.OperationType.Multiply, Operand = 2 },
        new Operation { Type = Operation.Types.OperationType.Subtract, Operand = 100 },
        new Operation { Type = Operation.Types.OperationType.Divide, Operand = 5 }
    };

    foreach (var op in operations)
    {
        await call.RequestStream.WriteAsync(op);
        await Task.Delay(300);
    }

    await call.RequestStream.CompleteAsync();
    await readTask;

    Console.WriteLine("\n✅ Bidirectional Streaming: Both send/receive streams\n");
}

// ============================================================================
// gRPC Service Implementation
// ============================================================================

public class CalculatorService : Calculator.CalculatorBase
{
    // Unary RPC: Add
    public override Task<CalculationResult> Add(BinaryOperation request, ServerCallContext context)
    {
        var result = request.A + request.B;
        Console.WriteLine($"[Server] Add: {request.A} + {request.B} = {result}");

        return Task.FromResult(new CalculationResult
        {
            Result = result,
            Operation = "Add",
            Success = true
        });
    }

    // Unary RPC: Subtract
    public override Task<CalculationResult> Subtract(BinaryOperation request, ServerCallContext context)
    {
        var result = request.A - request.B;
        Console.WriteLine($"[Server] Subtract: {request.A} - {request.B} = {result}");

        return Task.FromResult(new CalculationResult
        {
            Result = result,
            Operation = "Subtract",
            Success = true
        });
    }

    // Unary RPC: Multiply
    public override Task<CalculationResult> Multiply(BinaryOperation request, ServerCallContext context)
    {
        var result = request.A * request.B;
        Console.WriteLine($"[Server] Multiply: {request.A} × {request.B} = {result}");

        return Task.FromResult(new CalculationResult
        {
            Result = result,
            Operation = "Multiply",
            Success = true
        });
    }

    // Unary RPC: Divide
    public override Task<CalculationResult> Divide(BinaryOperation request, ServerCallContext context)
    {
        if (request.B == 0)
        {
            Console.WriteLine($"[Server] Divide: {request.A} ÷ 0 = ERROR");
            return Task.FromResult(new CalculationResult
            {
                Result = 0,
                Operation = "Divide",
                Success = false,
                ErrorMessage = "Division by zero"
            });
        }

        var result = request.A / request.B;
        Console.WriteLine($"[Server] Divide: {request.A} ÷ {request.B} = {result}");

        return Task.FromResult(new CalculationResult
        {
            Result = result,
            Operation = "Divide",
            Success = true
        });
    }

    // Server Streaming RPC: Fibonacci sequence
    public override async Task Fibonacci(FibonacciRequest request, IServerStreamWriter<NumberResult> responseStream, ServerCallContext context)
    {
        Console.WriteLine($"[Server] Fibonacci: Generating {request.Count} numbers");

        long a = 0, b = 1;

        for (int i = 0; i < request.Count; i++)
        {
            await responseStream.WriteAsync(new NumberResult
            {
                Value = a,
                Index = i
            });

            Console.WriteLine($"[Server] Fibonacci[{i}] = {a}");

            var temp = a;
            a = b;
            b = temp + b;

            await Task.Delay(100); // Simulate processing
        }

        Console.WriteLine("[Server] Fibonacci: Completed");
    }

    // Client Streaming RPC: Sum of numbers
    public override async Task<CalculationResult> Sum(IAsyncStreamReader<Number> requestStream, ServerCallContext context)
    {
        Console.WriteLine("[Server] Sum: Receiving numbers...");

        double sum = 0;
        int count = 0;

        await foreach (var number in requestStream.ReadAllAsync())
        {
            sum += number.Value;
            count++;
            Console.WriteLine($"[Server] Sum: Received {number.Value}, running total = {sum}");
        }

        Console.WriteLine($"[Server] Sum: Total = {sum} (from {count} numbers)");

        return new CalculationResult
        {
            Result = sum,
            Operation = $"Sum of {count} numbers",
            Success = true
        };
    }

    // Bidirectional Streaming RPC: Calculator
    public override async Task Calculate(IAsyncStreamReader<Operation> requestStream, IServerStreamWriter<CalculationResult> responseStream, ServerCallContext context)
    {
        Console.WriteLine("[Server] Calculate: Bidirectional streaming started");

        double accumulator = 0;
        bool first = true;

        await foreach (var operation in requestStream.ReadAllAsync())
        {
            if (first)
            {
                accumulator = operation.Operand;
                first = false;
                Console.WriteLine($"[Server] Calculate: Initial value = {accumulator}");
                continue;
            }

            var oldValue = accumulator;

            switch (operation.Type)
            {
                case Operation.Types.OperationType.Add:
                    accumulator += operation.Operand;
                    Console.WriteLine($"[Server] Calculate: {oldValue} + {operation.Operand} = {accumulator}");
                    break;
                case Operation.Types.OperationType.Subtract:
                    accumulator -= operation.Operand;
                    Console.WriteLine($"[Server] Calculate: {oldValue} - {operation.Operand} = {accumulator}");
                    break;
                case Operation.Types.OperationType.Multiply:
                    accumulator *= operation.Operand;
                    Console.WriteLine($"[Server] Calculate: {oldValue} × {operation.Operand} = {accumulator}");
                    break;
                case Operation.Types.OperationType.Divide:
                    if (operation.Operand != 0)
                    {
                        accumulator /= operation.Operand;
                        Console.WriteLine($"[Server] Calculate: {oldValue} ÷ {operation.Operand} = {accumulator}");
                    }
                    else
                    {
                        Console.WriteLine($"[Server] Calculate: Division by zero ignored");
                    }
                    break;
            }

            await responseStream.WriteAsync(new CalculationResult
            {
                Result = accumulator,
                Operation = operation.Type.ToString(),
                Success = true
            });

            await Task.Delay(50);
        }

        Console.WriteLine("[Server] Calculate: Bidirectional streaming completed");
    }
}

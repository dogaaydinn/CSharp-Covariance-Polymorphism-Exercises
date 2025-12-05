# gRPC Example

> High-performance RPC with Protocol Buffers.

## Features
- **gRPC** - Modern RPC framework
- **Protocol Buffers** - Efficient binary serialization
- **HTTP/2** - Multiplexing, streaming

## Structure
```
Protos/calculator.proto  # Service definition
Services/CalculatorService.cs  # Implementation
```

## Run
```bash
dotnet run
```

## Test (requires gRPC client)
```bash
# Using grpcurl:
grpcurl -plaintext -d '{"a": 10, "b": 32}' localhost:5000 Calculator/Add
```

## Benefits
- **10-20x faster** than REST (binary vs JSON)
- **Type-safe** - Generated client code
- **Streaming** - Bidirectional communication

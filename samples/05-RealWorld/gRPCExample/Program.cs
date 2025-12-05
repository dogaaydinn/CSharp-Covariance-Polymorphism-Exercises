using gRPCExample.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<CalculatorService>();
app.MapGet("/", () => "gRPC endpoint. Use gRPC client to connect.");

app.Run();

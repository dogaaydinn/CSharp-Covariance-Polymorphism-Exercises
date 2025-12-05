# Azure Functions

> Serverless compute - run code without managing servers.

## Features
- **HTTP Trigger** - Respond to HTTP requests
- **Pay-per-execution** - No cost when idle
- **Auto-scale** - Handles 1 to 1M requests
- **Isolated Worker** - .NET 8 isolated process model

## Run Locally
```bash
# Install Azure Functions Core Tools
# brew install azure-functions-core-tools (macOS)
# choco install azure-functions-core-tools (Windows)

cd samples/06-CuttingEdge/AzureFunctions
func start

# Or with dotnet
dotnet run
```

## Test
```bash
curl -X POST http://localhost:7071/api/ImageResize \
  -H "Content-Type: application/json" \
  -d '{"imageUrl": "photo.jpg"}'
```

## Deploy to Azure
```bash
func azure functionapp publish <function-app-name>
```

**Use Cases:** Image processing, scheduled jobs, webhooks, data transformation.

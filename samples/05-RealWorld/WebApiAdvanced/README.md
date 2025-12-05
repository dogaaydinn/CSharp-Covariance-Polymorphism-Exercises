# Web API Advanced

> Production-ready Minimal API with rate limiting, OpenAPI, and modern patterns.

## Features
- **Minimal API** - .NET 8 minimal endpoints
- **Rate Limiting** - 5 requests per 10 seconds
- **Swagger/OpenAPI** - Auto-generated documentation
- **CRUD operations** - Product management

## Quick Start
```bash
cd samples/05-RealWorld/WebApiAdvanced
dotnet run
# Navigate to: https://localhost:5001/swagger
```

## Test Rate Limiting
```bash
# Make 6 requests (6th will be rate-limited)
for i in {1..6}; do curl http://localhost:5000/products; done
```

## Endpoints
- `GET /products` - List all products (rate limited)
- `GET /products/{id}` - Get single product
- `POST /products` - Create product

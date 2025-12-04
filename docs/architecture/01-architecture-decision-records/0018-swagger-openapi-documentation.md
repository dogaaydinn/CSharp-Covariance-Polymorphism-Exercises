# ADR-0018: Swagger/OpenAPI for API Documentation

**Status:** Accepted
**Date:** 2025-12-02
**Deciders:** Architecture Team
**Technical Story:** API documentation and testing

## Context

RESTful APIs need documentation for developers to understand endpoints, request/response models, and authentication. Manual documentation becomes outdated quickly.

## Decision

Use **Swagger UI** with OpenAPI specification for interactive API documentation.

Configuration:
- Swashbuckle.AspNetCore 6.5.0+
- Swagger UI at `/swagger`
- OpenAPI JSON at `/swagger/v1/swagger.json`
- Enabled in development only

## Consequences

### Positive

- **Interactive**: Test endpoints directly from browser
- **Up-to-Date**: Generated from code (never outdated)
- **Discoverable**: Easy for developers to explore API
- **Code-First**: No separate documentation file
- **Client Generation**: Can generate client SDKs

### Negative

- **Development Only**: Should disable in production
- **Performance**: Small overhead (negligible in dev)

## Implementation

```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/videos", async (VideoDbContext db) => { })
   .WithName("GetVideos")
   .WithOpenApi()
   .Produces<List<Video>>(200)
   .ProducesProblem(500);
```

## Notes

- Access Swagger at: `http://localhost:{port}/swagger`
- Customize with XML comments: `builder.Services.AddSwaggerGen(c => c.IncludeXmlComments("..."))`
- Production: Use Scalar or Redoc for documentation

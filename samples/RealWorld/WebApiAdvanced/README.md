# Web API Advanced - Production Patterns

> **Level:** Real-World  
> **Framework:** ASP.NET Core 8.0  
> **Patterns:** JWT Auth, Rate Limiting, Caching, Validation

## 📚 Overview

Production-ready Web API demonstrating authentication, authorization, caching, rate limiting, validation, error handling, and observability patterns.

## 🎯 Key Features

### Authentication & Authorization
- ✅ **JWT Bearer Tokens** - Stateless authentication
- ✅ **Policy-Based Authorization** - Role and claim-based access
- ✅ **Refresh Tokens** - Secure token rotation
- ✅ **API Keys** - Service-to-service auth

### Performance
- ✅ **Response Caching** - HTTP caching headers
- ✅ **Redis Distributed Cache** - Shared cache across instances
- ✅ **Output Caching** - ASP.NET Core 7+ output cache
- ✅ **Compression** - Gzip/Brotli response compression

### Security
- ✅ **Rate Limiting** - Per-user/IP throttling
- ✅ **CORS** - Cross-origin resource sharing
- ✅ **HTTPS Redirection** - Force secure connections
- ✅ **Security Headers** - HSTS, X-Frame-Options, CSP

### Validation & Error Handling
- ✅ **FluentValidation** - Request validation
- ✅ **Global Exception Handling** - Consistent error responses
- ✅ **Problem Details** - RFC 7807 compliant errors
- ✅ **Model State Validation** - Automatic model validation

### Observability
- ✅ **Structured Logging** - Serilog integration
- ✅ **Health Checks** - /health/live, /health/ready
- ✅ **OpenTelemetry** - Distributed tracing
- ✅ **Prometheus Metrics** - /metrics endpoint

### Documentation
- ✅ **Swagger/OpenAPI** - Interactive API docs
- ✅ **XML Comments** - IntelliSense documentation
- ✅ **API Versioning** - v1, v2 support
- ✅ **Examples** - Request/response samples

## 🚀 Quick Start

```bash
cd samples/05-RealWorld/WebApiAdvanced
dotnet run

# Navigate to https://localhost:5001/swagger
```

## 📊 API Endpoints

### Authentication
```http
POST /api/v1/auth/login
POST /api/v1/auth/refresh
POST /api/v1/auth/revoke
```

### Products (Example Resource)
```http
GET    /api/v1/products          # List all (cached)
GET    /api/v1/products/{id}     # Get single (cached)
POST   /api/v1/products           # Create (requires auth)
PUT    /api/v1/products/{id}      # Update (requires auth)
DELETE /api/v1/products/{id}      # Delete (requires admin)
```

### Health & Metrics
```http
GET /health/live                  # Liveness probe
GET /health/ready                 # Readiness probe
GET /metrics                      # Prometheus metrics
```

## 🔧 Configuration

### appsettings.json
```json
{
  "Jwt": {
    "SecretKey": "your-256-bit-secret",
    "Issuer": "your-api",
    "Audience": "your-clients",
    "ExpirationMinutes": 60
  },
  "RateLimiting": {
    "PermitLimit": 100,
    "Window": 60
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  }
}
```

### Environment Variables
```bash
ASPNETCORE_ENVIRONMENT=Production
JWT__SECRET_KEY=<secret>
REDIS__CONNECTION_STRING=<redis-url>
```

## 📈 Performance Optimizations

### Response Caching
```csharp
[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
public async Task<IActionResult> Get(int id)
{
    // Cached for 60 seconds
}
```

### Distributed Caching
```csharp
var cachedData = await _cache.GetStringAsync(key);
if (cachedData == null)
{
    cachedData = await LoadFromDatabase();
    await _cache.SetStringAsync(key, cachedData, 
        new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });
}
```

### Rate Limiting
```csharp
[EnableRateLimiting("fixed")]
public async Task<IActionResult> Post([FromBody] CreateProductRequest request)
{
    // Rate limited to 100 requests per minute
}
```

## 🔐 Security Best Practices

### JWT Authentication
```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]))
        };
    });
```

### Policy-Based Authorization
```csharp
services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("CanEdit", policy => policy.RequireClaim("permission", "edit"));
});

[Authorize(Policy = "AdminOnly")]
public async Task<IActionResult> Delete(int id) { }
```

## 🎯 Real-World Scenarios

### E-Commerce API
- Product catalog with caching
- Order processing with validation
- User authentication with JWT
- Rate limiting for public endpoints

### SaaS API
- Multi-tenant architecture
- API key authentication
- Usage tracking and billing
- Webhook integrations

### Mobile Backend
- Token refresh mechanism
- Offline data sync
- Push notifications
- Analytics tracking

## 📦 Docker Deployment

### Dockerfile
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "WebApiAdvanced.dll"]
```

### docker-compose.yml
```yaml
version: '3.8'
services:
  api:
    build: .
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - Redis__ConnectionString=redis:6379
    depends_on:
      - redis
  
  redis:
    image: redis:alpine
    ports:
      - "6379:6379"
```

## 🔗 Further Reading

- [ASP.NET Core Security](https://docs.microsoft.com/en-us/aspnet/core/security/)
- [JWT Best Practices](https://tools.ietf.org/html/rfc8725)
- [API Design Guidelines](https://github.com/microsoft/api-guidelines)
- [Rate Limiting Patterns](https://docs.microsoft.com/en-us/aspnet/core/performance/rate-limit)

---

**Production-Ready Web API! 🌐**

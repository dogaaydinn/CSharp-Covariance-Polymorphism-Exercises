# ADR-0011: Service Discovery Pattern

**Status:** Accepted
**Date:** 2025-12-02
**Deciders:** Architecture Team
**Technical Story:** Inter-service communication in microservices

## Context

In microservices architecture, services need to communicate with each other. Services are deployed across multiple containers/pods with dynamic IP addresses and ports. Traditional approaches using hardcoded URLs don't work because:
- IPs/ports change on deployment
- Kubernetes assigns random ports
- Horizontal scaling adds/removes instances
- Load balancing requires knowing all instances

Traditional approach (doesn't scale):
```csharp
var client = new HttpClient();
client.BaseAddress = new Uri("http://localhost:5001"); // Breaks in production!
```

## Decision

We will use **.NET Aspire's built-in Service Discovery** for all inter-service communication.

Implementation:
- Services reference each other by **name** (not URL)
- AppHost defines service relationships via `WithReference()`
- ServiceDefaults configures `AddServiceDiscovery()` and `AddStandardResilienceHandler()`
- HttpClient automatically resolves service names to endpoints

Example:
```csharp
// AppHost
var processingService = builder.AddProject("videoprocessing", ...);
var apiService = builder.AddProject("api", ...)
    .WithReference(processingService);

// API Service
builder.Services.AddHttpClient<VideoProcessingClient>(client =>
{
    client.BaseAddress = new Uri("http://videoprocessing"); // ← Service name!
});
```

## Consequences

### Positive

- **Zero Configuration**: No hardcoded IPs or ports
- **Environment Agnostic**: Same code works in dev, staging, production
- **Dynamic Discovery**: Handles service restarts, scaling, failovers
- **Type Safety**: Compile-time checking with AppHost `WithReference()`
- **Load Balancing**: Automatic load balancing across instances
- **Resilience**: Built-in retry, circuit breaker, timeout
- **Cloud-Native**: Works with Kubernetes, Docker, cloud services
- **Testability**: Easy to mock/stub services in tests

### Negative

- **Aspire Dependency**: Tightly coupled to Aspire (production requires alternative like Consul)
- **Learning Curve**: Developers must understand service discovery concepts
- **Debugging**: Harder to trace exact endpoint being called
- **Production Gap**: Aspire service discovery is dev-only (prod uses Kubernetes/Consul)

### Neutral

- **DNS-Based**: Uses DNS resolution under the hood
- **Caching**: Client caches resolved endpoints (must handle updates)

## Alternatives Considered

### Alternative 1: Hardcoded URLs with Configuration

**Pros:**
- **Simple**: Just set URL in appsettings.json
- **Explicit**: Clear what endpoint is being called
- **No Dependencies**: Works without service discovery

**Cons:**
- **Environment-Specific**: Different URLs for dev/staging/prod
- **Manual Management**: Must update configs on every change
- **No Load Balancing**: Single endpoint (must add reverse proxy)
- **Not Cloud-Native**: Doesn't work with Kubernetes dynamic IPs
- **Error-Prone**: Easy to misconfigure

**Why rejected:** Doesn't scale to cloud-native deployments. Service discovery is industry standard.

### Alternative 2: Consul

**Pros:**
- **Production-Grade**: Battle-tested by Netflix, Uber
- **Service Mesh**: Advanced features (health checks, KV store)
- **Multi-Cloud**: Works anywhere
- **Strong Consistency**: Raft consensus for service registry

**Cons:**
- **Complex Setup**: Requires Consul cluster (3-5 nodes)
- **Operational Overhead**: Must manage Consul infrastructure
- **Learning Curve**: Steep learning curve
- **Overkill for Dev**: Too complex for local development

**Why rejected:** Overkill for educational sample. Aspire provides service discovery for dev; production uses Kubernetes.

### Alternative 3: Kubernetes Service DNS

**Pros:**
- **Native**: Built into Kubernetes
- **Zero Config**: Services automatically discoverable
- **Reliable**: Part of Kubernetes core

**Cons:**
- **Kubernetes Only**: Doesn't work in dev without Kubernetes
- **No Local Dev**: Must run Minikube/Kind locally (slow)
- **Limited Features**: Basic DNS-based discovery only

**Why rejected:** Not suitable for local development. Aspire provides similar experience without Kubernetes.

### Alternative 4: Manual DNS with Docker Compose

**Pros:**
- **Simple**: Docker Compose provides DNS
- **Standard**: Works with any Docker setup

**Cons:**
- **No Resilience**: No retry/circuit breaker
- **No Load Balancing**: Single instance only (or manual nginx)
- **Limited**: Cannot express complex service relationships
- **No Observability**: No built-in tracing

**Why rejected:** Aspire provides service discovery + resilience + observability in one package.

### Alternative 5: Service Mesh (Istio, Linkerd)

**Pros:**
- **Enterprise-Grade**: Advanced traffic management
- **Security**: mTLS, authorization
- **Observability**: Distributed tracing built-in

**Cons:**
- **Extremely Complex**: Requires deep Kubernetes knowledge
- **Performance Overhead**: Sidecar proxies add latency
- **Not for Dev**: Impossible to run locally
- **Overkill**: Too much for simple microservices

**Why rejected:** Service mesh is for production at scale. Aspire is for development.

## Related Decisions

- [ADR-0002](0002-using-dotnet-aspire.md): Aspire provides service discovery
- [ADR-0019](0019-apphost-orchestration.md): AppHost defines service relationships
- [ADR-0013](0013-servicedefaults-pattern.md): ServiceDefaults configures discovery

## Related Links

- [Service Discovery Pattern](https://microservices.io/patterns/client-side-discovery.html)
- [Aspire Service Discovery](https://learn.microsoft.com/dotnet/aspire/service-discovery/overview)
- [Microsoft.Extensions.ServiceDiscovery](https://www.nuget.org/packages/Microsoft.Extensions.ServiceDiscovery)

## Notes

- **How It Works**:
  1. AppHost registers services with discovery provider (dev: in-memory, prod: Kubernetes DNS)
  2. HttpClient configured with `AddServiceDiscovery()`
  3. On request, HttpClient queries discovery provider
  4. Discovery provider returns endpoint (IP:port)
  5. HttpClient caches endpoint for performance

- **Production Deployment**:
  - **Kubernetes**: Use Kubernetes Service DNS
  - **Azure**: Azure Service Fabric / Container Apps service discovery
  - **AWS**: AWS Cloud Map / ECS service discovery
  - **Manual**: Consul, Eureka, or custom implementation

- **Resilience Integration**:
  ```csharp
  builder.Services.ConfigureHttpClientDefaults(http =>
  {
      http.AddServiceDiscovery(); // Resolve service names
      http.AddStandardResilienceHandler(); // Retry + circuit breaker
  });
  ```

- **Monitoring**:
  - OpenTelemetry traces include resolved endpoints
  - Aspire Dashboard shows service dependencies graph
  - Log endpoint resolution for debugging

- **Testing Strategy**:
  - Unit tests: Mock `IHttpClientFactory`
  - Integration tests: Use real service discovery (AppHost)
  - E2E tests: Deploy to Kubernetes and test real discovery

- **Service Naming Convention**:
  - Lowercase, hyphen-separated: `video-processing`, `api`, `web`
  - Avoid special characters (DNS-safe names)
  - Consistent with project names

- **Future**: .NET 9 improves service discovery with better caching and endpoint selection strategies

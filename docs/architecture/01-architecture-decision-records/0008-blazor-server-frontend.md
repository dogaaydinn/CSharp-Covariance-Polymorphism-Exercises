# ADR-0008: Blazor Server for Web Frontend

**Status:** Accepted
**Date:** 2025-12-02
**Deciders:** Architecture Team
**Technical Story:** Frontend technology selection for video management UI

## Context

The AspireVideoService needs a web frontend for:
- Browsing video library
- Adding new videos
- Viewing video details and status
- Incrementing view counts
- Demonstrating service discovery from frontend

Requirements:
- .NET-based (consistent technology stack)
- Modern UI framework
- Real-time updates support
- Simple deployment
- Type-safe API consumption
- Educational value (demonstrate .NET full-stack)

## Decision

We will use **Blazor Server** for the web frontend.

Configuration:
- Rendering Mode: Interactive Server
- API Communication: HttpClient with service discovery
- State Management: Component state (scoped)
- UI Framework: Bootstrap 5 (included in template)

## Consequences

### Positive

- **C# Full-Stack**: Write frontend in C# (consistent with backend)
- **Type Safety**: Compile-time checking of API models
- **Code Sharing**: Share DTOs and models between API and Web
- **No JavaScript**: Minimal JavaScript required
- **Real-Time**: SignalR built-in for real-time updates
- **Tooling**: Full Visual Studio/Rider IntelliSense support
- **Debugging**: Server-side debugging with breakpoints
- **Simple Deployment**: Single .NET deployment (no Node.js/npm)
- **Aspire Integration**: First-class Aspire support with service discovery
- **Security**: Server-side rendering reduces attack surface

### Negative

- **SignalR Dependency**: Requires persistent WebSocket connection
- **Server Load**: UI state maintained on server (memory per user)
- **Latency**: Every UI interaction requires server round-trip (50-200ms)
- **Scalability**: More challenging to scale than stateless SPAs
- **Offline**: Cannot work offline (requires server connection)
- **Bandwidth**: More bandwidth than Blazor WebAssembly for large payloads

### Neutral

- **Learning Curve**: Developers must learn Blazor component model
- **Ecosystem**: Smaller ecosystem than React/Angular (but growing)
- **Browser Support**: Requires modern browsers (SignalR requirement)

## Alternatives Considered

### Alternative 1: Blazor WebAssembly

**Pros:**
- **Client-Side**: No server resources for UI state
- **Offline**: Can work offline once loaded
- **Scalability**: Pure static files (CDN-friendly)
- **Low Latency**: No round-trips for UI interactions

**Cons:**
- **Download Size**: 2-3MB initial download (.NET runtime in browser)
- **Startup Time**: 3-5 second cold start
- **No Real-Time**: Must implement own SignalR client
- **API Authentication**: More complex (JWT tokens, CORS)
- **Limited APIs**: Cannot use all .NET APIs (file system, etc.)

**Why rejected:** Slower initial load and more complex for educational sample. Blazor Server provides better developer experience for this use case.

### Alternative 2: React + TypeScript

**Pros:**
- **Industry Standard**: Most popular frontend framework
- **Rich Ecosystem**: Massive component library ecosystem
- **Performance**: Highly optimized, fast rendering
- **Developer Pool**: Easier to find React developers

**Cons:**
- **Separate Stack**: JavaScript/TypeScript (not .NET)
- **Build Tooling**: Requires Node.js, npm, Webpack/Vite
- **No Type Sharing**: Cannot share C# types with API
- **API Typing**: Must manually create TypeScript types or use codegen
- **Deployment**: Separate deployment pipeline

**Why rejected:** Educational sample focuses on .NET ecosystem. React would dilute the message and add complexity (Node.js setup, npm, etc.).

### Alternative 3: ASP.NET Core MVC (Razor Pages)

**Pros:**
- **Mature**: 10+ years of production use
- **Simple**: Server-side rendering, no JavaScript
- **Performance**: Fast page loads
- **SEO**: Great for public-facing sites

**Cons:**
- **Not SPA**: Full page reloads on navigation
- **Limited Interactivity**: Requires JavaScript for dynamic UI
- **Old-School**: Doesn't demonstrate modern .NET frontend
- **No Real-Time**: Must implement SignalR manually

**Why rejected:** Blazor is the modern .NET frontend approach. MVC/Razor Pages feel dated compared to SPAs.

### Alternative 4: Angular

**Pros:**
- **Enterprise**: Strong in enterprise environments
- **Complete Framework**: Batteries-included
- **TypeScript First**: Built for TypeScript

**Cons:**
- **Separate Stack**: JavaScript/TypeScript ecosystem
- **Complex**: Steeper learning curve than React
- **Heavyweight**: Larger bundle sizes
- **Declining Popularity**: Losing market share to React/Vue

**Why rejected:** Same reasons as React, plus Angular is more complex and less popular.

### Alternative 5: Vue.js

**Pros:**
- **Simple**: Easier learning curve than React/Angular
- **Lightweight**: Smaller bundle sizes
- **Progressive**: Can adopt incrementally

**Cons:**
- **Separate Stack**: JavaScript/TypeScript
- **Smaller Ecosystem**: Fewer enterprise-grade components
- **Less Adoption**: Smaller in enterprise compared to React

**Why rejected:** Same reasons as React/Angular.

## Related Decisions

- [ADR-0001](0001-adopting-dotnet-8-platform.md): Blazor Server requires .NET 8
- [ADR-0002](0002-using-dotnet-aspire.md): Aspire provides Blazor integration
- [ADR-0011](0011-service-discovery-pattern.md): Blazor uses service discovery for API calls

## Related Links

- [Blazor Documentation](https://learn.microsoft.com/aspnet/core/blazor/)
- [Blazor Server vs WebAssembly](https://learn.microsoft.com/aspnet/core/blazor/hosting-models)
- [Aspire Blazor Component](https://learn.microsoft.com/dotnet/aspire/frameworks/framework-blazor)
- [SignalR Documentation](https://learn.microsoft.com/aspnet/core/signalr/)

## Notes

- **When to Use Blazor Server**:
  - Internal admin tools
  - Line-of-business apps
  - Real-time dashboards
  - Educational samples (like this)

- **When to Use Blazor WebAssembly**:
  - Public-facing websites
  - PWAs (Progressive Web Apps)
  - Offline-first applications
  - High-traffic sites (CDN-friendly)

- **Performance Optimization**:
  - Use `@rendermode InteractiveServer` for interactive components only
  - Static rendering for read-only content
  - Virtualization for large lists (`<Virtualize>`)
  - Pre-rendering for faster initial load

- **API Communication Pattern**:
  ```csharp
  @inject IHttpClientFactory HttpClientFactory

  var client = HttpClientFactory.CreateClient("api");
  var videos = await client.GetFromJsonAsync<List<Video>>("/api/videos");
  ```

- **State Management**:
  - Component state for simple scenarios (this sample)
  - Fluxor for complex state management (future)
  - Cascading parameters for cross-component state

- **Production Considerations**:
  - Enable compression for SignalR
  - Configure circuit timeout (default 30 seconds)
  - Monitor SignalR connection counts
  - Use sticky sessions with load balancers
  - Consider Redis backplane for multi-server deployments

- **Future**: Blazor United (.NET 8+) combines Server and WebAssembly for best of both worlds

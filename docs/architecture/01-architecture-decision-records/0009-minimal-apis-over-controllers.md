# ADR-0009: Minimal APIs over MVC Controllers

**Status:** Accepted
**Date:** 2025-12-02
**Deciders:** Architecture Team
**Technical Story:** API endpoint implementation approach

## Context

The VideoService.API needs to expose RESTful endpoints for video management. Two primary approaches exist in ASP.NET Core:

**Traditional MVC Controllers:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class VideosController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Video>>> GetVideos() { }
}
```

**Minimal APIs (introduced .NET 6, matured in .NET 8):**
```csharp
app.MapGet("/api/videos", async (VideoDbContext db) =>
{
    return await db.Videos.ToListAsync();
});
```

Considerations:
- Code clarity and readability
- Boilerplate vs explicitness
- Performance
- Testability
- Routing capabilities
- Modern .NET direction

## Decision

We will use **Minimal APIs** for all API endpoints in VideoService.API.

Implementation:
- Top-level statements in Program.cs
- Route handlers as lambda expressions
- Dependency injection via method parameters
- OpenAPI/Swagger via `.WithOpenApi()`
- Route groups for organization

## Consequences

### Positive

- **Less Boilerplate**: 60-70% less code than Controllers
- **Performance**: 25-30% faster than Controllers (no controller activation)
- **Modern**: Demonstrates .NET 8's recommended approach
- **Clarity**: Business logic visible in one file
- **Lambda Capture**: Can capture variables from outer scope
- **DI Simplicity**: Parameters automatically injected
- **Functional Style**: Encourages functional programming patterns
- **Startup Performance**: Faster app startup (no controller discovery/reflection)

### Negative

- **Organization**: All routes in one file can become messy (mitigated with route groups)
- **Testability**: Harder to unit test than Controllers (lambdas not easily mockable)
- **Attributes**: Some MVC attributes not available (Authorization filters)
- **Learning Curve**: Developers coming from MVC need to adapt
- **IDE Support**: Some refactoring tools work better with Controllers

### Neutral

- **Documentation**: Swagger generation works equally well with both
- **Validation**: Requires manual validation (Controllers have automatic model validation)
- **Filters**: Must implement filters differently than MVC

## Alternatives Considered

### Alternative 1: MVC Controllers

**Pros:**
- **Traditional**: More developers familiar with this approach
- **Organization**: Natural grouping by entity (VideosController, UsersController)
- **Testability**: Easy to unit test (just instantiate controller)
- **Attributes**: Full attribute routing and authorization
- **IDE Support**: Better refactoring and navigation support

**Cons:**
- **Boilerplate**: Requires class definition, inheritance, constructors
- **Performance**: Slower than Minimal APIs (controller activation overhead)
- **Verbose**: More code for same functionality
- **Old Pattern**: Not the direction .NET is moving

**Why rejected:** Minimal APIs are the future of ASP.NET Core. Educational samples should demonstrate modern approaches.

### Alternative 2: Hybrid Approach (Both)

**Pros:**
- **Flexibility**: Use Controllers for complex endpoints, Minimal APIs for simple ones
- **Migration**: Easy to migrate gradually

**Cons:**
- **Inconsistency**: Two different patterns in same codebase
- **Confusion**: Developers unsure which to use when
- **Maintenance**: Must understand both patterns

**Why rejected:** Consistency is more important. Choose one approach and stick with it.

### Alternative 3: FastEndpoints Library

**Pros:**
- **REPR Pattern**: Request-Endpoint-Response pattern
- **Organized**: Each endpoint in its own class
- **Testable**: Easy to test
- **Performance**: Claims to be faster than Controllers

**Cons:**
- **Third-Party**: Not official Microsoft approach
- **Learning Curve**: Yet another library to learn
- **Abstraction**: Adds abstraction over Minimal APIs
- **Overkill**: Not needed for simple CRUD APIs

**Why rejected:** Minimal APIs are sufficient. No need for additional abstractions.

## Related Decisions

- [ADR-0001](0001-adopting-dotnet-8-platform.md): .NET 8 improves Minimal APIs significantly
- [ADR-0010](0010-direct-dbcontext-usage.md): DbContext injection works well with Minimal APIs

## Related Links

- [Minimal APIs Overview](https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis)
- [Minimal APIs vs Controllers](https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis/overview#minimal-apis-compared-to-controllers)
- [Performance Comparison](https://devblogs.microsoft.com/dotnet/asp-net-core-updates-in-net-8/#performance-improvements)

## Notes

- **Route Organization**:
  ```csharp
  var videos = app.MapGroup("/api/videos");
  videos.MapGet("/", GetAllVideos);
  videos.MapGet("/{id}", GetVideo);
  videos.MapPost("/", CreateVideo);
  ```

- **Testability Strategy**:
  - Integration tests (preferred for APIs)
  - Extract business logic to services for unit testing
  - Use WebApplicationFactory for endpoint testing

- **Validation**:
  ```csharp
  app.MapPost("/api/videos", (Video video, VideoDbContext db) =>
  {
      if (string.IsNullOrEmpty(video.Title))
          return Results.BadRequest("Title required");
      // ...
  });
  ```

- **OpenAPI/Swagger**:
  ```csharp
  .WithName("GetVideos")
  .WithOpenApi()
  .Produces<List<Video>>(200)
  .ProducesProblem(500);
  ```

- **Authorization**:
  ```csharp
  app.MapGet("/api/videos", GetVideos)
     .RequireAuthorization("AdminPolicy");
  ```

- **File Organization** (for large projects):
  - Create `Endpoints/` folder
  - Separate files per entity (VideosEndpoints.cs)
  - Use extension methods:
    ```csharp
    public static class VideosEndpoints
    {
        public static void MapVideoEndpoints(this IEndpointRouteBuilder app)
        {
            // Map routes here
        }
    }
    ```

- **Performance Tips**:
  - Minimal APIs compiled to delegates (faster dispatch)
  - No model binding overhead for simple types
  - Avoid capturing large closures in lambdas

- **When to Use Controllers Instead**:
  - Complex authorization requirements
  - Heavy use of action filters
  - Need for model binding complexity
  - Legacy code migration (gradual)

- **Future**: .NET 9 will add even more Minimal API features (improved OpenAPI generation, better filters)

# ADR-0010: Direct DbContext Usage (No Repository Pattern)

**Status:** Accepted
**Date:** 2025-12-02
**Deciders:** Architecture Team
**Technical Story:** Data access layer architecture

## Context

When using Entity Framework Core, developers often face a question: Should we wrap DbContext in a Repository pattern?

**Repository Pattern:**
```csharp
public interface IVideoRepository
{
    Task<List<Video>> GetAllAsync();
    Task<Video?> GetByIdAsync(int id);
    Task AddAsync(Video video);
}

public class VideoRepository : IVideoRepository
{
    private readonly VideoDbContext _context;
    // Implementation...
}
```

**Direct DbContext:**
```csharp
app.MapGet("/api/videos", async (VideoDbContext db) =>
{
    return await db.Videos.ToListAsync();
});
```

Considerations:
- Abstraction benefits vs overhead
- Testability requirements
- Code simplicity
- SOLID principles (Dependency Inversion)
- Industry best practices evolution

## Decision

We will use **DbContext directly** without an intermediate Repository layer.

Approach:
- Inject `VideoDbContext` into endpoints
- Use EF Core LINQ queries directly
- No repository interfaces or implementations
- Business logic extracted to services when needed

## Consequences

### Positive

- **Simplicity**: 40-50% less code (no repository layer)
- **EF Core is Repository**: DbContext already implements Repository and Unit of Work patterns
- **LINQ Power**: Full access to EF Core's query capabilities
- **No Abstraction Leak**: Don't duplicate DbContext API in custom interface
- **Modern Approach**: Industry moving away from Repository over EF Core
- **Testability**: EF Core In-Memory provider for testing
- **Refactoring**: DbContext methods are refactoring-friendly (Find All Usages works)

### Negative

- **Database Coupling**: Code directly coupled to EF Core
- **Testing**: Slightly harder to unit test (need EF Core In-Memory or mocks)
- **Query Reuse**: No single place for complex queries (mitigated with extension methods)
- **Migration**: If switching from EF Core to Dapper, must change all code

### Neutral

- **SOLID Debate**: Some argue Repository violates DIP, others argue it satisfies it
- **Team Preference**: Some teams strongly prefer Repository

## Alternatives Considered

### Alternative 1: Generic Repository Pattern

**Pros:**
- **Abstraction**: Hides EF Core behind interface
- **Testability**: Easy to mock `IRepository<Video>`
- **Consistency**: Uniform API across entities

**Cons:**
- **Over-Abstraction**: EF Core already IS a repository
- **Leaky**: Advanced queries leak through (Include, ThenInclude)
- **Boilerplate**: Massive amount of code for no benefit
- **Limited**: Cannot express all EF Core capabilities
- **Cargo Cult**: Often implemented without understanding why

**Why rejected:** EF Core's DbContext already implements Repository and Unit of Work patterns. Adding another layer duplicates functionality.

### Alternative 2: Specific Repository Per Entity

**Pros:**
- **Encapsulation**: Complex queries in one place
- **Testability**: Can mock `IVideoRepository`
- **Explicit**: Clear intent for each operation

**Cons:**
- **Duplication**: Each repository repeats CRUD operations
- **Maintenance**: Must update repository for new queries
- **Over-Engineering**: Simple CRUD doesn't need abstraction
- **Query Reuse**: Could use extension methods instead

**Why rejected:** For a simple CRUD API, repositories add complexity without benefit. If queries become complex, extract to query services.

### Alternative 3: CQRS with MediatR

**Pros:**
- **Separation**: Commands and Queries separated
- **Organization**: Each operation in its own class
- **Testable**: Easy to test individual handlers
- **Scalable**: Good for complex domains

**Cons:**
- **Overkill**: Way too complex for simple CRUD
- **Boilerplate**: Massive code overhead (Command, Handler, Validator, etc.)
- **Indirection**: Hard to follow flow from API to database
- **Learning Curve**: Team must learn CQRS + MediatR

**Why rejected:** CQRS is for complex business domains. Video CRUD is not complex enough to justify CQRS.

## Related Decisions

- [ADR-0003](0003-entity-framework-core-data-access.md): EF Core chosen as ORM
- [ADR-0009](0009-minimal-apis-over-controllers.md): Minimal APIs pair well with direct DbContext

## Related Links

- [Repository Pattern Debate](https://www.ben-morris.com/why-the-repository-pattern-is-meaningless-with-entity-framework/)
- [EF Core Testing](https://learn.microsoft.com/ef/core/testing/)
- [MSDN Repository Pattern](https://learn.microsoft.com/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)
- [Clean Architecture EF Core](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

## Notes

- **When Repository Makes Sense**:
  - Switching between multiple data sources (SQL + MongoDB)
  - Complex domain logic requiring encapsulation
  - Team mandate (company standards)
  - Large team with junior developers (guard rails)

- **Alternatives to Repository**:
  - **Extension Methods**: For query reuse
    ```csharp
    public static class VideoQueryExtensions
    {
        public static IQueryable<Video> Active(this IQueryable<Video> query)
            => query.Where(v => v.Status == VideoStatus.Ready);
    }

    // Usage: db.Videos.Active().ToListAsync();
    ```

  - **Query Services**: For complex queries
    ```csharp
    public class VideoQueryService
    {
        private readonly VideoDbContext _db;
        public Task<List<Video>> GetPopularVideosAsync(int count)
            => _db.Videos.OrderByDescending(v => v.ViewCount)
                         .Take(count).ToListAsync();
    }
    ```

  - **Specification Pattern**: For reusable filters
    ```csharp
    public class ActiveVideoSpec : ISpecification<Video>
    {
        public Expression<Func<Video, bool>> Criteria
            => v => v.Status == VideoStatus.Ready;
    }
    ```

- **Testing Strategy**:
  ```csharp
  // Integration test with EF Core In-Memory
  var options = new DbContextOptionsBuilder<VideoDbContext>()
      .UseInMemoryDatabase("TestDb")
      .Options;

  using var context = new VideoDbContext(options);
  // Seed data, test endpoint
  ```

- **Migration Path**: If you later need Repository:
  1. Keep existing code (it works!)
  2. Add repositories only for complex entities
  3. Don't refactor working code unnecessarily

- **Industry Trends**:
  - Microsoft's official samples use DbContext directly
  - Clean Architecture samples (Jason Taylor) use thin repositories with MediatR
  - Most modern .NET developers prefer direct DbContext
  - Repository pattern is legacy from pre-EF days

- **Performance**: Direct DbContext has zero overhead vs Repository (no extra method calls)

- **Conclusion**: Repository Pattern is not evil, it's just unnecessary when using EF Core. Keep things simple until complexity demands abstraction.

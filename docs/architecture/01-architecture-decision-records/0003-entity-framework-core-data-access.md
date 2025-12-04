# ADR-0003: Entity Framework Core for Data Access

**Status:** Accepted
**Date:** 2025-12-02
**Deciders:** Architecture Team
**Technical Story:** Database abstraction and ORM selection

## Context

The VideoService API requires persistent data storage for video metadata with the following needs:
- CRUD operations for video entities
- Strong typing and compile-time safety
- Database migrations management
- Query optimization capabilities
- Cross-database compatibility (PostgreSQL, SQL Server, SQLite)
- Integration with dependency injection
- Support for async/await patterns

We need to choose between:
- Full ORM (Entity Framework Core, NHibernate)
- Micro-ORM (Dapper, RepoDB)
- Raw ADO.NET
- Custom data access layer

## Decision

We will use **Entity Framework Core 8.0** as the data access technology for the AspireVideoService.

Specific configuration:
- Code-First approach with fluent API configuration
- DbContext lifetime: Scoped (per-request)
- Migrations: Automatic via `EnsureCreatedAsync()` for samples
- Connection management: Aspire-provided connection strings
- Query tracking: As-needed basis

## Consequences

### Positive

- **Productivity**: 80% less code compared to ADO.NET or Dapper
- **Type Safety**: Compile-time checking of queries via LINQ
- **Migrations**: Built-in schema versioning and evolution
- **Change Tracking**: Automatic detection of entity modifications
- **Relationship Management**: Automatic foreign key handling and navigation properties
- **Query Optimization**: Query compilation caching and split query support
- **Aspire Integration**: First-class support via `AddNpgsqlDbContext<T>()`
- **Testability**: Easy to mock DbContext for unit tests
- **Community**: Extensive documentation and community support
- **Cross-Database**: Switch databases with minimal code changes

### Negative

- **Performance Overhead**: 10-30% slower than Dapper for simple queries
- **Memory Usage**: Higher memory footprint due to change tracking
- **Learning Curve**: Requires understanding of EF Core internals for optimization
- **Over-fetching Risk**: Easy to accidentally load entire object graphs
- **N+1 Query Problem**: Can occur if relationships not properly configured
- **Abstraction Leaks**: Some SQL-specific features require raw SQL
- **Package Size**: Larger dependency footprint than micro-ORMs

### Neutral

- **Query Translation**: Some LINQ queries may not translate efficiently
- **Updates Required**: Must keep EF Core packages updated with .NET
- **Debugging**: SQL queries can be opaque (mitigated with logging)

## Alternatives Considered

### Alternative 1: Dapper (Micro-ORM)

**Pros:**
- **Performance**: 2-3x faster than EF Core for reads
- **Simplicity**: Thin wrapper over ADO.NET
- **Control**: Write exact SQL queries
- **Lightweight**: Minimal dependencies
- **Mature**: 10+ years of production use

**Cons:**
- **Manual Mapping**: Must write all SQL and mapping code
- **No Migrations**: Schema management requires external tools
- **No Change Tracking**: Must manually track modifications
- **Boilerplate**: Significantly more code for CRUD operations
- **Type Safety**: Weaker than EF Core (magic strings for SQL)

**Why rejected:** Too much boilerplate for an educational sample. EF Core provides better developer experience and demonstrates modern .NET practices.

### Alternative 2: Raw ADO.NET

**Pros:**
- **Maximum Performance**: Fastest possible data access
- **No Dependencies**: Ships with .NET runtime
- **Full Control**: Complete control over SQL and connections

**Cons:**
- **Extreme Boilerplate**: 10x more code than EF Core
- **Error-Prone**: Easy to introduce SQL injection, connection leaks
- **No Compile-Time Safety**: All SQL is magic strings
- **Manual Everything**: Mapping, transactions, connection management
- **Not Educational**: Doesn't demonstrate modern patterns

**Why rejected:** Unacceptably high maintenance burden. No compelling reason to use raw ADO.NET in modern applications unless performance is critical (>100k requests/second).

### Alternative 3: NHibernate

**Pros:**
- **Mature**: 15+ years of production use
- **Feature-Rich**: More features than EF Core (multi-tenancy, etc.)
- **XML/Code Configuration**: Flexible mapping options

**Cons:**
- **Legacy Feel**: API design shows its age
- **XML Configuration**: Most configurations use XML (verbose)
- **Less .NET-Native**: Ported from Java (Hibernate)
- **Smaller Community**: Declining community compared to EF Core
- **No Aspire Integration**: No built-in Aspire support

**Why rejected:** EF Core is the de facto standard in .NET ecosystem. NHibernate offers no significant advantage for this use case.

### Alternative 4: MongoDB C# Driver (Document DB)

**Pros:**
- **Schemaless**: No migrations required
- **JSON Native**: Natural fit for APIs
- **Horizontal Scaling**: Built for distributed systems

**Cons:**
- **Wrong Tool**: Relational data (videos with metadata) fits RDBMS better
- **No ACID Guarantees**: Eventual consistency may complicate logic
- **Learning Curve**: Different paradigm than relational
- **Aspire PostgreSQL**: Sample demonstrates PostgreSQL integration

**Why rejected:** Video metadata is inherently relational. PostgreSQL with EF Core is the right tool for this data model.

## Related Decisions

- [ADR-0004](0004-postgresql-primary-database.md): PostgreSQL chosen as database
- [ADR-0010](0010-direct-dbcontext-usage.md): No repository pattern over EF Core
- [ADR-0002](0002-using-dotnet-aspire.md): Aspire provides EF Core integration

## Related Links

- [EF Core 8 Documentation](https://learn.microsoft.com/ef/core/)
- [EF Core Performance](https://learn.microsoft.com/ef/core/performance/)
- [EF Core vs Dapper Benchmarks](https://github.com/DapperLib/Dapper#performance)
- [Aspire EF Core Integration](https://learn.microsoft.com/dotnet/aspire/database/postgresql-entity-framework-component)

## Notes

- Use `.AsNoTracking()` for read-only queries to improve performance
- Enable query logging in development: `EnableSensitiveDataLogging()`
- Consider `AsSplitQuery()` for complex queries with includes
- Use compiled queries for hot paths (if performance becomes issue)
- Future: Evaluate EF Core 9's JSON columns and AOT support
- Migrations strategy: Use `dotnet ef migrations` in production (not `EnsureCreated`)

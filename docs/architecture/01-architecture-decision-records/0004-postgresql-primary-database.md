# ADR-0004: PostgreSQL as Primary Database

**Status:** Accepted
**Date:** 2025-12-02
**Deciders:** Architecture Team
**Technical Story:** RDBMS selection for video metadata storage

## Context

The VideoService requires a relational database for storing:
- Video metadata (title, description, URL, status)
- View counts and statistics
- Timestamps and audit information
- Potential future: user data, comments, ratings

Requirements:
- ACID transactions
- Strong consistency
- Relational integrity (foreign keys)
- JSON support (for flexible metadata)
- Open-source preferred
- Cloud-native deployment support
- Strong .NET ecosystem integration
- Production-grade reliability

## Decision

We will use **PostgreSQL 16** as the primary database for AspireVideoService.

Deployment:
- Development: Docker container via Aspire (`builder.AddPostgres()`)
- Production: Managed service (Azure PostgreSQL, AWS RDS, etc.)
- Connection: Npgsql driver with Entity Framework Core
- Management Tool: PgAdmin (included via Aspire)

## Consequences

### Positive

- **Open Source**: No licensing costs, full source code access
- **ACID Compliance**: Strong consistency guarantees for video state
- **JSON Support**: Native JSONB for flexible video metadata
- **Performance**: Excellent query optimization and indexing
- **Reliability**: Battle-tested in production at scale (Instagram, Reddit, Spotify)
- **Cloud-Native**: First-class support in all major clouds
- **Aspire Integration**: Built-in Aspire hosting (`AddPostgres()`, `AddPgAdmin()`)
- **Advanced Features**: Full-text search, GIS data, arrays, custom types
- **Community**: Large, active community and extensive documentation
- **.NET Ecosystem**: Excellent Npgsql driver and EF Core provider
- **Compliance**: Strong support for data compliance (GDPR, HIPAA)
- **Horizontal Scaling**: Read replicas and partitioning support

### Negative

- **Resource Usage**: Higher memory usage than MySQL (10-30% more)
- **Complexity**: More configuration options can be overwhelming
- **Windows Support**: Not as well-optimized on Windows as Linux
- **Learning Curve**: Advanced features require deeper database knowledge
- **Version Upgrades**: Major version upgrades require planning

### Neutral

- **SQL Dialect**: PostgreSQL-specific SQL may not translate to other databases
- **Tooling**: Different tools than SQL Server (PgAdmin vs SSMS)
- **Backup Strategy**: Requires understanding PostgreSQL-specific backup tools

## Alternatives Considered

### Alternative 1: SQL Server

**Pros:**
- **Microsoft Ecosystem**: Native integration with .NET and Azure
- **Tooling**: Best-in-class tooling (SSMS, Azure Data Studio)
- **Performance**: Excellent for Windows workloads
- **Enterprise Features**: Advanced analytics, reporting services
- **Familiar**: Most .NET developers know SQL Server

**Cons:**
- **Licensing**: Expensive for commercial use ($3,500-$14,000 per core)
- **Linux Support**: Second-class citizen on Linux
- **Container Size**: Larger Docker images (1.5GB+ vs 200MB for PostgreSQL)
- **Open Source**: Not fully open source
- **Cloud Lock-in**: Best experience on Azure only

**Why rejected:** Licensing costs make it unsuitable for open-source educational samples. PostgreSQL offers similar features at zero cost.

### Alternative 2: MySQL

**Pros:**
- **Popularity**: Most widely deployed open-source database
- **Resource Efficient**: Lower memory footprint than PostgreSQL
- **Simple**: Easier to get started for beginners
- **Ecosystem**: Huge ecosystem of tools and hosting providers

**Cons:**
- **ACID Compliance**: Weaker guarantees than PostgreSQL (depends on engine)
- **JSON Support**: Less mature JSONB support
- **Advanced Features**: Lacks PostgreSQL's advanced features
- **.NET Integration**: Weaker .NET ecosystem compared to PostgreSQL
- **Oracle Ownership**: Concerns about future direction under Oracle
- **No Aspire Integration**: No built-in Aspire hosting package

**Why rejected:** PostgreSQL offers superior features (JSON, arrays, full-text search) needed for modern cloud-native apps. Aspire's built-in PostgreSQL support is a significant advantage.

### Alternative 3: SQLite

**Pros:**
- **Zero Configuration**: No server process required
- **Embedded**: Single file database, easy to deploy
- **Lightweight**: Minimal resource usage
- **Portable**: Works everywhere .NET runs

**Cons:**
- **Concurrency**: Limited concurrent write support
- **Scalability**: Not suitable for production cloud apps
- **Features**: Lacks advanced features (JSON, arrays, etc.)
- **No Network Access**: Cannot be shared across services
- **Single Process**: All services must access same file

**Why rejected:** SQLite is excellent for local/mobile apps but unsuitable for distributed microservices. Cannot demonstrate cloud-native patterns.

### Alternative 4: Azure Cosmos DB (NoSQL)

**Pros:**
- **Globally Distributed**: Multi-region replication
- **Flexible Schema**: Schemaless JSON documents
- **Horizontal Scaling**: Automatic partitioning
- **Multi-Model**: Supports SQL, MongoDB, Cassandra APIs

**Cons:**
- **Cost**: Very expensive compared to PostgreSQL ($0.25/hour minimum)
- **Azure Lock-in**: Only available on Azure
- **Eventual Consistency**: Default consistency model complex
- **Learning Curve**: Different paradigm than relational
- **Overkill**: Video metadata doesn't require global distribution

**Why rejected:** Cost-prohibitive for educational samples. Video metadata is inherently relational and doesn't require NoSQL capabilities.

## Related Decisions

- [ADR-0003](0003-entity-framework-core-data-access.md): EF Core chosen for data access
- [ADR-0002](0002-using-dotnet-aspire.md): Aspire provides PostgreSQL integration
- [ADR-0020](0020-zero-configuration-infrastructure.md): Aspire simplifies PostgreSQL setup

## Related Links

- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Npgsql Documentation](https://www.npgsql.org/doc/)
- [EF Core PostgreSQL Provider](https://www.npgsql.org/efcore/)
- [Aspire PostgreSQL Component](https://learn.microsoft.com/dotnet/aspire/database/postgresql-component)
- [PostgreSQL vs MySQL Comparison](https://www.postgresql.org/about/featurematrix/)

## Notes

- Use JSONB (not JSON) for flexible metadata: better indexing and query performance
- Enable connection pooling via Npgsql (default with Aspire)
- Consider partitioning for video table if >10M rows expected
- Use `TEXT` column type for video descriptions (unlimited length)
- Index strategy: Composite index on (UploadedAt DESC, Status)
- Production: Use managed PostgreSQL (Azure PostgreSQL Flexible Server, AWS RDS)
- Backup strategy: Automated daily backups with point-in-time recovery
- Future: Evaluate PostgreSQL 17's performance improvements (expected 2025)

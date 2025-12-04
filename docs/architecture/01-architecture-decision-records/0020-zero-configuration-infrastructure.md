# ADR-0020: Zero-Configuration Infrastructure

**Status:** Accepted
**Date:** 2025-12-02
**Deciders:** Architecture Team
**Technical Story:** Developer productivity and onboarding

## Context

Traditional development requires:
1. Install PostgreSQL locally
2. Create database manually
3. Configure connection strings
4. Install Redis
5. Start services manually

This creates:
- 30+ minute onboarding time
- Environment inconsistencies
- Configuration errors
- Maintenance burden

## Decision

Adopt **zero-configuration infrastructure** via .NET Aspire.

Approach:
- No local installation of databases/caches
- No connection string configuration
- No manual database creation
- Single command startup

## Consequences

### Positive

- **5-Minute Onboarding**: New developers productive immediately
- **No Configuration**: Connection strings injected automatically
- **Consistent**: Everyone has identical environment
- **Clean**: No local installations
- **Maintenance-Free**: Containers managed automatically

### Negative

- **Docker Required**: Must have Docker Desktop
- **Black Box**: Less visibility into configuration (mitigated by Aspire Dashboard)

## Example

Before (traditional):
```bash
# Install PostgreSQL
brew install postgresql
# Start PostgreSQL
brew services start postgresql
# Create database
createdb videodb
# Configure connection string
export ConnectionStrings__Default="Host=localhost;Database=videodb;..."
# Install Redis
brew install redis
# Start Redis
brew services start redis
# Run app
dotnet run
```

After (Aspire):
```bash
# Run app (that's it!)
dotnet run --project VideoService.AppHost
```

## Related Decisions

- [ADR-0002](0002-using-dotnet-aspire.md): Aspire framework
- [ADR-0012](0012-container-first-development.md): Container-based infrastructure

## Notes

How it works:
1. AppHost defines: `builder.AddPostgres("postgres").AddDatabase("videodb")`
2. Aspire starts PostgreSQL container automatically
3. Aspire generates connection string
4. Aspire injects connection string to API: `builder.AddNpgsqlDbContext<VideoDbContext>("videodb")`
5. DbContext automatically configured

No configuration files, no connection strings, just code!

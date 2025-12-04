# 7. Choose PostgreSQL over SQL Server

**Status:** ✅ Accepted

**Date:** 2024-12-01

**Deciders:** Architecture Team, Database Team, DevOps

**Technical Story:** Implementation in `samples/05-RealWorld/MicroserviceTemplate` and `samples/06-CuttingEdge/AspireCloudStack`

---

## Context and Problem Statement

We need a relational database for our microservices that supports:
- ACID transactions
- Complex queries and joins
- JSON/JSONB storage for flexibility
- Full-text search
- Scalability (vertical and horizontal)
- Cross-platform deployment (local dev, Docker, cloud)

**Requirements:**
- Must work in local development (Docker)
- Must work in Azure/AWS/GCP
- Must support EF Core
- Must have good tooling and IDE support
- Licensing must be compatible with our business model

---

## Decision Drivers

* **Open Source** - No licensing costs, community-driven
* **Cross-Platform** - Works on Windows, Linux, macOS, containers
* **Cloud Native** - First-class support in all cloud providers
* **Advanced Features** - JSON, arrays, full-text search, extensions
* **Performance** - Competitive with commercial databases
* **Ecosystem** - Strong .NET integration via Npgsql

---

## Considered Options

* **Option 1** - PostgreSQL
* **Option 2** - Microsoft SQL Server
* **Option 3** - MySQL/MariaDB
* **Option 4** - SQLite

---

## Decision Outcome

**Chosen option:** "PostgreSQL", because it offers enterprise-grade features, excellent performance, full JSON support, extensibility, and zero licensing costs while working seamlessly across all environments (local, Docker, any cloud).

### Positive Consequences

* **Open Source** - No licensing fees, even in production
* **Advanced JSON Support** - JSONB type with indexing and querying
* **Extensions** - PostGIS (geospatial), pg_trgm (fuzzy search), TimescaleDB (time-series)
* **Standards Compliance** - Most SQL-standard compliant database
* **Cloud Support** - Azure PostgreSQL, AWS RDS/Aurora, GCP Cloud SQL
* **Docker-Friendly** - Official images, small footprint
* **Tooling** - pgAdmin, DBeaver, DataGrip, Azure Data Studio

### Negative Consequences

* **Less Windows Integration** - Not native to Windows like SQL Server
* **Learning Curve** - Team familiar with SQL Server needs training
* **Different Tooling** - Can't use SQL Server Management Studio
* **Case Sensitivity** - Identifiers are case-sensitive (can be configured)

---

## Pros and Cons of the Options

### PostgreSQL (Chosen)

**What is PostgreSQL?**

PostgreSQL is an open-source, object-relational database system with 35+ years of active development. Known for reliability, feature robustness, and standards compliance.

**Pros:**
* **Open Source** - MIT-like license, no licensing costs
* **JSON/JSONB** - First-class JSON support with indexing
* **Advanced Types** - Arrays, ranges, UUIDs, enums, custom types
* **Full-Text Search** - Built-in without needing external services
* **Extensions** - PostGIS, TimescaleDB, pg_cron, pg_partman
* **ACID Compliance** - Rock-solid transactions
* **Performance** - Excellent query optimizer, parallel queries
* **Standards** - Most SQL-standard compliant
* **Community** - Huge ecosystem, frequent updates

**Cons:**
* **Windows Support** - Less native than SQL Server on Windows
* **Tooling** - Fewer enterprise tools than SQL Server
* **Vacuum Process** - Requires VACUUM for space reclamation
* **Replication** - Slightly more complex setup than SQL Server

**Connection String:**
```csharp
"Host=localhost;Port=5432;Database=mydb;Username=postgres;Password=password"
```

**EF Core Configuration:**
```csharp
// Program.cs or Startup.cs
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        configuration.GetConnectionString("PostgreSQL"),
        npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
            npgsqlOptions.CommandTimeout(30);
            npgsqlOptions.MigrationsAssembly("Infrastructure");
        }));

// Install: Npgsql.EntityFrameworkCore.PostgreSQL
```

**Docker Compose:**
```yaml
version: '3.8'

services:
  postgres:
    image: postgres:16-alpine
    container_name: myapp-postgres
    environment:
      POSTGRES_DB: mydb
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: password
    ports:
      - "5432:5432"
    volumes:
      - postgres-data:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 10s
      timeout: 5s
      retries: 5

  pgadmin:
    image: dpage/pgadmin4:latest
    container_name: myapp-pgadmin
    environment:
      PGADMIN_DEFAULT_EMAIL: admin@example.com
      PGADMIN_DEFAULT_PASSWORD: admin
    ports:
      - "5050:80"
    depends_on:
      - postgres

volumes:
  postgres-data:
```

**.NET Aspire (Even Simpler):**
```csharp
// AppHost/Program.cs
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()          // Automatically adds pgAdmin
    .WithDataVolume();      // Persistent storage

var db = postgres.AddDatabase("mydb");

var api = builder.AddProject<Projects.ApiService>("api")
    .WithReference(db);     // Connection string injected automatically!

await builder.Build().RunAsync();

// No docker-compose needed!
// Navigate to: http://localhost:18888 for Aspire Dashboard
// pgAdmin available automatically
```

**Advanced PostgreSQL Features:**

**1. JSONB (Binary JSON):**
```csharp
// Entity
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }

    // Store arbitrary JSON data
    [Column(TypeName = "jsonb")]
    public JsonDocument Metadata { get; set; }
}

// Query JSONB
var productsWithTag = await _context.Products
    .Where(p => EF.Functions.JsonContains(
        p.Metadata.RootElement,
        JsonDocument.Parse(@"{""tags"":[""electronics""]}")))
    .ToListAsync();

// PostgreSQL generates:
// SELECT * FROM products
// WHERE metadata @> '{"tags":["electronics"]}'

// Create index on JSONB field
migrationBuilder.Sql(
    "CREATE INDEX idx_product_metadata_tags ON products USING GIN ((metadata->'tags'))");
```

**2. Array Types:**
```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }

    // Native array support
    public string[] Tags { get; set; }
    public int[] CategoryIds { get; set; }
}

// Query arrays
var products = await _context.Products
    .Where(p => p.Tags.Contains("electronics"))
    .ToListAsync();

// PostgreSQL:
// SELECT * FROM products WHERE 'electronics' = ANY(tags)
```

**3. Full-Text Search:**
```csharp
// Entity configuration
modelBuilder.Entity<Product>()
    .HasIndex(p => p.Name)
    .HasMethod("GIN")
    .IsTsVectorExpressionIndex("english");

// Search query
var results = await _context.Products
    .Where(p => EF.Functions.ToTsVector("english", p.Name)
        .Matches(EF.Functions.ToTsQuery("english", "laptop")))
    .ToListAsync();

// PostgreSQL:
// SELECT * FROM products
// WHERE to_tsvector('english', name) @@ to_tsquery('english', 'laptop')
```

**4. Generated Columns:**
```csharp
public class Product
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public decimal TaxRate { get; set; }

    // Computed column (stored in database)
    public decimal TotalPrice { get; set; }
}

// Migration
migrationBuilder.Sql(@"
    ALTER TABLE products
    ADD COLUMN total_price decimal
    GENERATED ALWAYS AS (price * (1 + tax_rate)) STORED
");
```

**5. Extensions - PostGIS (Geospatial):**
```csharp
// Install: Npgsql.EntityFrameworkCore.PostgreSQL.NetTopologySuite

public class Store
{
    public int Id { get; set; }
    public string Name { get; set; }

    [Column(TypeName = "geometry(Point)")]
    public Point Location { get; set; }
}

// Find stores within 5km radius
var nearbyStores = await _context.Stores
    .Where(s => s.Location.Distance(userLocation) < 5000)
    .OrderBy(s => s.Location.Distance(userLocation))
    .ToListAsync();

// PostgreSQL (PostGIS):
// SELECT * FROM stores
// WHERE ST_Distance(location, ST_SetSRID(ST_MakePoint(lon, lat), 4326)) < 5000
// ORDER BY ST_Distance(location, ST_SetSRID(ST_MakePoint(lon, lat), 4326))
```

### Microsoft SQL Server

**Pros:**
* **Windows Integration** - Native Windows authentication, SSMS
* **Tooling** - SQL Server Management Studio, Profiler
* **Team Familiarity** - Many .NET devs know SQL Server
* **Azure Integration** - Azure SQL Database is excellent
* **Temporal Tables** - Built-in time-travel queries

**Cons:**
* **Licensing Costs** - ~$14,000 per core for Enterprise, ~$1,400 for Standard
* **Windows-Centric** - Linux support is newer, less mature
* **Docker Images** - Larger images (~1.5GB vs 200MB for PostgreSQL)
* **Limited JSON Support** - JSON functions but no native JSONB type
* **Cloud Costs** - Azure SQL more expensive than PostgreSQL equivalents

**Connection String:**
```csharp
"Server=localhost;Database=mydb;User Id=sa;Password=Password123!;TrustServerCertificate=True"
```

**When SQL Server Makes Sense:**
- Heavily invested in Microsoft ecosystem
- Need SQL Server-specific features (columnstore, in-memory OLTP)
- Azure SQL Managed Instance required
- Team has deep SQL Server expertise and no time for training

**Why Not Chosen:**
Licensing costs (~$14k/core for Enterprise) and Docker image size (1.5GB) make SQL Server less suitable for cloud-native microservices. For educational repository, PostgreSQL's free and open nature is preferable.

### MySQL/MariaDB

**Pros:**
* Open source (MySQL has dual license, MariaDB is GPL)
* Very fast for read-heavy workloads
* Wide adoption (WordPress, Drupal)
* Good replication support

**Cons:**
* **Less Feature-Rich** - No native JSON indexing (MySQL 5.7), limited window functions
* **Oracle Ownership** - MySQL owned by Oracle (licensing concerns)
* **Less Standards-Compliant** - Deviates from SQL standard in places
* **Weaker Transactions** - Historically weaker ACID guarantees
* **Limited Extensions** - No equivalent to PostgreSQL's extension ecosystem

**Why Not Chosen:**
PostgreSQL offers superior feature set (JSONB, arrays, extensions, standards compliance) while also being open source. MySQL's advantages (speed in simple read-heavy workloads) don't outweigh PostgreSQL's flexibility for complex applications.

### SQLite

**Pros:**
* Embedded - no server process needed
* Single file database
* Perfect for development/testing
* Extremely fast for small datasets

**Cons:**
* **Not Client-Server** - Can't scale horizontally
* **Limited Concurrency** - Writers block readers
* **No User Management** - No built-in authentication
* **Limited ALTER TABLE** - Can't modify columns easily
* **Not for Production** - Not suitable for multi-user web apps

**When SQLite Makes Sense:**
- Local development/testing
- Mobile apps
- Desktop applications
- Embedded devices

**Why Not Chosen for Production:**
SQLite is excellent for testing and local dev, but our microservices need client-server databases with proper concurrency and scalability. We **do use SQLite** in unit tests via `UseInMemoryDatabase()`.

---

## Feature Comparison Table

| Feature | PostgreSQL | SQL Server | MySQL | SQLite |
|---------|-----------|------------|-------|--------|
| **License** | Open Source (PostgreSQL License) | Proprietary (~$14k/core) | Open (GPL) / Proprietary | Public Domain |
| **JSON Support** | ✅ JSONB (indexed) | ⚠️ JSON (no index) | ⚠️ JSON (basic) | ❌ |
| **Arrays** | ✅ Native | ❌ | ❌ | ❌ |
| **Full-Text Search** | ✅ Built-in | ✅ Built-in | ✅ Built-in | ❌ |
| **Extensions** | ✅ PostGIS, TimescaleDB, etc. | ⚠️ Limited | ❌ | ❌ |
| **Window Functions** | ✅ Excellent | ✅ Excellent | ⚠️ Basic | ✅ |
| **CTEs (WITH)** | ✅ | ✅ | ✅ (8.0+) | ✅ |
| **Geospatial** | ✅ PostGIS | ✅ Built-in | ⚠️ Limited | ❌ |
| **Horizontal Scaling** | ✅ Citus, partitioning | ⚠️ Complex | ✅ Replication | ❌ |
| **Docker Image Size** | 200MB (Alpine) | 1.5GB | 500MB | N/A |
| **Cloud Support** | ✅ All clouds | ✅ Azure best | ✅ All clouds | ❌ |

---

## Migration from SQL Server to PostgreSQL

If migrating existing SQL Server databases:

**1. Schema Migration:**
```bash
# Use pgloader for automated migration
pgloader mssql://user:pass@sqlserver:1433/MyDB postgresql://user:pass@postgres:5432/mydb

# Or manual migration
# Export SQL Server schema → Convert to PostgreSQL DDL
```

**2. Code Changes:**
```csharp
// SQL Server specific
builder.HasDefaultValueSql("NEWID()");        // GUID generation
builder.HasDefaultValueSql("GETUTCDATE()");  // Current timestamp

// PostgreSQL equivalent
builder.HasDefaultValueSql("gen_random_uuid()");  // GUID
builder.HasDefaultValueSql("NOW()");              // Timestamp
```

**3. Case Sensitivity:**
```sql
-- SQL Server (case-insensitive)
SELECT * FROM Products WHERE Name = 'widget'  -- Matches 'Widget', 'WIDGET'

-- PostgreSQL (case-sensitive by default)
SELECT * FROM products WHERE name = 'widget'  -- Only matches 'widget'
SELECT * FROM products WHERE name ILIKE 'widget'  -- Case-insensitive
```

---

## Local Development Setup

**Option 1: Docker Compose**
```bash
docker-compose up -d
# PostgreSQL: localhost:5432
# pgAdmin: http://localhost:5050
```

**Option 2: .NET Aspire (Recommended)**
```bash
cd AppHost
dotnet run

# Aspire Dashboard: http://localhost:18888
# PostgreSQL and pgAdmin auto-configured
```

**Option 3: Local Install**
```bash
# macOS
brew install postgresql@16
brew services start postgresql@16

# Ubuntu
sudo apt install postgresql-16
sudo systemctl start postgresql

# Windows
# Download installer from postgresql.org
```

---

## Production Deployment Options

**1. Azure Database for PostgreSQL:**
```bash
az postgres flexible-server create \
  --name myapp-postgres \
  --resource-group myapp-rg \
  --location eastus \
  --admin-user myadmin \
  --admin-password MyPassword123! \
  --sku-name Standard_D2s_v3 \
  --tier GeneralPurpose \
  --storage-size 128
```

**2. AWS RDS PostgreSQL:**
```bash
aws rds create-db-instance \
  --db-instance-identifier myapp-postgres \
  --db-instance-class db.t3.medium \
  --engine postgres \
  --engine-version 16.1 \
  --master-username postgres \
  --master-user-password MyPassword123! \
  --allocated-storage 100
```

**3. Kubernetes with Helm:**
```bash
helm install postgres bitnami/postgresql \
  --set auth.postgresPassword=password \
  --set primary.persistence.size=10Gi
```

---

## Links

* [PostgreSQL Official Documentation](https://www.postgresql.org/docs/)
* [Npgsql - .NET Data Provider](https://www.npgsql.org/)
* [EF Core PostgreSQL Provider](https://www.npgsql.org/efcore/)
* [pgAdmin](https://www.pgadmin.org/)
* [PostGIS](https://postgis.net/)
* [Sample Implementation](../../samples/05-RealWorld/MicroserviceTemplate)

---

## Notes

**Best Practices:**
- **Connection Pooling** - Always use connection pooling (enabled by default in Npgsql)
- **Indexes** - Create indexes on foreign keys and frequently queried columns
- **VACUUM** - Schedule regular VACUUM ANALYZE for statistics
- **Prepared Statements** - EF Core uses them automatically via Npgsql
- **Partitioning** - Use table partitioning for very large tables (100M+ rows)

**Common Pitfalls:**
- ❌ Not creating indexes on foreign keys (PostgreSQL doesn't auto-create them)
- ❌ Using `SELECT *` in production (explicitly list columns)
- ❌ Not using connection pooling
- ❌ Forgetting to analyze tables after bulk inserts

**Monitoring:**
```sql
-- Check database size
SELECT pg_size_pretty(pg_database_size('mydb'));

-- Check slow queries
SELECT query, mean_exec_time, calls
FROM pg_stat_statements
ORDER BY mean_exec_time DESC
LIMIT 10;

-- Check table bloat
SELECT schemaname, tablename, pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename))
FROM pg_tables
ORDER BY pg_total_relation_size(schemaname||'.'||tablename) DESC;
```

**Review Date:** 2025-12-01

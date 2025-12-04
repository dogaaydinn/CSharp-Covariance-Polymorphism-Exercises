# 14. Containerize with Docker

**Status:** ✅ Accepted

**Date:** 2024-12-01

**Deciders:** Architecture Team, DevOps Team

**Technical Story:** Implementation in `Dockerfile` and `docker-compose.yml`

---

## Context and Problem Statement

Modern applications need to run consistently across:
- Developer laptops (Windows, macOS, Linux)
- CI/CD pipelines
- Test environments
- Production (cloud or on-premises)

**Traditional deployment problems:**
- "Works on my machine" syndrome
- Dependency conflicts
- Manual server setup
- Environment-specific configurations
- Difficult rollbacks

**Requirements:**
- Consistent environments (dev = prod)
- Isolation from host system
- Easy local development
- CI/CD compatible
- Orchestration-ready (Kubernetes)

---

## Decision Drivers

* **Consistency** - Same environment everywhere
* **Isolation** - No conflicts with host system
* **Portability** - Run anywhere containers are supported
* **Industry Standard** - Docker is ubiquitous
* **DevOps Integration** - Works with all CI/CD tools
* **Kubernetes Ready** - Path to orchestration

---

## Considered Options

* **Option 1** - Docker + Docker Compose
* **Option 2** - Virtual Machines (VMs)
* **Option 3** - Bare metal deployment
* **Option 4** - .NET Self-Contained Deployment

---

## Decision Outcome

**Chosen option:** "Docker + Docker Compose", because containers provide lightweight, consistent environments that work identically in development and production, with industry-standard tooling and orchestration support.

### Positive Consequences

* **Consistent Environments** - "Works on my machine" = "Works in production"
* **Lightweight** - Seconds to start vs minutes for VMs
* **Isolation** - Each service in its own container
* **Version Control** - Dockerfile is code
* **Easy Rollback** - Just deploy previous image
* **Ecosystem** - Docker Hub, registries, orchestrators
* **Local Development** - docker-compose up for full stack

### Negative Consequences

* **Learning Curve** - Team needs Docker knowledge
* **Windows Containers** - Less mature than Linux containers
* **Networking Complexity** - Container networking can be tricky
* **Storage** - Container images consume disk space

---

## Pros and Cons of the Options

### Docker + Docker Compose (Chosen)

**What is Docker?**

Docker is a platform for developing, shipping, and running applications in containers—lightweight, standalone, executable packages that include everything needed to run software.

**Pros:**
* **Lightweight** - Share OS kernel, MB not GB
* **Fast** - Start in seconds
* **Portable** - Run anywhere (dev, cloud, on-prem)
* **Isolation** - Process, network, filesystem isolation
* **Immutable** - Rebuild images, don't modify containers
* **Docker Compose** - Multi-container orchestration
* **Industry standard** - Universal adoption

**Cons:**
* **Linux-centric** - Windows containers less common
* **Networking** - Can be complex
* **Persistent data** - Requires volume management
* **Security** - Shared kernel has risks

**Multi-Stage Dockerfile (.NET 8):**
```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies (cached layer)
COPY ["src/API/API.csproj", "src/API/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "src/API/API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/API"
RUN dotnet build "API.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Copy published app
COPY --from=publish /app/publish .

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl --fail http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "API.dll"]
```

**Why Multi-Stage?**
- **Stage 1 (build)**: Uses SDK image (large, ~1GB) for compilation
- **Stage 2 (publish)**: Publishes optimized build
- **Stage 3 (final)**: Uses runtime image (small, ~200MB) - only includes runtime, not SDK
- **Result**: Final image is 5x smaller!

**Docker Compose (Local Development):**
```yaml
version: '3.8'

services:
  api:
    build:
      context: .
      dockerfile: Dockerfile
    container_name: myapp-api
    ports:
      - "5000:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__PostgreSQL=Host=postgres;Database=mydb;Username=postgres;Password=password
      - ConnectionStrings__Redis=redis:6379
    depends_on:
      postgres:
        condition: service_healthy
      redis:
        condition: service_started
    networks:
      - myapp-network
    volumes:
      - ./logs:/app/logs  # Persist logs

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
    networks:
      - myapp-network

  redis:
    image: redis:7-alpine
    container_name: myapp-redis
    ports:
      - "6379:6379"
    volumes:
      - redis-data:/data
    command: redis-server --appendonly yes
    networks:
      - myapp-network

  pgadmin:
    image: dpage/pgadmin4
    container_name: myapp-pgadmin
    environment:
      PGADMIN_DEFAULT_EMAIL: admin@example.com
      PGADMIN_DEFAULT_PASSWORD: admin
    ports:
      - "5050:80"
    depends_on:
      - postgres
    networks:
      - myapp-network

networks:
  myapp-network:
    driver: bridge

volumes:
  postgres-data:
  redis-data:
```

**Usage:**
```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f api

# Stop all services
docker-compose down

# Rebuild and restart
docker-compose up -d --build

# Remove volumes (DANGER: deletes data)
docker-compose down -v
```

**.dockerignore:**
```
# Ignore build artifacts
**/bin/
**/obj/
**/out/

# Ignore dependencies
**/node_modules/

# Ignore git
.git/
.gitignore

# Ignore IDE files
.vscode/
.vs/
.idea/

# Ignore test results
**/TestResults/
**/coverage/

# Ignore documentation
**/*.md
docs/

# Ignore secrets
**/*.pfx
**/*.key
**/*.Development.json
```

**Production Optimizations:**

**1. Layer Caching:**
```dockerfile
# ✅ GOOD: Restore dependencies first (cached if csproj unchanged)
COPY ["*.csproj", "./"]
RUN dotnet restore

# Then copy source code
COPY . .
RUN dotnet build
```

**2. Use Alpine Images (Smaller):**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
# Alpine images are 50% smaller than Debian-based
```

**3. Security Scanning:**
```bash
# Scan image for vulnerabilities
docker scan myapp:latest

# Or use Trivy
trivy image myapp:latest
```

**4. Read-Only Root Filesystem:**
```dockerfile
# Add to docker-compose.yml
services:
  api:
    read_only: true
    tmpfs:
      - /tmp
      - /app/logs
```

### Virtual Machines

**Pros:**
* Full OS isolation
* Run any OS
* Mature technology

**Cons:**
* **Heavy** - GB of disk, GB of RAM
* **Slow** - Minutes to boot
* **Resource intensive** - Full OS overhead
* **Less portable** - Image formats vary (VHD, VMDK, OVA)

**Why Rejected:**
VMs are too heavyweight for microservices. Containers provide isolation with 10x less overhead.

### Bare Metal Deployment

**Pros:**
* Maximum performance
* No overhead
* Direct hardware access

**Cons:**
* **No isolation** - Dependency conflicts
* **Environment drift** - Dev ≠ Prod
* **Manual setup** - No automation
* **Difficult rollback** - No versioning
* **Scaling complexity** - Manual provisioning

**Why Rejected:**
"Works on my machine" problem. Containers eliminate environment inconsistencies.

### .NET Self-Contained Deployment

**Pros:**
* Single executable
* No .NET runtime required on server
* AOT compilation available

**Cons:**
* **Large executables** - 50-150MB
* **No database/Redis** - Just the app
* **Still need environment** - OS dependencies
* **No orchestration** - Can't use Kubernetes

**When to Use:**
- Desktop applications
- Serverless functions
- Edge devices

**Why Not Primary Choice:**
Self-contained deployment solves .NET runtime dependency but doesn't solve database, caching, or environment management. Containers provide complete solution.

---

## Container Registry

**Docker Hub (Public):**
```bash
# Tag image
docker tag myapp:latest username/myapp:1.0.0

# Push to Docker Hub
docker push username/myapp:1.0.0
```

**Azure Container Registry:**
```bash
# Login
az acr login --name myregistry

# Tag image
docker tag myapp:latest myregistry.azurecr.io/myapp:1.0.0

# Push
docker push myregistry.azurecr.io/myapp:1.0.0
```

**GitHub Container Registry:**
```bash
# Login
echo $GITHUB_TOKEN | docker login ghcr.io -u USERNAME --password-stdin

# Tag
docker tag myapp:latest ghcr.io/username/myapp:1.0.0

# Push
docker push ghcr.io/username/myapp:1.0.0
```

---

## CI/CD Integration

**GitHub Actions:**
```yaml
name: Docker Build and Push

on:
  push:
    branches: [main]

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v3

      - name: Set up Docker Buildx
        uses: docker/setup-buildx-action@v2

      - name: Login to GitHub Container Registry
        uses: docker/login-action@v2
        with:
          registry: ghcr.io
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}

      - name: Build and push
        uses: docker/build-push-action@v4
        with:
          context: .
          push: true
          tags: |
            ghcr.io/${{ github.repository }}:latest
            ghcr.io/${{ github.repository }}:${{ github.sha }}
          cache-from: type=gha
          cache-to: type=gha,mode=max
```

---

## Monitoring Container Health

**Docker Health Check:**
```dockerfile
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl --fail http://localhost:8080/health || exit 1
```

**ASP.NET Health Checks:**
```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("PostgreSQL")!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

var app = builder.Build();

app.MapHealthChecks("/health");  // Docker will call this endpoint
app.Run();
```

---

## Best Practices

**1. Don't Run as Root:**
```dockerfile
RUN adduser --disabled-password --gecos '' appuser
USER appuser
```

**2. Use .dockerignore:**
```
# Ignore unnecessary files for faster build
**/bin/
**/obj/
.git/
```

**3. Pin Image Versions:**
```dockerfile
# ❌ BAD: latest tag (unpredictable)
FROM mcr.microsoft.com/dotnet/aspnet:latest

# ✅ GOOD: Specific version
FROM mcr.microsoft.com/dotnet/aspnet:8.0
```

**4. Scan for Vulnerabilities:**
```bash
docker scan myapp:latest
```

**5. Use Multi-Stage Builds:**
```dockerfile
# Keeps final image small (200MB vs 1GB)
FROM sdk AS build
FROM runtime AS final
```

---

## Links

* [Docker Documentation](https://docs.docker.com/)
* [Docker Compose](https://docs.docker.com/compose/)
* [.NET Docker Images](https://hub.docker.com/_/microsoft-dotnet)
* [Sample Dockerfile](../../Dockerfile)

---

## Notes

**Container vs VM:**
- Container: Process isolation, shares kernel, MB of disk, seconds to start
- VM: Full OS, separate kernel, GB of disk, minutes to start

**When to Use Docker:**
- ✅ Microservices
- ✅ CI/CD pipelines
- ✅ Local development
- ✅ Cloud deployment

**When NOT to Use:**
- ❌ GUI applications (limited support)
- ❌ Hardware-specific apps
- ❌ When kernel access needed

**Review Date:** 2025-12-01

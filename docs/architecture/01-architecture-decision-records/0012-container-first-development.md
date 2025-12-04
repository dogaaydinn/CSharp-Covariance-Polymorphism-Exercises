# ADR-0012: Container-First Development

**Status:** Accepted
**Date:** 2025-12-02
**Deciders:** Architecture Team
**Technical Story:** Development environment and deployment strategy

## Context

Modern cloud-native applications deploy as containers. Development environments should match production to avoid "works on my machine" issues. We need to decide:
- How to run infrastructure (Redis, PostgreSQL) locally
- Whether to containerize application services in development
- How to ensure dev/prod parity

Traditional approach: Install services locally (PostgreSQL, Redis via brew/apt)
Modern approach: Everything runs in containers

## Decision

We will adopt a **container-first development** approach using Docker containers for all infrastructure and optional containerization for application services.

Implementation:
- Infrastructure (Redis, PostgreSQL): Always containers via Aspire
- Application services: Run natively during development, containerize for deployment
- Docker Desktop required for development
- No local installation of databases/caches required

## Consequences

### Positive

- **Dev/Prod Parity**: Same PostgreSQL/Redis versions in dev and prod
- **Zero Local Installation**: No need to install PostgreSQL, Redis locally
- **Isolation**: No port conflicts with other projects
- **Clean Machine**: Development machine stays clean
- **Version Control**: Infrastructure versions defined in code (Aspire)
- **Onboarding**: New developers up and running in minutes
- **Multiple Projects**: Run multiple projects simultaneously (different ports)
- **Consistency**: Everyone has identical environment
- **Easy Cleanup**: `docker system prune` removes everything

### Negative

- **Docker Requirement**: Must install and run Docker Desktop
- **Resource Usage**: Docker containers use memory (500MB-1GB for PostgreSQL)
- **Performance**: Slightly slower than native (5-10% overhead)
- **Windows/Mac**: Docker on Windows/Mac uses VM (additional overhead)
- **Learning Curve**: Developers must understand container basics
- **Startup Time**: Containers take 5-10 seconds to start

### Neutral

- **Disk Space**: Containers use disk space (5-10GB for common images)
- **Networking**: Container networking sometimes confusing for beginners

## Alternatives Considered

### Alternative 1: Local Installation (PostgreSQL, Redis)

**Pros:**
- **Native Performance**: No container overhead
- **No Docker**: Don't need Docker Desktop
- **Familiar**: Traditional development approach
- **Instant Startup**: Services always running

**Cons:**
- **Environment Drift**: Different versions across developers
- **Port Conflicts**: Redis on 6379 conflicts with other projects
- **Machine Pollution**: Must install/maintain services locally
- **OS-Specific**: Different setup on Windows vs Mac vs Linux
- **Hard to Clean**: Services linger after project ends
- **Version Management**: Hard to switch PostgreSQL versions

**Why rejected:** "Works on my machine" is unacceptable in modern development. Containers eliminate environment issues.

### Alternative 2: Full Application Containerization

**Pros:**
- **Complete Parity**: Entire app runs in containers
- **Production-Like**: Exact production configuration
- **Reproducible**: Dockerfile defines exact environment

**Cons:**
- **Slow Iteration**: Must rebuild container on every code change
- **Debugging**: Harder to attach debugger to container
- **Hot Reload**: Doesn't work in containers (or requires volume mounts)
- **Resource Intensive**: Running 5+ containers locally is heavy
- **Complexity**: Debugging networking issues between containers

**Why rejected:** Too slow for development. Hybrid approach (native apps, containerized infrastructure) provides best developer experience.

### Alternative 3: Docker Compose Only (No Aspire)

**Pros:**
- **Simple**: Just docker-compose.yml
- **Standard**: Widely understood
- **Portable**: Works without Aspire

**Cons:**
- **Manual**: Must write docker-compose.yml manually
- **No Observability**: No built-in dashboard
- **No Service Discovery**: Must manage ports manually
- **No Resilience**: No retry/circuit breaker
- **Static Configuration**: Can't easily change config

**Why rejected:** Aspire provides Docker Compose functionality + service discovery + observability + resilience.

### Alternative 4: Kubernetes (Minikube/Kind)

**Pros:**
- **Production-Like**: Exact production environment
- **Complete**: Full Kubernetes features

**Cons:**
- **Extremely Slow**: Startup time 1-2 minutes
- **Resource Intensive**: Kubernetes cluster uses 2-4GB RAM
- **Complex**: Requires deep Kubernetes knowledge
- **Overkill**: Too much for local development
- **Poor DX**: Terrible developer experience

**Why rejected:** Kubernetes is for production, not development. Aspire provides similar patterns without Kubernetes complexity.

### Alternative 5: Cloud Development (GitHub Codespaces, Gitpod)

**Pros:**
- **Zero Local Setup**: Everything in cloud
- **Consistent**: Identical environment for everyone
- **Powerful**: Can use any machine (even Chromebook)

**Cons:**
- **Cost**: $0.18/hour (Codespaces) or $0.36/hour (Gitpod)
- **Latency**: Network latency affects responsiveness
- **Offline**: Cannot work without internet
- **Vendor Lock-in**: Tied to specific cloud provider

**Why rejected:** Not everyone has budget or good internet. Local development should be primary approach.

## Related Decisions

- [ADR-0002](0002-using-dotnet-aspire.md): Aspire manages containers
- [ADR-0004](0004-postgresql-primary-database.md): PostgreSQL via container
- [ADR-0005](0005-redis-distributed-caching.md): Redis via container

## Related Links

- [The Twelve-Factor App](https://12factor.net/dev-prod-parity)
- [Docker Documentation](https://docs.docker.com/)
- [Aspire Container Hosting](https://learn.microsoft.com/dotnet/aspire/fundamentals/persist-data-volumes)

## Notes

- **Docker Desktop Requirements**:
  - Windows: Docker Desktop for Windows (WSL2 required)
  - Mac: Docker Desktop for Mac (Apple Silicon or Intel)
  - Linux: Docker Engine (native, best performance)

- **Container Images Used**:
  - PostgreSQL: `postgres:16-alpine` (~200MB)
  - Redis: `redis:7-alpine` (~50MB)
  - Management: PgAdmin (~150MB), Redis Commander (~100MB)

- **Resource Allocation**:
  - Minimum: 8GB RAM, 20GB disk space
  - Recommended: 16GB RAM, 50GB disk space
  - Docker Desktop settings: Allocate 4GB RAM minimum

- **Performance Optimization**:
  - Use named volumes for database persistence
  - Enable Docker BuildKit for faster builds
  - Prune unused images regularly: `docker system prune`

- **Development Workflow**:
  1. Start Aspire AppHost: `dotnet run --project VideoService.AppHost`
  2. Aspire starts Redis/PostgreSQL containers automatically
  3. Application services run natively (.NET process)
  4. Stop AppHost: Containers stop automatically

- **Production Deployment**:
  - Build container images: `dotnet publish -p:PublishProfile=DefaultContainer`
  - Push to registry: `docker push myregistry/videoservice-api:1.0`
  - Deploy to Kubernetes/Azure Container Apps/AWS ECS

- **Troubleshooting**:
  - Port conflicts: `docker ps` to see running containers
  - Slow performance: Increase Docker Desktop memory allocation
  - Disk space: `docker system df` and `docker system prune -a`

- **Future**: .NET 9 improves container building with Native AOT support (smaller, faster images)

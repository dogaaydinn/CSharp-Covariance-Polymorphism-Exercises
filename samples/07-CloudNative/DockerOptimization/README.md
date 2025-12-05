# Docker Optimization

> Demonstrate Docker best practices for minimal, efficient container images.

## Features
- **Multi-Stage Builds** - Separate build and runtime environments
- **Alpine Images** - Minimal Linux distribution (5MB base)
- **Layer Caching** - Optimize build times
- **Trimmed Publishing** - Remove unused assemblies (40% smaller)
- **Chiseled Images** - Ultra-minimal Ubuntu-based images

## Image Size Comparison

| Dockerfile | Base Image | Final Size | Reduction |
|------------|-----------|------------|-----------|
| Dockerfile.standard | SDK (full) | ~220 MB | Baseline |
| Dockerfile.optimized | ASP.NET Chiseled | ~110 MB | 50% smaller |
| Dockerfile.alpine | Alpine | ~50 MB | 77% smaller |

## Build and Compare

### Standard Build
```bash
cd samples/07-CloudNative/DockerOptimization

# Standard (NOT optimized)
docker build -f Dockerfile.standard -t docker-demo:standard .
docker images docker-demo:standard
```

### Optimized Build
```bash
# Optimized with multi-stage + chiseled
docker build -f Dockerfile.optimized -t docker-demo:optimized .
docker images docker-demo:optimized
```

### Alpine Build (Smallest)
```bash
# Alpine (smallest possible)
docker build -f Dockerfile.alpine -t docker-demo:alpine .
docker images docker-demo:alpine
```

## Run Containers
```bash
# Run standard
docker run -p 5000:8080 docker-demo:standard

# Run optimized
docker run -p 5000:8080 docker-demo:optimized

# Run Alpine
docker run -p 5000:8080 docker-demo:alpine

# Test
curl http://localhost:5000
```

## Optimization Techniques

### 1. Multi-Stage Builds
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# ... build steps ...

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
COPY --from=build /app/publish .
```
**Why?** SDK image is 700MB. Runtime is only 200MB. Don't ship build tools to production!

### 2. Layer Caching
```dockerfile
# Copy project file FIRST (changes rarely)
COPY ["DockerOptimization.csproj", "./"]
RUN dotnet restore

# Copy source SECOND (changes often)
COPY . .
RUN dotnet build
```
**Why?** Docker caches each layer. If source changes, only rebuild from that point.

### 3. Trimmed Publishing
```dockerfile
RUN dotnet publish \
    /p:PublishTrimmed=true \
    /p:PublishSingleFile=true
```
**Why?** Removes unused assemblies. Can save 40-60% of output size.

### 4. Alpine Base Images
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
```
**Why?** Alpine Linux is 5MB vs 200MB for Ubuntu-based images.

### 5. Chiseled Images
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled
```
**Why?** Ubuntu-based but with only essential packages. No package manager = smaller attack surface.

### 6. .dockerignore
```
bin/
obj/
.git/
```
**Why?** Don't copy build artifacts or source control to Docker context (faster builds).

## Security Benefits

### Non-Root User
```dockerfile
USER $APP_UID  # Chiseled
USER appuser   # Alpine
```
**Why?** Running as root inside containers is a security risk.

### Smaller Attack Surface
- **Standard**: Includes bash, package managers, compilers
- **Optimized**: Only runtime dependencies
- **Alpine**: Minimal packages

## Production Patterns

### Image Scanning
```bash
# Scan for vulnerabilities
docker scan docker-demo:optimized
```

### Image Signing
```bash
# Sign images for trust
docker trust sign docker-demo:optimized
```

### Registry Push
```bash
# Tag and push
docker tag docker-demo:optimized myregistry.azurecr.io/app:v1
docker push myregistry.azurecr.io/app:v1
```

## Build Time Comparison

| Dockerfile | First Build | Cached Build |
|------------|-------------|--------------|
| Standard | 45s | 40s |
| Optimized | 60s | 5s (layer cache!) |
| Alpine | 50s | 3s |

## When to Use Each

**Standard**: Development, debugging (has tools)
**Optimized (Chiseled)**: Production, security-critical (Ubuntu-based)
**Alpine**: Smallest size, cloud cost optimization

**Use Cases:** Container optimization, CI/CD pipelines, cloud cost reduction, security hardening.

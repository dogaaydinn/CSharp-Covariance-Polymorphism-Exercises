# Distributed Caching with Redis Dockerfile
# Multi-stage build for production-ready caching demo

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY *.csproj .
RUN dotnet restore

# Copy source code and build
COPY . .
RUN dotnet build -c Release -o /app/build

# Publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app

# Copy published files
COPY --from=build /app/publish .

# Environment variables
ENV DOTNET_ENVIRONMENT=Production
ENV ASPNETCORE_ENVIRONMENT=Production
# Redis connection string (override with docker run -e)
ENV REDIS_CONNECTION=redis:6379

# Health check (optional - if running as web service)
# HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
#   CMD curl -f http://localhost:5000/health || exit 1

# Run the application
ENTRYPOINT ["dotnet", "CachingExample.dll"]

# ============================================================================
# Docker Compose Example (docker-compose.yml):
# ============================================================================
# version: '3.8'
# services:
#   redis:
#     image: redis:7-alpine
#     ports:
#       - "6379:6379"
#     volumes:
#       - redis-data:/data
#     command: redis-server --appendonly yes
#
#   caching-demo:
#     build: .
#     depends_on:
#       - redis
#     environment:
#       - REDIS_CONNECTION=redis:6379
#
# volumes:
#   redis-data:
# ============================================================================

# ============================================================================
# Usage:
# ============================================================================
# Build:
#   docker build -t caching-example .
#
# Run with Redis:
#   docker run --rm -e REDIS_CONNECTION=localhost:6379 caching-example
#
# Run with Docker Compose:
#   docker-compose up
#
# Run without Redis (MemoryCache only):
#   docker run --rm -e REDIS_CONNECTION=invalid:1234 caching-example
# ============================================================================

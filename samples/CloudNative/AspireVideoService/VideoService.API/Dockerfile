# Multi-stage Dockerfile for AspireVideoService API
# Optimized for production with minimal image size

# Build arguments
ARG BUILD_DATE
ARG VCS_REF
ARG VERSION=1.0.0

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Copy solution and project files for dependency restoration
COPY ["VideoService.API/VideoService.API.csproj", "VideoService.API/"]
COPY ["VideoService.ServiceDefaults/VideoService.ServiceDefaults.csproj", "VideoService.ServiceDefaults/"]

# Restore dependencies (cached layer)
RUN dotnet restore "VideoService.API/VideoService.API.csproj" \
    --runtime linux-musl-x64

# Copy source code
COPY . .

# Build and publish
WORKDIR "/src/VideoService.API"
RUN dotnet publish \
    --configuration Release \
    --runtime linux-musl-x64 \
    --self-contained false \
    --no-restore \
    --output /app/publish \
    /p:PublishTrimmed=false \
    /p:PublishReadyToRun=true \
    /p:PublishSingleFile=false

# Stage 2: Runtime (Minimal Alpine-based image)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime

# Install dependencies for health checks
RUN apk add --no-cache curl

# Create non-root user
RUN addgroup -g 1000 appuser && \
    adduser -D -u 1000 -G appuser appuser

WORKDIR /app

# Copy published app from build stage
COPY --from=build --chown=appuser:appuser /app/publish .

# Switch to non-root user
USER appuser

# Configure environment
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_EnableDiagnostics=0

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Expose port
EXPOSE 8080

# Labels (OCI standard)
LABEL org.opencontainers.image.created="${BUILD_DATE}" \
      org.opencontainers.image.title="AspireVideoService API" \
      org.opencontainers.image.description="Cloud-native video service API built with .NET Aspire" \
      org.opencontainers.image.version="${VERSION}" \
      org.opencontainers.image.revision="${VCS_REF}" \
      org.opencontainers.image.vendor="AdvancedCSharpConcepts" \
      org.opencontainers.image.licenses="MIT" \
      org.opencontainers.image.source="https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises"

# Entry point
ENTRYPOINT ["dotnet", "VideoService.API.dll"]

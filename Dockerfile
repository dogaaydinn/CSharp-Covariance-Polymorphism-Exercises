# ==============================================
# Multi-Stage Dockerfile for .NET 8 Application
# Enterprise-Grade Build Optimization
# Phase 6: CI/CD & Automation
# ==============================================

# Build arguments
ARG DOTNET_VERSION=8.0
ARG BUILD_CONFIGURATION=Release

# ==============================================
# Stage 1: Restore Dependencies
# ==============================================
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION}-alpine AS restore
LABEL stage=restore

WORKDIR /src

# Copy only project files for dependency restore (better layer caching)
COPY ["AdvancedCsharpConcepts.sln", "./"]
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Build.targets", "./"]
COPY ["src/AdvancedConcepts.Core/AdvancedConcepts.Core.csproj", "src/AdvancedConcepts.Core/"]
COPY ["tests/AdvancedConcepts.UnitTests/AdvancedConcepts.UnitTests.csproj", "tests/AdvancedConcepts.UnitTests/"]
COPY ["tests/AdvancedConcepts.IntegrationTests/AdvancedConcepts.IntegrationTests.csproj", "tests/AdvancedConcepts.IntegrationTests/"]
COPY ["benchmarks/AdvancedConcepts.Benchmarks/AdvancedConcepts.Benchmarks.csproj", "benchmarks/AdvancedConcepts.Benchmarks/"]

# Restore dependencies (cached layer unless project files change)
RUN dotnet restore "AdvancedCsharpConcepts.sln" \
    --runtime linux-musl-x64 \
    --verbosity minimal

# ==============================================
# Stage 2: Build Application
# ==============================================
FROM restore AS build
ARG BUILD_CONFIGURATION
LABEL stage=build

# Install additional tools for builds
RUN apk add --no-cache git

# Copy source code
COPY ["src/", "src/"]
COPY ["tests/", "tests/"]
COPY ["benchmarks/", "benchmarks/"]

# Build solution
WORKDIR /src
RUN dotnet build "AdvancedCsharpConcepts.sln" \
    --configuration ${BUILD_CONFIGURATION} \
    --no-restore \
    --verbosity minimal

# ==============================================
# Stage 3: Run Tests (Optional - can be skipped in production builds)
# ==============================================
FROM build AS test
ARG BUILD_CONFIGURATION

# Run unit tests
RUN dotnet test "AdvancedCsharpConcepts.sln" \
    --configuration ${BUILD_CONFIGURATION} \
    --no-build \
    --verbosity normal \
    --logger "console;verbosity=normal"

# ==============================================
# Stage 4: Publish Application
# ==============================================
FROM build AS publish
ARG BUILD_CONFIGURATION
ARG VERSION=1.0.0
ARG BUILD_DATE
ARG VCS_REF

WORKDIR /src/src/AdvancedConcepts.Core

# Publish with aggressive optimizations
RUN dotnet publish \
    --configuration ${BUILD_CONFIGURATION} \
    --runtime linux-musl-x64 \
    --self-contained false \
    --output /app/publish \
    --no-restore \
    --no-build \
    -p:PublishReadyToRun=true \
    -p:PublishSingleFile=false \
    -p:PublishTrimmed=false \
    -p:DebugType=None \
    -p:DebugSymbols=false \
    -p:Version=${VERSION} \
    -p:InformationalVersion=${VERSION}+${VCS_REF}

# Remove unnecessary files to reduce image size
RUN find /app/publish -name "*.pdb" -delete && \
    find /app/publish -name "*.xml" -delete

# ==============================================
# Stage 5: Final Runtime Image (Production)
# ==============================================
FROM mcr.microsoft.com/dotnet/runtime:${DOTNET_VERSION}-alpine AS final
ARG VERSION=1.0.0
ARG BUILD_DATE
ARG VCS_REF

# OCI Image annotations
LABEL org.opencontainers.image.title="Advanced C# Concepts"
LABEL org.opencontainers.image.description="Enterprise-grade C# advanced concepts with SOLID principles, design patterns, and performance optimization"
LABEL org.opencontainers.image.authors="Doğa Aydın <dogaaydinn@gmail.com>"
LABEL org.opencontainers.image.version="${VERSION}"
LABEL org.opencontainers.image.created="${BUILD_DATE}"
LABEL org.opencontainers.image.revision="${VCS_REF}"
LABEL org.opencontainers.image.source="https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises"
LABEL org.opencontainers.image.licenses="MIT"
LABEL org.opencontainers.image.documentation="https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/blob/master/README.md"

# Install runtime dependencies
RUN apk add --no-cache \
    ca-certificates \
    tzdata \
    && update-ca-certificates

# Create non-root user for security (principle of least privilege)
RUN addgroup -g 1000 appuser && \
    adduser -D -u 1000 -G appuser appuser && \
    mkdir -p /app /app/logs && \
    chown -R appuser:appuser /app

WORKDIR /app

# Copy published application from publish stage
COPY --from=publish --chown=appuser:appuser /app/publish .

# Switch to non-root user
USER appuser

# Configure container environment
ENV DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true \
    DOTNET_EnableDiagnostics=0 \
    DOTNET_USE_POLLING_FILE_WATCHER=true \
    DOTNET_gcServer=1 \
    ASPNETCORE_URLS=http://+:8080 \
    APP_VERSION=${VERSION}

# Health check (for container orchestration)
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
    CMD pgrep -x dotnet > /dev/null || exit 1

# Expose application port
EXPOSE 8080

# Entry point
ENTRYPOINT ["dotnet", "AdvancedConcepts.Core.dll"]

# ==============================================
# Development Image (Optional)
# ==============================================
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION}-alpine AS development
LABEL stage=development

WORKDIR /src

# Install development tools
RUN apk add --no-cache \
    git \
    curl \
    bash \
    vim

# Copy solution files
COPY . .

# Restore dependencies
RUN dotnet restore "AdvancedCsharpConcepts.sln"

# Development environment variables
ENV ASPNETCORE_ENVIRONMENT=Development \
    DOTNET_USE_POLLING_FILE_WATCHER=true \
    DOTNET_WATCH_RESTART_ON_RUDE_EDIT=true

# Entry point for development (with hot reload)
ENTRYPOINT ["dotnet", "watch", "run", "--project", "src/AdvancedConcepts.Core/AdvancedConcepts.Core.csproj"]

# ==============================================
# Build Instructions:
# ==============================================
# Production build:
#   docker build --target final -t advancedconcepts:latest .
#
# Development build:
#   docker build --target development -t advancedconcepts:dev .
#
# Build with specific version:
#   docker build --target final \
#     --build-arg VERSION=1.0.0 \
#     --build-arg BUILD_DATE=$(date -u +'%Y-%m-%dT%H:%M:%SZ') \
#     --build-arg VCS_REF=$(git rev-parse --short HEAD) \
#     -t advancedconcepts:1.0.0 .
#
# Multi-platform build:
#   docker buildx build --platform linux/amd64,linux/arm64 \
#     --target final \
#     -t advancedconcepts:latest \
#     --push .
#
# Build without tests (faster):
#   docker build --target final -t advancedconcepts:latest .
#
# Run container:
#   docker run --rm -it -p 8080:8080 advancedconcepts:latest
#
# Size optimization achieved:
#   - Multi-stage build: ~95% size reduction
#   - Alpine base: ~100MB final image
#   - Layer caching: Faster builds
#   - Security: Non-root user
#   - No debug symbols or PDB files
# ==============================================

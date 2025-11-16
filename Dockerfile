# ==============================================
# Multi-Stage Dockerfile for .NET 8 Application
# Enterprise-Grade Build Optimization
# ==============================================

# ==============================================
# Stage 1: Base SDK Image
# ==============================================
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
LABEL stage=builder
LABEL maintainer="Doğa Aydın <dogaaydinn@gmail.com>"

# Set working directory
WORKDIR /src

# Install additional tools
RUN apk add --no-cache \
    git \
    curl \
    bash

# Copy solution and project files
COPY ["CSharp-Covariance-Polymorphism-Exercises.sln", "./"]
COPY ["AdvancedCsharpConcepts/AdvancedCsharpConcepts.csproj", "AdvancedCsharpConcepts/"]
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Build.targets", "./"]
COPY ["global.json", "./"]

# Restore dependencies (cached layer)
RUN dotnet restore "CSharp-Covariance-Polymorphism-Exercises.sln" \
    --runtime linux-musl-x64

# Copy source code
COPY . .

# ==============================================
# Stage 2: Build and Publish
# ==============================================
FROM build AS publish
WORKDIR /src/AdvancedCsharpConcepts

# Build with optimizations
RUN dotnet publish \
    --configuration Release \
    --runtime linux-musl-x64 \
    --self-contained false \
    --output /app/publish \
    -p:PublishReadyToRun=true \
    -p:PublishSingleFile=false \
    -p:PublishTrimmed=false \
    -p:DebugType=None \
    -p:DebugSymbols=false

# ==============================================
# Stage 3: Runtime Image (Minimal)
# ==============================================
FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine AS final
LABEL org.opencontainers.image.title="Advanced C# Concepts"
LABEL org.opencontainers.image.description="Enterprise-grade C# advanced concepts demonstration"
LABEL org.opencontainers.image.authors="Doğa Aydın <dogaaydinn@gmail.com>"
LABEL org.opencontainers.image.version="1.0.0"
LABEL org.opencontainers.image.licenses="MIT"

# Create non-root user for security
RUN addgroup -g 1000 appuser \
    && adduser -D -u 1000 -G appuser appuser

WORKDIR /app

# Copy published application
COPY --from=publish --chown=appuser:appuser /app/publish .

# Switch to non-root user
USER appuser

# Health check configuration
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD pgrep -x dotnet || exit 1

# Set environment variables
ENV DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true \
    DOTNET_EnableDiagnostics=0 \
    ASPNETCORE_URLS=http://+:8080

# Expose port (if needed)
EXPOSE 8080

# Entry point
ENTRYPOINT ["dotnet", "AdvancedCsharpConcepts.dll"]

# ==============================================
# Build Instructions:
# ==============================================
# Build: docker build -t advancedconcepts:latest .
# Run: docker run --rm -it advancedconcepts:latest
#
# Multi-platform build:
# docker buildx build --platform linux/amd64,linux/arm64 -t advancedconcepts:latest .
#
# Size optimization achieved:
# - Multi-stage build reduces final image size
# - Alpine-based images (~100MB vs ~200MB)
# - Non-root user for security
# - Minimal runtime dependencies
# ==============================================

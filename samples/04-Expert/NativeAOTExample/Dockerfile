# Native AOT Dockerfile
# Demonstrates ahead-of-time compilation for minimal container size and fast startup

# ============================================
# Stage 1: Build with Native AOT
# ============================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Install Native AOT prerequisites
RUN apt-get update && apt-get install -y \
    clang \
    zlib1g-dev \
    && rm -rf /var/lib/apt/lists/*

# Copy project file
COPY NativeAOTExample.csproj .

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY . .

# Build with Native AOT
# This produces a native executable (no .NET runtime needed!)
RUN dotnet publish -c Release -o /app/publish \
    /p:PublishAot=true \
    /p:StripSymbols=true \
    /p:IlcOptimizationPreference=Speed

# ============================================
# Stage 2: Runtime (minimal - no .NET runtime!)
# ============================================
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-alpine AS runtime
WORKDIR /app

# Copy only the native executable (5-10MB!)
COPY --from=build /app/publish/NativeAOTExample .

# Set permissions
RUN chmod +x /app/NativeAOTExample

# Native AOT binary runs directly - NO RUNTIME NEEDED! 🚀
ENTRYPOINT ["/app/NativeAOTExample"]

# ============================================
# BENEFITS:
# - Binary size: ~5-10MB (vs ~150MB with runtime)
# - Startup time: ~5-10ms (vs ~200ms standard)
# - No .NET runtime required
# - Smaller attack surface
# - Faster cold start in containers/serverless
# ============================================

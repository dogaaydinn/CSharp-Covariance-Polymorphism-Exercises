# High Performance Span Dockerfile
# Demonstrates zero-allocation programming with Span<T>

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file
COPY HighPerformanceSpan.csproj .

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY . .

# Build the application
RUN dotnet build -c Release -o /app/build

# Publish
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app

# Copy published files
COPY --from=build /app/publish .

# Set environment variables
ENV DOTNET_ENVIRONMENT=Production

# Run the application
ENTRYPOINT ["dotnet", "HighPerformanceSpan.dll"]

# ============================================
# SPAN<T> BENEFITS:
# ============================================
# - Zero allocations for slicing/parsing
# - 100x faster than string.Split()
# - Stack allocation with stackalloc
# - No GC pressure
# - Cache-friendly access patterns
#
# USE CASES:
# - High-performance parsers
# - Data processing pipelines
# - Real-time systems
# - Game engines
# - Low-latency services
# ============================================

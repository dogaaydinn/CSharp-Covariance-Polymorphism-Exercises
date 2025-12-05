# Background Service Example Dockerfile
# Demonstrates long-running background tasks with IHostedService

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file
COPY *.csproj .

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
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check (if applicable)
# HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
#   CMD curl -f http://localhost:5000/health || exit 1

# Run the application
ENTRYPOINT ["dotnet", "BackgroundServiceExample.dll"]

# ============================================
# BACKGROUND SERVICE NOTES:
# ============================================
# - Runs continuously in background
# - IHostedService interface
# - Graceful shutdown support
# - Scoped service injection
# - Health monitoring
#
# USE CASES:
# - Data processing workers
# - Message queue consumers
# - Scheduled jobs (cron-like)
# - Cache warming
# - Metrics collection
# ============================================

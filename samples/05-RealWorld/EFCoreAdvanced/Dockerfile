# EF Core Advanced Dockerfile
# Demonstrates advanced Entity Framework Core patterns

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
ENV ConnectionStrings__DefaultConnection="Data Source=efcore.db"

# Run the application
ENTRYPOINT ["dotnet", "EFCoreAdvanced.dll"]

# ============================================
# EF CORE ADVANCED FEATURES:
# ============================================
# - Global query filters (soft delete)
# - Owned entities
# - Table splitting
# - Shadow properties
# - Interceptors
# - Compiled queries
# - Raw SQL queries
# - Change tracking optimization
#
# PRODUCTION NOTES:
# - Use connection pooling
# - Enable query caching
# - Configure retry policies
# - Use AsNoTracking for read-only
# - Batch operations where possible
# ============================================

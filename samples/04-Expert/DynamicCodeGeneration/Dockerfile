# Dynamic Code Generation Dockerfile
# Demonstrates runtime code generation with Reflection.Emit

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file
COPY DynamicCodeGeneration.csproj .

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

# IMPORTANT: Dynamic code generation requires runtime JIT compiler
# Cannot use Native AOT with Reflection.Emit!

# Run the application
ENTRYPOINT ["dotnet", "DynamicCodeGeneration.dll"]

# ============================================
# NOTES:
# - Reflection.Emit requires full .NET runtime
# - Not compatible with Native AOT
# - Runtime generates IL code dynamically
# - Use cases: ORMs, serializers, proxies
# ============================================

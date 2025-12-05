# gRPC Example Dockerfile
# Demonstrates high-performance RPC with Protocol Buffers

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file
COPY *.csproj .

# Restore dependencies (includes Grpc.Tools for protobuf compilation)
RUN dotnet restore

# Copy source code (including .proto files)
COPY . .

# Build the application (protobuf files are auto-generated)
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

# Expose gRPC port (if server)
# EXPOSE 5000

# Run the application
ENTRYPOINT ["dotnet", "gRPCExample.dll"]

# ============================================
# gRPC ADVANTAGES:
# ============================================
# - 10-20x faster than REST/JSON
# - Binary protocol (Protocol Buffers)
# - HTTP/2 multiplexing
# - Streaming (client, server, bidirectional)
# - Strong typing
# - Code generation
#
# USE CASES:
# - Microservices communication
# - Mobile to backend
# - Real-time applications
# - IoT devices
# - Internal APIs (not public-facing)
#
# COMPARISON:
# - gRPC: Fast, typed, complex
# - REST: Universal, simple, slower
# - GraphQL: Flexible queries, HTTP/1.1
#
# NOTES:
# - Requires HTTP/2
# - Not browser-friendly (use gRPC-Web)
# - Best for internal services
# ============================================

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["MicroVideoPlatform.ApiGateway/MicroVideoPlatform.ApiGateway.csproj", "MicroVideoPlatform.ApiGateway/"]
COPY ["MicroVideoPlatform.Shared/MicroVideoPlatform.Shared.csproj", "MicroVideoPlatform.Shared/"]

# Restore dependencies
RUN dotnet restore "MicroVideoPlatform.ApiGateway/MicroVideoPlatform.ApiGateway.csproj"

# Copy all source code
COPY . .

# Build the project
WORKDIR "/src/MicroVideoPlatform.ApiGateway"
RUN dotnet build "MicroVideoPlatform.ApiGateway.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "MicroVideoPlatform.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Create logs directory
RUN mkdir -p /logs && chmod 777 /logs

# Copy published output
COPY --from=publish /app/publish .

# Expose ports
EXPOSE 80
EXPOSE 443

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
  CMD curl -f http://localhost:80/health || exit 1

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Entry point
ENTRYPOINT ["dotnet", "MicroVideoPlatform.ApiGateway.dll"]

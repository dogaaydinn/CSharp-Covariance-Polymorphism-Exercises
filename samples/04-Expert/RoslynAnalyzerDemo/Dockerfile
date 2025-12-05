# RoslynAnalyzerDemo Dockerfile
# Multi-stage build for Roslyn analyzer consumer application

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY Analyzer/Analyzer.csproj Analyzer/
COPY Consumer/Consumer.csproj Consumer/

# Restore dependencies
RUN dotnet restore Consumer/Consumer.csproj

# Copy source code
COPY Analyzer/ Analyzer/
COPY Consumer/ Consumer/

# Build the analyzer first
WORKDIR /src/Analyzer
RUN dotnet build Analyzer.csproj -c Release

# Build the consumer application
WORKDIR /src/Consumer
RUN dotnet build Consumer.csproj -c Release -o /app/build

# Publish
RUN dotnet publish Consumer.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app

# Copy published files
COPY --from=build /app/publish .

# Set environment variables
ENV DOTNET_ENVIRONMENT=Production
ENV DOTNET_EnableDiagnostics=0

# Run the application
ENTRYPOINT ["dotnet", "Consumer.dll"]

# Unsafe Code Example Dockerfile
# Demonstrates pointer arithmetic and high-performance unsafe code

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file
COPY UnsafeCodeExample.csproj .

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY . .

# Build the application (with AllowUnsafeBlocks enabled)
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
ENTRYPOINT ["dotnet", "UnsafeCodeExample.dll"]

# ============================================
# UNSAFE CODE NOTES:
# ============================================
# - AllowUnsafeBlocks must be enabled in .csproj
# - Direct memory access (pointers)
# - No bounds checking (faster but riskier)
# - Use for extreme performance scenarios
# - stackalloc for zero-allocation buffers
#
# USE CASES:
# - Image/video processing
# - Game engines
# - Scientific computing
# - Interop with native code
# ============================================

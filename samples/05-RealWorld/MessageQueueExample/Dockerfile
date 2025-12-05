# Order Processing with RabbitMQ Dockerfile
# Multi-stage build for production-ready message queue demo

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY *.csproj .
RUN dotnet restore

# Copy source code and build
COPY . .
RUN dotnet build -c Release -o /app/build

# Publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app

# Copy published files
COPY --from=build /app/publish .

# Environment variables
ENV DOTNET_ENVIRONMENT=Production
ENV ASPNETCORE_ENVIRONMENT=Production
# RabbitMQ connection (override with docker run -e)
ENV RABBITMQ_HOST=rabbitmq

# Run the application
ENTRYPOINT ["dotnet", "MessageQueueExample.dll"]

# ============================================================================
# Docker Compose Example (docker-compose.yml):
# ============================================================================
# version: '3.8'
# services:
#   rabbitmq:
#     image: rabbitmq:3.13-management-alpine
#     ports:
#       - "5672:5672"   # AMQP
#       - "15672:15672" # Management UI
#     environment:
#       RABBITMQ_DEFAULT_USER: guest
#       RABBITMQ_DEFAULT_PASS: guest
#     volumes:
#       - rabbitmq-data:/var/lib/rabbitmq
#     healthcheck:
#       test: rabbitmq-diagnostics -q ping
#       interval: 10s
#       timeout: 5s
#       retries: 5
#
#   message-queue-demo:
#     build: .
#     depends_on:
#       rabbitmq:
#         condition: service_healthy
#     environment:
#       - RABBITMQ_HOST=rabbitmq
#
# volumes:
#   rabbitmq-data:
# ============================================================================

# ============================================================================
# Usage:
# ============================================================================
# Build:
#   docker build -t message-queue-example .
#
# Run with RabbitMQ:
#   docker run --rm -e RABBITMQ_HOST=localhost message-queue-example
#
# Run with Docker Compose:
#   docker-compose up
#
# Run without RabbitMQ (Channel<T> fallback):
#   docker run --rm -e RABBITMQ_HOST=invalid message-queue-example
#
# Access RabbitMQ Management UI:
#   http://localhost:15672 (guest/guest)
# ============================================================================

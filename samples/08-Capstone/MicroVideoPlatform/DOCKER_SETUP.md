# 🐳 Docker Infrastructure Setup Guide

Complete guide to deploying the Micro-Video Platform using Docker Compose.

---

## 📋 Table of Contents

1. [Prerequisites](#prerequisites)
2. [Quick Start](#quick-start)
3. [Infrastructure Components](#infrastructure-components)
4. [Environment Configuration](#environment-configuration)
5. [Service Ports](#service-ports)
6. [Volume Management](#volume-management)
7. [Networking](#networking)
8. [Development vs Production](#development-vs-production)
9. [Troubleshooting](#troubleshooting)
10. [Makefile Commands](#makefile-commands)

---

## Prerequisites

### Required Software

- **Docker**: Version 24.0+ ([Install Docker](https://docs.docker.com/get-docker/))
- **Docker Compose**: Version 2.0+ (included with Docker Desktop)
- **.NET SDK**: Version 8.0+ (for local development)
- **Make**: For Makefile commands (optional but recommended)

### System Requirements

**Minimum:**
- CPU: 2 cores
- RAM: 4 GB
- Disk: 10 GB free space

**Recommended:**
- CPU: 4+ cores
- RAM: 8+ GB
- Disk: 20+ GB free space

---

## Quick Start

### 1. Clone and Navigate

```bash
cd samples/08-Capstone/MicroVideoPlatform
```

### 2. First-Time Setup

```bash
# Option 1: Using Makefile (Recommended)
make setup

# Option 2: Manual setup
cp .env .env.local
mkdir -p uploads ml-models logs backups
docker-compose build
docker-compose up -d
```

### 3. Verify Services

```bash
# Check service status
make status

# Check health endpoints
make health

# View logs
make logs
```

### 4. Access Services

Once all services are healthy, access them at:

| Service | URL | Credentials |
|---------|-----|-------------|
| **ApiGateway** | http://localhost:5000 | - |
| **Content.API** | http://localhost:5001 | JWT required |
| **Analytics.Function** | http://localhost:5003 | - |
| **Web.UI** | http://localhost:5004 | - |
| **RabbitMQ Management** | http://localhost:15672 | guest/guest |
| **Adminer (DB UI)** | http://localhost:8080 | postgres/postgres |
| **Redis Commander** | http://localhost:8081 | - |
| **MailHog (Email)** | http://localhost:8025 | - |
| **Seq (Logs)** | http://localhost:5341 | - |

---

## Infrastructure Components

### Core Infrastructure Services

#### 1. PostgreSQL 16
- **Purpose:** Primary data store for video metadata
- **Port:** 5432
- **Database:** `microvideo`
- **Features:**
  - Automatic schema initialization via `init-db.sql`
  - Full-text search indexes
  - Sample data seeding
  - Automatic backups

#### 2. Redis 7.4
- **Purpose:** Caching layer for API responses
- **Port:** 6379
- **Features:**
  - AOF persistence enabled
  - Password protection
  - Command logging

#### 3. RabbitMQ 3.12
- **Purpose:** Event bus for asynchronous communication
- **Ports:**
  - 5672: AMQP protocol
  - 15672: Management UI
- **Features:**
  - Pre-configured exchanges and queues
  - Dead letter queue (DLQ)
  - High availability policies
  - Message persistence

### Application Services

#### 1. Content.API
- **Technology:** ASP.NET Core WebAPI
- **Port:** 5001
- **Features:**
  - Video metadata CRUD
  - JWT authentication
  - Redis caching
  - Domain event publishing

#### 2. Processing.Worker
- **Technology:** .NET Background Service
- **Port:** Internal only
- **Features:**
  - CQRS with MediatR
  - Domain-Driven Design
  - Event-driven processing
  - FFmpeg simulation

#### 3. Analytics.Function
- **Technology:** ASP.NET Core WebAPI + ML.NET
- **Port:** 5003
- **Features:**
  - Video content classification
  - ML model inference
  - Confidence scoring

#### 4. Web.UI
- **Technology:** Blazor Server
- **Port:** 5004
- **Features:**
  - Video upload interface
  - Real-time updates with SignalR
  - Responsive design

#### 5. ApiGateway
- **Technology:** YARP (Yet Another Reverse Proxy)
- **Port:** 5000
- **Features:**
  - Request routing
  - Rate limiting
  - Load balancing

---

## Environment Configuration

### Default Configuration (.env)

```bash
# PostgreSQL
POSTGRES_DB=microvideo
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres

# Redis
REDIS_PASSWORD=redis

# RabbitMQ
RABBITMQ_USER=guest
RABBITMQ_PASSWORD=guest
RABBITMQ_VHOST=/

# JWT
JWT_SECRET=your-super-secret-key-change-in-production-minimum-32-characters
JWT_ISSUER=MicroVideoPlatform
JWT_AUDIENCE=MicroVideoPlatform
JWT_EXPIRY_MINUTES=60

# Processing
PROCESSING_SIMULATE_DELAY=true
PROCESSING_DELAY_SECONDS=5

# ML.NET
ML_MODEL_PATH=/app/models/video-classifier.zip
ML_CONFIDENCE_THRESHOLD=0.7

# Rate Limiting
RATE_LIMITING_ENABLED=true
RATE_LIMITING_PERMIT_LIMIT=100
RATE_LIMITING_WINDOW_SECONDS=60
```

### Production Configuration

⚠️ **CRITICAL: Before deploying to production:**

1. **Create `.env.local` file:**
```bash
cp .env .env.local
```

2. **Update sensitive values:**
```bash
# Generate strong passwords
openssl rand -base64 32  # For JWT_SECRET
openssl rand -base64 16  # For POSTGRES_PASSWORD
openssl rand -base64 16  # For REDIS_PASSWORD
openssl rand -base64 16  # For RABBITMQ_PASSWORD
```

3. **Set production environment:**
```bash
ASPNETCORE_ENVIRONMENT=Production
ENABLE_SWAGGER=false
PROCESSING_SIMULATE_DELAY=false
SERILOG_MINIMUM_LEVEL=Warning
```

4. **Verify `.gitignore` excludes `.env.local`:**
```bash
grep ".env.local" .gitignore  # Should return .env.local
```

---

## Service Ports

### External (Host → Container)

| Service | Host Port | Container Port | Protocol |
|---------|-----------|----------------|----------|
| ApiGateway | 5000 | 8080 | HTTP |
| Content.API | 5001 | 8080 | HTTP |
| Analytics.Function | 5003 | 8080 | HTTP |
| Web.UI | 5004 | 8080 | HTTP |
| PostgreSQL | 5432 | 5432 | TCP |
| Redis | 6379 | 6379 | TCP |
| RabbitMQ (AMQP) | 5672 | 5672 | AMQP |
| RabbitMQ (Management) | 15672 | 15672 | HTTP |
| Adminer | 8080 | 8080 | HTTP |
| Redis Commander | 8081 | 8081 | HTTP |
| MailHog (SMTP) | 1025 | 1025 | SMTP |
| MailHog (Web) | 8025 | 8025 | HTTP |
| Seq | 5341 | 80 | HTTP |

### Internal (Container → Container)

Services communicate using service names on the `microvideo-network`:

```
content-api:8080
processing-worker:8080
analytics-function:8080
postgres:5432
redis:6379
rabbitmq:5672
```

---

## Volume Management

### Persistent Volumes

Docker Compose creates named volumes for data persistence:

```bash
# List volumes
docker volume ls | grep microvideo

# Expected volumes:
# - microvideo-postgres-data (Database)
# - microvideo-redis-data (Cache)
# - microvideo-rabbitmq-data (Message queue)
# - microvideo-seq-data (Logs)
```

### Backup and Restore

#### PostgreSQL Backup

```bash
# Using Makefile
make db-backup

# Manual
docker-compose exec -T postgres pg_dump -U postgres microvideo > backup_$(date +%Y%m%d).sql
```

#### PostgreSQL Restore

```bash
# Using Makefile
make db-restore FILE=backup_20250102.sql

# Manual
cat backup_20250102.sql | docker-compose exec -T postgres psql -U postgres microvideo
```

#### Full Volume Backup

```bash
# Backup all volumes
docker run --rm \
  -v microvideo-postgres-data:/data \
  -v $(pwd)/backups:/backup \
  alpine tar czf /backup/postgres-data.tar.gz -C /data .
```

---

## Networking

### Network Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                   microvideo-network (bridge)               │
│                     Subnet: 172.20.0.0/16                   │
│                                                             │
│  ┌──────────┐   ┌──────────┐   ┌──────────┐               │
│  │ Web.UI   │───│ApiGateway│───│Content.API│               │
│  │ :5004    │   │  :5000   │   │  :5001    │               │
│  └──────────┘   └──────────┘   └─────┬─────┘               │
│                                       │                      │
│                         ┌─────────────┼─────────────┐       │
│                         ▼             ▼             ▼       │
│                  ┌──────────┐  ┌──────────┐  ┌──────────┐  │
│                  │PostgreSQL│  │  Redis   │  │ RabbitMQ │  │
│                  │  :5432   │  │  :6379   │  │  :5672   │  │
│                  └──────────┘  └──────────┘  └──────────┘  │
└─────────────────────────────────────────────────────────────┘
```

### DNS Resolution

Services communicate using Docker's internal DNS:

```bash
# Content.API connects to PostgreSQL
Host=postgres;Port=5432

# Processing.Worker connects to RabbitMQ
RabbitMQ__Host=rabbitmq

# ApiGateway routes to Content.API
http://content-api:8080
```

---

## Development vs Production

### Development Mode

**Enabled by default** with `docker-compose.override.yml`:

```bash
# Start in development mode
docker-compose up -d

# Or explicitly
make dev
```

**Features:**
- ✅ Hot reload enabled
- ✅ Swagger UI enabled
- ✅ Debug logging
- ✅ Sample data seeded
- ✅ Development tools (Adminer, Redis Commander, MailHog, Seq)
- ✅ Rate limiting disabled
- ✅ Simplified passwords

### Production Mode

**Enable for production deployment:**

```bash
# Set environment
ASPNETCORE_ENVIRONMENT=Production docker-compose up -d

# Or using Makefile
make prod
```

**Changes:**
- ❌ Swagger UI disabled
- ❌ Hot reload disabled
- ❌ Debug logging disabled
- ❌ Sample data not seeded
- ❌ Development tools excluded
- ✅ Rate limiting enabled
- ✅ Strong passwords required
- ✅ TLS/SSL enabled
- ✅ Error pages sanitized

---

## Troubleshooting

### Services Won't Start

**Issue:** Services fail to start or crash immediately

**Solutions:**

1. **Check logs:**
```bash
make logs
# Or specific service
make logs-api
```

2. **Verify ports are available:**
```bash
# Check if ports are in use
lsof -i :5000
lsof -i :5432
```

3. **Clean and rebuild:**
```bash
make clean
make rebuild
```

### Database Connection Errors

**Issue:** `Npgsql.NpgsqlException: Could not connect to server`

**Solutions:**

1. **Wait for PostgreSQL to be healthy:**
```bash
docker-compose ps postgres
# Status should show "healthy"
```

2. **Verify database credentials:**
```bash
docker-compose exec postgres psql -U postgres -d microvideo
```

3. **Check connection string:**
```bash
# Should match POSTGRES_USER, POSTGRES_PASSWORD, POSTGRES_DB
Host=postgres;Port=5432;Database=microvideo;Username=postgres;Password=postgres
```

### RabbitMQ Connection Errors

**Issue:** `RabbitMQ.Client.Exceptions.BrokerUnreachableException`

**Solutions:**

1. **Check RabbitMQ status:**
```bash
make rabbitmq-status
```

2. **Verify queues exist:**
```bash
make rabbitmq-queues
```

3. **Check RabbitMQ logs:**
```bash
docker-compose logs rabbitmq
```

### Redis Connection Errors

**Issue:** `StackExchange.Redis.RedisConnectionException`

**Solutions:**

1. **Test Redis connection:**
```bash
make redis-cli
# Then type: PING
# Should return: PONG
```

2. **Verify password:**
```bash
# Check REDIS_PASSWORD in .env matches configuration
```

### Out of Memory Errors

**Issue:** Services crash with OOM (Out Of Memory) errors

**Solutions:**

1. **Increase Docker memory limit:**
   - Docker Desktop → Settings → Resources → Memory
   - Increase to 8+ GB

2. **Reduce service count:**
```bash
# Start only core services
docker-compose up -d postgres redis rabbitmq content-api
```

3. **Check memory usage:**
```bash
docker stats
```

---

## Makefile Commands

### Quick Reference

```bash
# Setup and startup
make setup              # First-time setup
make up                 # Start all services
make down               # Stop all services
make restart            # Restart all services

# Build and rebuild
make build              # Build all services
make rebuild            # Rebuild and restart

# Logs and monitoring
make logs               # Show all logs
make logs-api           # Show Content.API logs
make status             # Show service status
make health             # Check health endpoints

# Database operations
make db-connect         # Connect to PostgreSQL
make db-backup          # Backup database
make db-restore         # Restore database
make db-migrate         # Run migrations
make db-seed            # Seed sample data

# Redis operations
make redis-cli          # Connect to Redis CLI
make redis-flush        # Clear Redis cache

# RabbitMQ operations
make rabbitmq-status    # Show RabbitMQ status
make rabbitmq-queues    # List queues
make rabbitmq-purge     # Purge all queues

# Testing
make test               # Run unit tests
make test-integration   # Run integration tests
make test-api           # Test API endpoints

# Development
make dev                # Start in dev mode
make prod               # Start in prod mode
make shell-api          # Open shell in Content.API

# Maintenance
make clean              # Stop and remove volumes
make clean-images       # Remove all images
make prune              # Clean Docker system
make update             # Update all images
```

### Full Command List

Run `make help` to see all available commands:

```bash
make help
```

---

## Advanced Configuration

### Custom docker-compose Override

Create `docker-compose.override.local.yml` for personal overrides:

```yaml
version: '3.8'

services:
  content-api:
    environment:
      - MY_CUSTOM_VAR=value
    ports:
      - "5555:8080"  # Custom port
```

### SSL/TLS Configuration

For HTTPS in production:

1. **Generate certificates:**
```bash
dotnet dev-certs https -ep ./certs/aspnetapp.pfx -p YourPassword
```

2. **Update docker-compose.yml:**
```yaml
content-api:
  environment:
    - ASPNETCORE_URLS=https://+:8081;http://+:8080
    - ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
    - ASPNETCORE_Kestrel__Certificates__Default__Password=YourPassword
  volumes:
    - ./certs:/https:ro
  ports:
    - "5001:8080"
    - "5011:8081"
```

### Resource Limits

Add resource constraints in `docker-compose.yml`:

```yaml
content-api:
  deploy:
    resources:
      limits:
        cpus: '2.0'
        memory: 2G
      reservations:
        cpus: '1.0'
        memory: 1G
```

---

## Monitoring and Observability

### Health Checks

All services expose `/health` endpoints:

```bash
# Check all health endpoints
make health

# Manual check
curl http://localhost:5000/health
curl http://localhost:5001/health
curl http://localhost:5003/health
curl http://localhost:5004/health
```

### Structured Logging

Logs are collected by Seq:

1. **Access Seq UI:** http://localhost:5341
2. **Query logs:**
```
Application="Content.API" AND Level="Error"
```

### Metrics and Tracing

OpenTelemetry integration (configured in code):

```bash
# Set up Jaeger for tracing
docker run -d \
  --name jaeger \
  -p 16686:16686 \
  -p 4317:4317 \
  --network microvideo-network \
  jaegertracing/all-in-one:latest
```

---

## Production Deployment Checklist

Before deploying to production:

- [ ] Create `.env.local` with strong passwords
- [ ] Set `ASPNETCORE_ENVIRONMENT=Production`
- [ ] Disable Swagger (`ENABLE_SWAGGER=false`)
- [ ] Configure TLS/SSL certificates
- [ ] Set up automated backups
- [ ] Configure monitoring and alerting
- [ ] Review resource limits
- [ ] Test disaster recovery procedures
- [ ] Document rollback procedures
- [ ] Set up log aggregation
- [ ] Configure secrets management
- [ ] Review security hardening
- [ ] Set up CI/CD pipelines

---

## Support and Resources

- **Project README:** [README.md](README.md)
- **Architecture:** [ARCHITECTURE.md](ARCHITECTURE.md)
- **Docker Compose Docs:** https://docs.docker.com/compose/
- **ASP.NET Core Docs:** https://learn.microsoft.com/aspnet/core/
- **RabbitMQ Docs:** https://www.rabbitmq.com/documentation.html
- **PostgreSQL Docs:** https://www.postgresql.org/docs/

---

**Made with ❤️ for the .NET community**

**Last Updated:** 2025-12-02

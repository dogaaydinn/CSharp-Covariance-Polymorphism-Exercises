# 🚀 From Zero to Production: Deployment Guide

**Status:** Production-Ready Deployment Scenario
**Platform:** Azure (Free Tier Compatible)
**Container Registry:** GitHub Container Registry (GHCR) → Azure Container Registry (ACR)
**Infrastructure:** Terraform (Infrastructure as Code)

---

## 📋 Table of Contents

1. [Prerequisites](#prerequisites)
2. [Phase 1: Local Development](#phase-1-local-development)
3. [Phase 2: GitHub to GHCR](#phase-2-github-to-ghcr)
4. [Phase 3: Azure Setup](#phase-3-azure-setup)
5. [Phase 4: Deploy to Production](#phase-4-deploy-to-production)
6. [Phase 5: Configuration & Environment Variables](#phase-5-configuration--environment-variables)
7. [Phase 6: Monitoring & Health Checks](#phase-6-monitoring--health-checks)
8. [Infrastructure as Code (Terraform)](#infrastructure-as-code-terraform)
9. [Free Tier Limitations & Warnings](#free-tier-limitations--warnings)
10. [Production Checklist](#production-checklist)
11. [Troubleshooting](#troubleshooting)

---

## Prerequisites

### Required Tools
- ✅ **Docker Desktop** (for local container builds)
- ✅ **Azure CLI** (`az` command)
- ✅ **GitHub Account** (with GHCR access)
- ✅ **Azure Account** (free tier available)
- ✅ **Git** (version control)
- ✅ **Terraform** (optional, for IaC)

### Required Accounts
```bash
# Verify installations
docker --version          # Docker version 24.0+
az --version             # Azure CLI 2.50+
git --version            # Git 2.40+
terraform --version      # Terraform 1.5+ (optional)
```

---

## Phase 1: Local Development

### 1.1 Build and Test Locally

```bash
# Navigate to project root
cd MicroVideoPlatform

# Build all images
docker-compose build

# Start infrastructure (PostgreSQL, Redis, RabbitMQ, Seq)
docker-compose up -d postgres redis rabbitmq seq

# Wait for infrastructure to be healthy
docker-compose ps

# Start application services
docker-compose up -d content-api processing-worker

# Verify health
curl http://localhost:5001/health
# Expected: {"status":"Healthy","results":{"postgres":"Healthy","redis":"Healthy","rabbitmq":"Healthy"}}
```

### 1.2 Test Application

```bash
# Get JWT token (requires auth service or manual token generation)
TOKEN="your_jwt_token_here"

# Create a video
curl -X POST http://localhost:5001/api/videos \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Test Video",
    "description": "Deployment test",
    "fileName": "test.mp4",
    "fileSizeBytes": 1048576,
    "category": "Testing"
  }'

# Get all videos
curl http://localhost:5001/api/videos

# Check logs in Seq
open http://localhost:5341
```

**✅ Local Development Success Criteria:**
- All containers running (green status)
- Health check returns `Healthy`
- API responds to requests
- Logs visible in Seq
- RabbitMQ events published/consumed

---

## Phase 2: GitHub to GHCR

### 2.1 Create GitHub Repository

```bash
# Initialize Git repository (if not already)
git init
git add .
git commit -m "feat: initial Micro-Video Platform implementation"

# Add GitHub remote
git remote add origin https://github.com/YOUR_USERNAME/micro-video-platform.git
git branch -M main
git push -u origin main
```

### 2.2 Setup GitHub Container Registry (GHCR)

**GitHub Personal Access Token (PAT):**

1. Go to GitHub → Settings → Developer settings → Personal access tokens → Tokens (classic)
2. Click "Generate new token (classic)"
3. Select scopes:
   - `write:packages` (upload packages)
   - `read:packages` (download packages)
   - `delete:packages` (optional, manage packages)
4. Generate and save token securely

```bash
# Login to GHCR
echo $GITHUB_TOKEN | docker login ghcr.io -u YOUR_USERNAME --password-stdin

# Expected output:
# Login Succeeded
```

### 2.3 Build and Push Images to GHCR

```bash
# Set variables
GITHUB_USERNAME="your_github_username"
IMAGE_TAG="v1.0.0"

# Build Content.API
docker build -t ghcr.io/$GITHUB_USERNAME/microvideo-content-api:$IMAGE_TAG \
  -f MicroVideoPlatform.Content.API/Dockerfile .

# Build Processing.Worker
docker build -t ghcr.io/$GITHUB_USERNAME/microvideo-processing-worker:$IMAGE_TAG \
  -f MicroVideoPlatform.Processing.Worker/Dockerfile .

# Push to GHCR
docker push ghcr.io/$GITHUB_USERNAME/microvideo-content-api:$IMAGE_TAG
docker push ghcr.io/$GITHUB_USERNAME/microvideo-processing-worker:$IMAGE_TAG

# Tag as 'latest'
docker tag ghcr.io/$GITHUB_USERNAME/microvideo-content-api:$IMAGE_TAG \
  ghcr.io/$GITHUB_USERNAME/microvideo-content-api:latest
docker push ghcr.io/$GITHUB_USERNAME/microvideo-content-api:latest
```

**Verify in GitHub:**
- Go to `https://github.com/YOUR_USERNAME?tab=packages`
- You should see `microvideo-content-api` and `microvideo-processing-worker`

### 2.4 Automate with GitHub Actions

**Create `.github/workflows/build-and-push.yml`:**

```yaml
name: Build and Push to GHCR

on:
  push:
    branches: [main]
    tags: ['v*']
  pull_request:
    branches: [main]

env:
  REGISTRY: ghcr.io
  IMAGE_NAME_API: ${{ github.repository }}/content-api
  IMAGE_NAME_WORKER: ${{ github.repository }}/processing-worker

jobs:
  build-and-push:
    runs-on: ubuntu-latest
    permissions:
      contents: read
      packages: write

    steps:
      - name: Checkout code
        uses: actions/checkout@v4

      - name: Login to GHCR
        uses: docker/login-action@v3
        with:
          registry: ${{ env.REGISTRY }}
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}

      - name: Extract metadata (API)
        id: meta-api
        uses: docker/metadata-action@v5
        with:
          images: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME_API }}
          tags: |
            type=ref,event=branch
            type=ref,event=pr
            type=semver,pattern={{version}}
            type=semver,pattern={{major}}.{{minor}}
            type=sha

      - name: Build and push Content.API
        uses: docker/build-push-action@v5
        with:
          context: .
          file: MicroVideoPlatform.Content.API/Dockerfile
          push: true
          tags: ${{ steps.meta-api.outputs.tags }}
          labels: ${{ steps.meta-api.outputs.labels }}

      - name: Extract metadata (Worker)
        id: meta-worker
        uses: docker/metadata-action@v5
        with:
          images: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME_WORKER }}

      - name: Build and push Processing.Worker
        uses: docker/build-push-action@v5
        with:
          context: .
          file: MicroVideoPlatform.Processing.Worker/Dockerfile
          push: true
          tags: ${{ steps.meta-worker.outputs.tags }}
          labels: ${{ steps.meta-worker.outputs.labels }}
```

**✅ GHCR Success Criteria:**
- Images visible in GitHub Packages
- GitHub Actions workflow runs successfully
- Images pullable with `docker pull ghcr.io/YOUR_USERNAME/...`

---

## Phase 3: Azure Setup

### 3.1 Login to Azure

```bash
# Login to Azure
az login

# Set subscription (if you have multiple)
az account list --output table
az account set --subscription "Your Subscription Name"

# Verify
az account show
```

### 3.2 Create Resource Group

```bash
# Variables
RESOURCE_GROUP="rg-microvideo-prod"
LOCATION="eastus"  # Or: westeurope, southeastasia

# Create resource group
az group create \
  --name $RESOURCE_GROUP \
  --location $LOCATION

# Expected output:
# {
#   "id": "/subscriptions/.../resourceGroups/rg-microvideo-prod",
#   "location": "eastus",
#   "name": "rg-microvideo-prod",
#   "properties": {
#     "provisioningState": "Succeeded"
#   }
# }
```

### 3.3 Create Azure Container Registry (ACR)

```bash
# Variables
ACR_NAME="acrmicrovideo"  # Must be globally unique, lowercase, alphanumeric only

# Create ACR (Basic tier - free tier equivalent)
az acr create \
  --resource-group $RESOURCE_GROUP \
  --name $ACR_NAME \
  --sku Basic \
  --location $LOCATION

# Enable admin access (for App Service pull)
az acr update \
  --name $ACR_NAME \
  --admin-enabled true

# Get ACR credentials
az acr credential show --name $ACR_NAME

# Expected output:
# {
#   "passwords": [
#     {
#       "name": "password",
#       "value": "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"
#     },
#     {
#       "name": "password2",
#       "value": "YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY"
#     }
#   ],
#   "username": "acrmicrovideo"
# }

# Save credentials
ACR_USERNAME=$(az acr credential show --name $ACR_NAME --query username -o tsv)
ACR_PASSWORD=$(az acr credential show --name $ACR_NAME --query "passwords[0].value" -o tsv)
ACR_LOGIN_SERVER=$(az acr show --name $ACR_NAME --query loginServer -o tsv)

echo "ACR Login Server: $ACR_LOGIN_SERVER"
echo "ACR Username: $ACR_USERNAME"
echo "ACR Password: $ACR_PASSWORD"
```

### 3.4 Import Images from GHCR to ACR

```bash
# Login to ACR
echo $ACR_PASSWORD | docker login $ACR_LOGIN_SERVER -u $ACR_USERNAME --password-stdin

# Pull from GHCR
docker pull ghcr.io/YOUR_USERNAME/microvideo-content-api:latest

# Tag for ACR
docker tag ghcr.io/YOUR_USERNAME/microvideo-content-api:latest \
  $ACR_LOGIN_SERVER/content-api:latest

# Push to ACR
docker push $ACR_LOGIN_SERVER/content-api:latest

# Verify
az acr repository list --name $ACR_NAME --output table

# Expected:
# Result
# ------------
# content-api
```

**Alternative: Import directly with `az acr import`**

```bash
# Import from GHCR to ACR (requires public image or credentials)
az acr import \
  --name $ACR_NAME \
  --source ghcr.io/YOUR_USERNAME/microvideo-content-api:latest \
  --image content-api:latest \
  --username $GITHUB_USERNAME \
  --password $GITHUB_TOKEN
```

### 3.5 Create PostgreSQL Database (Azure Database for PostgreSQL)

```bash
# Variables
POSTGRES_SERVER="psql-microvideo"  # Must be globally unique
POSTGRES_ADMIN="adminuser"
POSTGRES_PASSWORD="P@ssw0rd1234!"  # Strong password required

# Create PostgreSQL server (Basic tier, 1 vCore, 5GB storage)
az postgres flexible-server create \
  --resource-group $RESOURCE_GROUP \
  --name $POSTGRES_SERVER \
  --location $LOCATION \
  --admin-user $POSTGRES_ADMIN \
  --admin-password "$POSTGRES_PASSWORD" \
  --sku-name Standard_B1ms \
  --tier Burstable \
  --storage-size 32 \
  --version 16 \
  --public-access 0.0.0.0-255.255.255.255

# Create database
az postgres flexible-server db create \
  --resource-group $RESOURCE_GROUP \
  --server-name $POSTGRES_SERVER \
  --database-name microvideo

# Get connection string
POSTGRES_CONNECTION_STRING="Host=$POSTGRES_SERVER.postgres.database.azure.com;Port=5432;Database=microvideo;Username=$POSTGRES_ADMIN;Password=$POSTGRES_PASSWORD;SSL Mode=Require;"

echo "PostgreSQL Connection String: $POSTGRES_CONNECTION_STRING"
```

**⚠️ FREE TIER WARNING:**
- PostgreSQL Flexible Server has NO free tier
- **Cost:** ~$12-30/month for Basic tier
- **Alternative:** Use free Azure SQL Database (250GB free) or PostgreSQL in Docker

### 3.6 Create Redis Cache (Azure Cache for Redis)

```bash
# Variables
REDIS_NAME="redis-microvideo"  # Must be globally unique

# Create Redis cache (Basic C0 - 250MB, ~$16/month)
az redis create \
  --resource-group $RESOURCE_GROUP \
  --name $REDIS_NAME \
  --location $LOCATION \
  --sku Basic \
  --vm-size c0

# Get Redis connection string
REDIS_KEY=$(az redis list-keys --resource-group $RESOURCE_GROUP --name $REDIS_NAME --query primaryKey -o tsv)
REDIS_HOST=$(az redis show --resource-group $RESOURCE_GROUP --name $REDIS_NAME --query hostName -o tsv)
REDIS_CONNECTION_STRING="$REDIS_HOST:6380,password=$REDIS_KEY,ssl=True,abortConnect=False"

echo "Redis Connection String: $REDIS_CONNECTION_STRING"
```

**⚠️ FREE TIER WARNING:**
- Azure Cache for Redis has NO free tier
- **Cost:** ~$16/month for Basic C0
- **Alternative:** Use Redis in Docker or free Redis Cloud (30MB free)

---

## Phase 4: Deploy to Production

### 4.1 Create App Service Plan

```bash
# Variables
APP_SERVICE_PLAN="asp-microvideo"

# Create App Service Plan (Free tier: F1)
az appservice plan create \
  --name $APP_SERVICE_PLAN \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --sku F1 \
  --is-linux

# For production, use Basic or Standard:
# --sku B1  # Basic: $13/month, 1.75GB RAM, 10GB storage
# --sku S1  # Standard: $69/month, 1.75GB RAM, 50GB storage, custom domains, SSL
```

**⚠️ FREE TIER LIMITATIONS (F1):**
- ⚠️ **1GB RAM** (insufficient for .NET app + PostgreSQL client)
- ⚠️ **1GB disk space**
- ⚠️ **60 CPU minutes/day**
- ⚠️ **No custom domains or SSL**
- ⚠️ **Shared infrastructure** (slow)
- ⚠️ **Apps sleep after 20 minutes of inactivity**

**Recommendation:** Use **Basic B1** ($13/month) for real production

### 4.2 Create Web App (Content.API)

```bash
# Variables
APP_NAME="app-microvideo-api"  # Must be globally unique

# Create web app with container
az webapp create \
  --resource-group $RESOURCE_GROUP \
  --plan $APP_SERVICE_PLAN \
  --name $APP_NAME \
  --deployment-container-image-name $ACR_LOGIN_SERVER/content-api:latest

# Configure ACR credentials for pulling images
az webapp config container set \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --docker-custom-image-name $ACR_LOGIN_SERVER/content-api:latest \
  --docker-registry-server-url https://$ACR_LOGIN_SERVER \
  --docker-registry-server-user $ACR_USERNAME \
  --docker-registry-server-password $ACR_PASSWORD

# Enable container logging
az webapp log config \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --docker-container-logging filesystem

# Get app URL
APP_URL=$(az webapp show --name $APP_NAME --resource-group $RESOURCE_GROUP --query defaultHostName -o tsv)
echo "App URL: https://$APP_URL"
```

### 4.3 Configure Environment Variables

```bash
# Set application settings (environment variables)
az webapp config appsettings set \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --settings \
    ASPNETCORE_ENVIRONMENT="Production" \
    ASPNETCORE_URLS="http://+:8080" \
    ConnectionStrings__PostgreSQL="$POSTGRES_CONNECTION_STRING" \
    ConnectionStrings__Redis="$REDIS_CONNECTION_STRING" \
    JWT__Secret="your-super-secret-jwt-key-minimum-32-characters-change-this" \
    JWT__Issuer="MicroVideoPlatform" \
    JWT__Audience="MicroVideoPlatform" \
    JWT__ExpiryMinutes="60" \
    RabbitMQ__Host="YOUR_RABBITMQ_HOST" \
    RabbitMQ__Port="5672" \
    RabbitMQ__Username="guest" \
    RabbitMQ__Password="guest" \
    Seq__Url="http://YOUR_SEQ_HOST:5341" \
    RateLimiting__RequestLimit="100" \
    RateLimiting__TimeWindowSeconds="60"

# Verify settings
az webapp config appsettings list \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --output table
```

**🔒 SECURITY BEST PRACTICE:**
Use **Azure Key Vault** for secrets:

```bash
# Create Key Vault
az keyvault create \
  --name kv-microvideo \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION

# Add secrets
az keyvault secret set --vault-name kv-microvideo --name jwt-secret --value "your-secret"
az keyvault secret set --vault-name kv-microvideo --name postgres-password --value "$POSTGRES_PASSWORD"

# Reference in App Service
az webapp config appsettings set \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --settings \
    JWT__Secret="@Microsoft.KeyVault(VaultName=kv-microvideo;SecretName=jwt-secret)"
```

### 4.4 Restart and Verify

```bash
# Restart app
az webapp restart --name $APP_NAME --resource-group $RESOURCE_GROUP

# Wait for startup (30-60 seconds)
sleep 60

# Check health
curl https://$APP_URL/health

# Expected output:
# {"status":"Healthy","results":{"postgres":"Healthy","redis":"Healthy"}}

# View logs
az webapp log tail --name $APP_NAME --resource-group $RESOURCE_GROUP
```

---

## Phase 5: Configuration & Environment Variables

### 5.1 Required Environment Variables

**Production `appsettings.Production.json`:**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Error"
    }
  },
  "AllowedHosts": "*.azurewebsites.net,yourdomain.com",
  "ConnectionStrings": {
    "PostgreSQL": "Host=psql-microvideo.postgres.database.azure.com;Port=5432;Database=microvideo;Username=adminuser;Password=***;SSL Mode=Require;",
    "Redis": "redis-microvideo.redis.cache.windows.net:6380,password=***,ssl=True,abortConnect=False"
  },
  "JWT": {
    "Secret": "*** FROM KEY VAULT ***",
    "Issuer": "MicroVideoPlatform",
    "Audience": "MicroVideoPlatform",
    "ExpiryMinutes": 15
  },
  "RateLimiting": {
    "RequestLimit": 1000,
    "TimeWindowSeconds": 60
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Warning",
      "Override": {
        "Microsoft": "Error",
        "System": "Error"
      }
    }
  }
}
```

### 5.2 Environment-Specific Settings

| Variable | Development | Production | Notes |
|----------|------------|------------|-------|
| `ASPNETCORE_ENVIRONMENT` | Development | Production | Enables prod optimizations |
| `ASPNETCORE_URLS` | http://+:8080 | http://+:8080 | Azure handles HTTPS termination |
| `JWT__ExpiryMinutes` | 60 | 15 | Shorter expiry in prod |
| `RateLimiting__RequestLimit` | 100 | 1000 | Higher limit in prod (load balancer) |
| `Serilog__MinimumLevel` | Information | Warning | Reduce log volume |
| `ConnectionStrings__*` | localhost | Azure services | Use Azure-managed services |

---

## Phase 6: Monitoring & Health Checks

### 6.1 Configure Application Insights

```bash
# Create Application Insights
az monitor app-insights component create \
  --app app-insights-microvideo \
  --location $LOCATION \
  --resource-group $RESOURCE_GROUP \
  --application-type web

# Get instrumentation key
APPINSIGHTS_KEY=$(az monitor app-insights component show \
  --app app-insights-microvideo \
  --resource-group $RESOURCE_GROUP \
  --query instrumentionKey -o tsv)

# Configure App Service to use Application Insights
az webapp config appsettings set \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --settings \
    APPLICATIONINSIGHTS_CONNECTION_STRING="InstrumentationKey=$APPINSIGHTS_KEY"
```

### 6.2 Setup Health Check Monitoring

```bash
# Configure health check path
az webapp config set \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --health-check-path "/health"

# App Service will automatically restart unhealthy instances
```

### 6.3 View Logs and Metrics

```bash
# Stream logs
az webapp log tail --name $APP_NAME --resource-group $RESOURCE_GROUP

# Download logs
az webapp log download --name $APP_NAME --resource-group $RESOURCE_GROUP

# View metrics in portal
az monitor metrics list \
  --resource "/subscriptions/.../resourceGroups/$RESOURCE_GROUP/providers/Microsoft.Web/sites/$APP_NAME" \
  --metric "CpuPercentage" \
  --output table
```

---

## Infrastructure as Code (Terraform)

### Create `infrastructure/main.tf`

See separate file: `infrastructure/main.tf` (created in next step)

**Features:**
- Resource Group
- App Service Plan (B1)
- Web App with container
- PostgreSQL Flexible Server
- Redis Cache
- Application Insights
- All environment variables configured

**Usage:**

```bash
cd infrastructure

# Initialize Terraform
terraform init

# Preview changes
terraform plan

# Apply infrastructure
terraform apply

# Destroy (cleanup)
terraform destroy
```

---

## Free Tier Limitations & Warnings

### ⚠️ Azure Free Tier Reality Check

| Service | Free Tier | Cost (Paid Tier) | Recommendation |
|---------|-----------|------------------|----------------|
| **App Service** | F1 (1GB RAM, 60 CPU min/day) | B1: $13/mo | **Use B1** (F1 too limited) |
| **PostgreSQL** | ❌ None | Flexible: $12-30/mo | Use Docker or free ElephantSQL |
| **Redis Cache** | ❌ None | Basic C0: $16/mo | Use Redis Cloud free tier (30MB) |
| **Container Registry** | Basic: 10GB storage | Basic: $5/mo | ✅ Use Basic (sufficient) |
| **Application Insights** | 5GB/month free | $2.30/GB | ✅ Use free tier |

**Total Monthly Cost (Minimum Production):**
- App Service (B1): **$13**
- PostgreSQL: **$12-30**
- Redis: **$16**
- ACR: **$5**
- **Total: $46-64/month**

**Free Alternatives:**
- PostgreSQL: ElephantSQL (20MB free), Supabase (500MB free)
- Redis: Redis Cloud (30MB free), Upstash (10K commands/day free)
- Total with free databases: **$13-18/month**

### ⚠️ F1 App Service Warnings

**DO NOT use F1 for production:**
1. **Apps sleep after 20 minutes** → Cold start on every request (5-10 seconds)
2. **60 CPU minutes/day** → App stops working after quota exceeded
3. **1GB RAM** → .NET app + EF Core + Redis client = crashes
4. **No custom domains/SSL** → Stuck with `*.azurewebsites.net`
5. **Shared infrastructure** → Slow, unpredictable performance

**Minimum for real production: B1 ($13/month)**

---

## Production Checklist

### 🔒 Security
- [ ] HTTPS enforced (`UseHttpsRedirection`)
- [ ] HSTS enabled (`UseHsts`)
- [ ] Security headers configured (CSP, X-Frame-Options)
- [ ] JWT secret in Key Vault (not hardcoded)
- [ ] Database password in Key Vault
- [ ] Rate limiting enabled
- [ ] Sensitive data masking in logs
- [ ] CORS restricted (not `AllowAnyOrigin`)
- [ ] SQL injection protection verified
- [ ] Container runs as non-root user

### 📊 Monitoring
- [ ] Application Insights configured
- [ ] Health check endpoint active (`/health`)
- [ ] Structured logging to Seq or Azure Log Analytics
- [ ] Alert rules for:
  - [ ] High CPU usage (>80%)
  - [ ] High memory usage (>80%)
  - [ ] Failed health checks
  - [ ] HTTP 5xx errors (>10/min)
  - [ ] Slow response times (>2s)

### 🚀 Performance
- [ ] Database indexes created
- [ ] Redis caching enabled
- [ ] Connection pooling configured
- [ ] Static files served from CDN (if applicable)
- [ ] Image optimization (gzip, brotli)

### 🔄 DevOps
- [ ] CI/CD pipeline configured (GitHub Actions)
- [ ] Automated tests passing
- [ ] Blue-green deployment strategy
- [ ] Rollback plan documented
- [ ] Database migration strategy
- [ ] Backup and restore tested

### 📝 Documentation
- [ ] API documentation (Swagger) available
- [ ] Environment variables documented
- [ ] Deployment runbook created
- [ ] Incident response plan documented

---

## Troubleshooting

### Issue: Container fails to start

```bash
# View container logs
az webapp log tail --name $APP_NAME --resource-group $RESOURCE_GROUP

# Common causes:
# 1. Missing environment variables
# 2. Database connection failure
# 3. Port mismatch (app must listen on port 8080)
# 4. Insufficient memory (F1 tier)
```

**Fix:**
```bash
# Check environment variables
az webapp config appsettings list --name $APP_NAME --resource-group $RESOURCE_GROUP

# Verify port binding in Dockerfile
# EXPOSE 8080
# ENV ASPNETCORE_URLS=http://+:8080
```

### Issue: Database migration fails

```bash
# Run migration manually
az webapp ssh --name $APP_NAME --resource-group $RESOURCE_GROUP

# Inside container:
dotnet ef database update --project /app/MicroVideoPlatform.Content.API.dll
```

### Issue: High memory usage / crashes

```bash
# Check memory metrics
az monitor metrics list \
  --resource "/subscriptions/.../resourceGroups/$RESOURCE_GROUP/providers/Microsoft.Web/sites/$APP_NAME" \
  --metric "MemoryPercentage"

# Fix: Upgrade to B1 or higher
az appservice plan update \
  --name $APP_SERVICE_PLAN \
  --resource-group $RESOURCE_GROUP \
  --sku B1
```

### Issue: App sleeps after 20 minutes (F1)

**This is expected behavior for F1 tier.**

**Fix:** Upgrade to B1 or use "Always On" setting:
```bash
# Enable "Always On" (requires Basic or higher tier)
az webapp config set \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --always-on true
```

### Issue: Cannot pull image from ACR

```bash
# Verify ACR credentials
az acr credential show --name $ACR_NAME

# Reconfigure web app with correct credentials
az webapp config container set \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --docker-registry-server-url https://$ACR_LOGIN_SERVER \
  --docker-registry-server-user $ACR_USERNAME \
  --docker-registry-server-password $ACR_PASSWORD
```

---

## Custom Domain & SSL (B1+ tier)

### Add Custom Domain

```bash
# Requirements:
# - App Service Plan: Basic (B1) or higher
# - Domain ownership verified

# Add custom domain
az webapp config hostname add \
  --webapp-name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --hostname www.yourdomain.com

# Bind SSL certificate (managed certificate - free)
az webapp config ssl create \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --hostname www.yourdomain.com

# Force HTTPS
az webapp update \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --https-only true
```

**DNS Configuration:**
```
Type: CNAME
Name: www
Value: app-microvideo-api.azurewebsites.net
TTL: 3600
```

---

## Conclusion

**🎉 Congratulations!** Your Micro-Video Platform is now deployed to Azure.

**What You've Accomplished:**
✅ Built production-ready Docker images
✅ Published images to GHCR and ACR
✅ Deployed to Azure App Service
✅ Configured PostgreSQL and Redis
✅ Setup monitoring with Application Insights
✅ Implemented security best practices
✅ Created Infrastructure as Code with Terraform

**Portfolio Value:**
This deployment demonstrates:
- **Cloud expertise** (Azure)
- **DevOps skills** (CI/CD, IaC)
- **Production mindset** (monitoring, security, cost optimization)
- **Real-world experience** (not just localhost)

**Interview Talking Points:**
- "I deployed a microservices platform to Azure using App Service and ACR"
- "I automated infrastructure with Terraform"
- "I implemented monitoring with Application Insights and health checks"
- "I optimized costs by using free tiers where possible"

---

**Next Steps:**
1. Scale to multiple regions (Azure Traffic Manager)
2. Implement CI/CD with automated tests
3. Add Kubernetes deployment (AKS)
4. Implement feature flags and A/B testing

**🚀 Your code is no longer just a sample - it's a production service!**

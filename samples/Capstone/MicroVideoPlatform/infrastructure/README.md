# Terraform Infrastructure - Micro-Video Platform

This directory contains Infrastructure as Code (IaC) using Terraform to deploy the Micro-Video Platform to Azure.

## Prerequisites

- **Terraform** 1.5+ installed ([Download](https://www.terraform.io/downloads))
- **Azure CLI** installed and authenticated (`az login`)
- **Azure Subscription** with Contributor access

## Quick Start

### 1. Setup Configuration

```bash
# Copy example variables
cp terraform.tfvars.example terraform.tfvars

# Edit terraform.tfvars with your values
nano terraform.tfvars

# IMPORTANT: Change these values:
# - postgres_admin_password
# - jwt_secret
```

### 2. Initialize Terraform

```bash
# Download provider plugins
terraform init
```

### 3. Preview Changes

```bash
# See what will be created
terraform plan
```

### 4. Deploy Infrastructure

```bash
# Apply changes (will prompt for confirmation)
terraform apply

# Or auto-approve (use with caution)
terraform apply -auto-approve
```

### 5. Get Outputs

```bash
# View all outputs
terraform output

# View specific output
terraform output app_service_url

# Get sensitive outputs
terraform output -raw acr_admin_password
terraform output -raw postgres_connection_string
```

### 6. Destroy Infrastructure

```bash
# Delete all resources (will prompt for confirmation)
terraform destroy

# WARNING: This will delete everything, including databases
```

## Resources Created

| Resource | Purpose | SKU | Cost/Month |
|----------|---------|-----|------------|
| **Resource Group** | Container for all resources | N/A | Free |
| **App Service Plan** | Compute for web apps | Basic B1 | $13 |
| **App Service (Linux)** | Content.API hosting | Container | Included in plan |
| **Container Registry** | Docker image storage | Basic | $5 |
| **PostgreSQL Server** | Database | Burstable B1ms | $12-30 |
| **Redis Cache** | Distributed caching | Basic C0 | $16 |
| **Application Insights** | Monitoring & telemetry | Pay-as-you-go | $0-10 (5GB free) |
| **Log Analytics** | Log storage | PerGB2018 | Included |

**Total Estimated Cost:** $46-64/month

## Configuration Options

### App Service SKU Options

```hcl
# terraform.tfvars
app_service_sku = "B1"   # Basic: $13/month, 1.75GB RAM, Always On
# app_service_sku = "S1"  # Standard: $69/month, 1.75GB RAM, Custom domains, SSL
# app_service_sku = "F1"  # Free: $0, 1GB RAM, 60 CPU min/day, sleeps after 20 min
```

### PostgreSQL SKU Options

```hcl
# terraform.tfvars
postgres_sku = "B_Standard_B1ms"   # Burstable: $12/month, 1 vCore
# postgres_sku = "GP_Standard_D2s_v3"  # General Purpose: $150/month, 2 vCores
```

### Redis SKU Options

```hcl
# terraform.tfvars
redis_sku = "Basic"
redis_capacity = 0  # 250MB: $16/month
# redis_capacity = 1  # 1GB: $50/month
```

## Environment Variables

The Terraform configuration automatically sets these App Service environment variables:

```bash
# ASP.NET Core
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080

# Database Connection
ConnectionStrings__PostgreSQL=Host=...;Database=...;

# Redis Connection
ConnectionStrings__Redis=...

# JWT Configuration
JWT__Secret=***
JWT__Issuer=MicroVideoPlatform
JWT__ExpiryMinutes=15

# Rate Limiting
RateLimiting__RequestLimit=1000
RateLimiting__TimeWindowSeconds=60

# Application Insights
APPLICATIONINSIGHTS_CONNECTION_STRING=...

# Docker Configuration
DOCKER_REGISTRY_SERVER_URL=https://acrmicrovideo.azurecr.io
DOCKER_REGISTRY_SERVER_USERNAME=***
DOCKER_REGISTRY_SERVER_PASSWORD=***
```

## State Management

### Local State (Default)

By default, Terraform stores state locally in `terraform.tfstate`.

**⚠️ WARNING:** Local state is NOT recommended for production or team collaboration.

### Remote State (Recommended)

For production, use Azure Storage backend:

```hcl
# Uncomment in main.tf:
terraform {
  backend "azurerm" {
    resource_group_name  = "rg-terraform-state"
    storage_account_name = "tfstatemicrovideo"
    container_name       = "tfstate"
    key                  = "microvideo.tfstate"
  }
}
```

**Setup Remote State:**

```bash
# Create resource group for state
az group create --name rg-terraform-state --location eastus

# Create storage account
az storage account create \
  --name tfstatemicrovideo \
  --resource-group rg-terraform-state \
  --location eastus \
  --sku Standard_LRS

# Create container
az storage container create \
  --name tfstate \
  --account-name tfstatemicrovideo

# Initialize with backend
terraform init -migrate-state
```

## Security Best Practices

### 1. Protect Sensitive Files

```bash
# Add to .gitignore
echo "terraform.tfvars" >> .gitignore
echo "*.tfstate" >> .gitignore
echo "*.tfstate.backup" >> .gitignore
echo ".terraform/" >> .gitignore
```

### 2. Use Azure Key Vault (Production)

Instead of terraform.tfvars, use Key Vault for secrets:

```bash
# Create Key Vault
az keyvault create \
  --name kv-microvideo \
  --resource-group rg-microvideo-prod \
  --location eastus

# Add secrets
az keyvault secret set --vault-name kv-microvideo --name jwt-secret --value "..."
az keyvault secret set --vault-name kv-microvideo --name postgres-password --value "..."

# Reference in Terraform
data "azurerm_key_vault_secret" "jwt_secret" {
  name         = "jwt-secret"
  key_vault_id = azurerm_key_vault.main.id
}
```

### 3. Use Environment Variables

```bash
# Set sensitive variables via environment
export TF_VAR_jwt_secret="your-secret-key"
export TF_VAR_postgres_admin_password="P@ssw0rd1234!"

# Run terraform without terraform.tfvars
terraform apply
```

## Deployment Workflow

### Initial Deployment

```bash
# 1. Login to Azure
az login

# 2. Set subscription
az account set --subscription "Your Subscription"

# 3. Configure variables
cp terraform.tfvars.example terraform.tfvars
nano terraform.tfvars

# 4. Initialize Terraform
terraform init

# 5. Preview changes
terraform plan

# 6. Apply infrastructure
terraform apply

# 7. Get App Service URL
terraform output app_service_url

# 8. Push Docker image to ACR
ACR_LOGIN_SERVER=$(terraform output -raw acr_login_server)
ACR_USERNAME=$(terraform output -raw acr_admin_username)
ACR_PASSWORD=$(terraform output -raw acr_admin_password)

echo $ACR_PASSWORD | docker login $ACR_LOGIN_SERVER -u $ACR_USERNAME --password-stdin
docker tag your-image:latest $ACR_LOGIN_SERVER/content-api:latest
docker push $ACR_LOGIN_SERVER/content-api:latest

# 9. Restart App Service
az webapp restart --name $(terraform output -raw app_service_name) --resource-group $(terraform output -raw resource_group_name)

# 10. Verify deployment
curl $(terraform output -raw app_service_url)/health
```

### Update Deployment

```bash
# 1. Modify main.tf or terraform.tfvars
nano main.tf

# 2. Preview changes
terraform plan

# 3. Apply changes
terraform apply

# 4. Verify
terraform output
```

## Troubleshooting

### Issue: "Resource name already exists"

**Solution:** Change `project_name` or wait for random_string to regenerate.

### Issue: "Insufficient permissions"

**Solution:** Ensure your Azure account has Contributor role:

```bash
az role assignment create \
  --assignee YOUR_EMAIL \
  --role Contributor \
  --scope /subscriptions/YOUR_SUBSCRIPTION_ID
```

### Issue: "Backend initialization required"

**Solution:** Run `terraform init` again.

### Issue: State lock error

**Solution:** Release state lock:

```bash
# For Azure backend
az storage blob lease break \
  --container-name tfstate \
  --blob-name microvideo.tfstate \
  --account-name tfstatemicrovideo
```

## Cost Optimization

### Free Tier Equivalent

```hcl
# terraform.tfvars
app_service_sku = "F1"   # Free (limited)
# Replace PostgreSQL with free ElephantSQL (20MB)
# Replace Redis with free Redis Cloud (30MB)
```

**Cost: ~$5/month (ACR only)**

### Production Minimum

```hcl
# terraform.tfvars
app_service_sku = "B1"
postgres_sku = "B_Standard_B1ms"
redis_sku = "Basic"
redis_capacity = 0
```

**Cost: ~$46-64/month**

### High Availability

```hcl
# terraform.tfvars
app_service_sku = "S1"
postgres_sku = "GP_Standard_D2s_v3"
redis_sku = "Standard"
redis_capacity = 1
```

**Cost: ~$250+/month**

## CI/CD Integration

### GitHub Actions

```yaml
# .github/workflows/terraform.yml
name: Terraform Deploy

on:
  push:
    branches: [main]
    paths: ['infrastructure/**']

jobs:
  terraform:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Setup Terraform
        uses: hashicorp/setup-terraform@v3

      - name: Azure Login
        uses: azure/login@v1
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}

      - name: Terraform Init
        run: terraform init
        working-directory: infrastructure

      - name: Terraform Plan
        run: terraform plan
        working-directory: infrastructure
        env:
          TF_VAR_jwt_secret: ${{ secrets.JWT_SECRET }}
          TF_VAR_postgres_admin_password: ${{ secrets.POSTGRES_PASSWORD }}

      - name: Terraform Apply
        if: github.ref == 'refs/heads/main'
        run: terraform apply -auto-approve
        working-directory: infrastructure
```

## References

- [Terraform Azure Provider Documentation](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs)
- [Azure App Service Pricing](https://azure.microsoft.com/en-us/pricing/details/app-service/)
- [Azure PostgreSQL Pricing](https://azure.microsoft.com/en-us/pricing/details/postgresql/)
- [Azure Redis Pricing](https://azure.microsoft.com/en-us/pricing/details/cache/)

## Support

For issues or questions:
1. Check [Troubleshooting](#troubleshooting) section
2. Review [DEPLOY.md](../DEPLOY.md) for deployment guide
3. Open GitHub issue with terraform output and error logs

# =============================================================================
# Micro-Video Platform - Azure Infrastructure as Code (Terraform)
# =============================================================================
# This Terraform configuration creates a complete Azure infrastructure for
# the Micro-Video Platform microservices application.
#
# Resources Created:
# - Resource Group
# - App Service Plan (Basic B1)
# - App Service (Content.API)
# - Azure Container Registry (ACR)
# - PostgreSQL Flexible Server
# - Azure Cache for Redis
# - Application Insights
# - Log Analytics Workspace
#
# Cost Estimate: $46-64/month (Basic tier)
# =============================================================================

terraform {
  required_version = ">= 1.5"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.80"
    }
    random = {
      source  = "hashicorp/random"
      version = "~> 3.5"
    }
  }

  # Uncomment to use Azure Storage for state management
  # backend "azurerm" {
  #   resource_group_name  = "rg-terraform-state"
  #   storage_account_name = "tfstatemicrovideo"
  #   container_name       = "tfstate"
  #   key                  = "microvideo.tfstate"
  # }
}

provider "azurerm" {
  features {
    resource_group {
      prevent_deletion_if_contains_resources = false
    }

    key_vault {
      purge_soft_delete_on_destroy    = true
      recover_soft_deleted_key_vaults = true
    }
  }
}

# =============================================================================
# Variables
# =============================================================================

variable "project_name" {
  description = "Project name used for resource naming"
  type        = string
  default     = "microvideo"
}

variable "environment" {
  description = "Environment name (dev, staging, prod)"
  type        = string
  default     = "prod"
}

variable "location" {
  description = "Azure region for resources"
  type        = string
  default     = "eastus"
}

variable "app_service_sku" {
  description = "App Service Plan SKU (F1, B1, S1, P1V2)"
  type        = string
  default     = "B1"  # Basic: $13/month
}

variable "postgres_sku" {
  description = "PostgreSQL SKU name"
  type        = string
  default     = "B_Standard_B1ms"  # Burstable: $12/month
}

variable "redis_sku" {
  description = "Redis cache SKU (Basic, Standard, Premium)"
  type        = string
  default     = "Basic"
}

variable "redis_capacity" {
  description = "Redis cache capacity (0=250MB, 1=1GB, 2=2.5GB)"
  type        = number
  default     = 0  # 250MB: ~$16/month
}

variable "acr_image_tag" {
  description = "Docker image tag to deploy"
  type        = string
  default     = "latest"
}

variable "jwt_secret" {
  description = "JWT signing secret (minimum 32 characters)"
  type        = string
  sensitive   = true
}

variable "postgres_admin_username" {
  description = "PostgreSQL administrator username"
  type        = string
  default     = "adminuser"
}

variable "postgres_admin_password" {
  description = "PostgreSQL administrator password"
  type        = string
  sensitive   = true
}

variable "enable_https_only" {
  description = "Force HTTPS for App Service"
  type        = bool
  default     = true
}

variable "enable_always_on" {
  description = "Enable Always On (requires Basic or higher)"
  type        = bool
  default     = true
}

# =============================================================================
# Local Variables
# =============================================================================

locals {
  resource_prefix = "${var.project_name}-${var.environment}"

  common_tags = {
    Project     = var.project_name
    Environment = var.environment
    ManagedBy   = "Terraform"
    CreatedDate = timestamp()
  }
}

# =============================================================================
# Random Suffix (for globally unique names)
# =============================================================================

resource "random_string" "suffix" {
  length  = 6
  special = false
  upper   = false
}

# =============================================================================
# Resource Group
# =============================================================================

resource "azurerm_resource_group" "main" {
  name     = "rg-${local.resource_prefix}"
  location = var.location
  tags     = local.common_tags
}

# =============================================================================
# Azure Container Registry (ACR)
# =============================================================================

resource "azurerm_container_registry" "acr" {
  name                = "acr${var.project_name}${random_string.suffix.result}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  sku                 = "Basic"  # $5/month, 10GB storage
  admin_enabled       = true      # Enable for App Service pull

  tags = local.common_tags
}

# =============================================================================
# Log Analytics Workspace (for Application Insights)
# =============================================================================

resource "azurerm_log_analytics_workspace" "main" {
  name                = "log-${local.resource_prefix}-${random_string.suffix.result}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  sku                 = "PerGB2018"
  retention_in_days   = 30  # Free tier: 31 days retention

  tags = local.common_tags
}

# =============================================================================
# Application Insights
# =============================================================================

resource "azurerm_application_insights" "main" {
  name                = "appi-${local.resource_prefix}-${random_string.suffix.result}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  workspace_id        = azurerm_log_analytics_workspace.main.id
  application_type    = "web"

  tags = local.common_tags
}

# =============================================================================
# PostgreSQL Flexible Server
# =============================================================================

resource "azurerm_postgresql_flexible_server" "main" {
  name                   = "psql-${local.resource_prefix}-${random_string.suffix.result}"
  resource_group_name    = azurerm_resource_group.main.name
  location               = azurerm_resource_group.main.location
  version                = "16"
  administrator_login    = var.postgres_admin_username
  administrator_password = var.postgres_admin_password

  sku_name   = var.postgres_sku
  storage_mb = 32768  # 32GB

  backup_retention_days        = 7
  geo_redundant_backup_enabled = false

  # Allow Azure services access
  public_network_access_enabled = true

  tags = local.common_tags
}

# PostgreSQL Firewall Rule - Allow Azure Services
resource "azurerm_postgresql_flexible_server_firewall_rule" "azure_services" {
  name             = "AllowAzureServices"
  server_id        = azurerm_postgresql_flexible_server.main.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}

# PostgreSQL Firewall Rule - Allow All IPs (ONLY FOR DEVELOPMENT)
# Comment out for production
resource "azurerm_postgresql_flexible_server_firewall_rule" "all" {
  name             = "AllowAll"
  server_id        = azurerm_postgresql_flexible_server.main.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "255.255.255.255"
}

# PostgreSQL Database
resource "azurerm_postgresql_flexible_server_database" "main" {
  name      = var.project_name
  server_id = azurerm_postgresql_flexible_server.main.id
  charset   = "UTF8"
  collation = "en_US.utf8"
}

# =============================================================================
# Azure Cache for Redis
# =============================================================================

resource "azurerm_redis_cache" "main" {
  name                = "redis-${local.resource_prefix}-${random_string.suffix.result}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  capacity            = var.redis_capacity
  family              = "C"
  sku_name            = var.redis_sku

  enable_non_ssl_port = false
  minimum_tls_version = "1.2"

  redis_configuration {
    enable_authentication = true
  }

  tags = local.common_tags
}

# =============================================================================
# App Service Plan
# =============================================================================

resource "azurerm_service_plan" "main" {
  name                = "asp-${local.resource_prefix}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  os_type             = "Linux"
  sku_name            = var.app_service_sku

  tags = local.common_tags
}

# =============================================================================
# App Service - Content.API
# =============================================================================

resource "azurerm_linux_web_app" "content_api" {
  name                = "app-${local.resource_prefix}-api-${random_string.suffix.result}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  service_plan_id     = azurerm_service_plan.main.id
  https_only          = var.enable_https_only

  site_config {
    always_on                         = var.enable_always_on
    health_check_path                 = "/health"
    health_check_eviction_time_in_min = 2

    application_stack {
      docker_image_name   = "content-api:${var.acr_image_tag}"
      docker_registry_url = "https://${azurerm_container_registry.acr.login_server}"
    }
  }

  app_settings = {
    # ASP.NET Core
    ASPNETCORE_ENVIRONMENT = var.environment == "prod" ? "Production" : "Development"
    ASPNETCORE_URLS        = "http://+:8080"

    # Database
    ConnectionStrings__PostgreSQL = "Host=${azurerm_postgresql_flexible_server.main.fqdn};Port=5432;Database=${azurerm_postgresql_flexible_server_database.main.name};Username=${var.postgres_admin_username};Password=${var.postgres_admin_password};SSL Mode=Require;Trust Server Certificate=true;"

    # Redis
    ConnectionStrings__Redis = "${azurerm_redis_cache.main.hostname}:${azurerm_redis_cache.main.ssl_port},password=${azurerm_redis_cache.main.primary_access_key},ssl=True,abortConnect=False"

    # JWT
    JWT__Secret        = var.jwt_secret
    JWT__Issuer        = "MicroVideoPlatform"
    JWT__Audience      = "MicroVideoPlatform"
    JWT__ExpiryMinutes = var.environment == "prod" ? "15" : "60"

    # Rate Limiting
    RateLimiting__RequestLimit       = var.environment == "prod" ? "1000" : "100"
    RateLimiting__TimeWindowSeconds  = "60"

    # Application Insights
    APPLICATIONINSIGHTS_CONNECTION_STRING = azurerm_application_insights.main.connection_string
    ApplicationInsightsAgent_EXTENSION_VERSION = "~3"

    # Docker
    WEBSITES_ENABLE_APP_SERVICE_STORAGE = "false"
    WEBSITES_PORT                       = "8080"
    DOCKER_REGISTRY_SERVER_URL          = "https://${azurerm_container_registry.acr.login_server}"
    DOCKER_REGISTRY_SERVER_USERNAME     = azurerm_container_registry.acr.admin_username
    DOCKER_REGISTRY_SERVER_PASSWORD     = azurerm_container_registry.acr.admin_password
    DOCKER_ENABLE_CI                    = "true"
  }

  logs {
    application_logs {
      file_system_level = "Information"
    }

    http_logs {
      file_system {
        retention_in_days = 7
        retention_in_mb   = 35
      }
    }
  }

  tags = local.common_tags
}

# =============================================================================
# Outputs
# =============================================================================

output "resource_group_name" {
  description = "Name of the resource group"
  value       = azurerm_resource_group.main.name
}

output "app_service_url" {
  description = "URL of the Content.API App Service"
  value       = "https://${azurerm_linux_web_app.content_api.default_hostname}"
}

output "app_service_name" {
  description = "Name of the Content.API App Service"
  value       = azurerm_linux_web_app.content_api.name
}

output "acr_login_server" {
  description = "Login server for Azure Container Registry"
  value       = azurerm_container_registry.acr.login_server
}

output "acr_admin_username" {
  description = "Admin username for ACR"
  value       = azurerm_container_registry.acr.admin_username
  sensitive   = true
}

output "acr_admin_password" {
  description = "Admin password for ACR"
  value       = azurerm_container_registry.acr.admin_password
  sensitive   = true
}

output "postgres_fqdn" {
  description = "Fully qualified domain name of PostgreSQL server"
  value       = azurerm_postgresql_flexible_server.main.fqdn
}

output "postgres_connection_string" {
  description = "PostgreSQL connection string"
  value       = "Host=${azurerm_postgresql_flexible_server.main.fqdn};Port=5432;Database=${azurerm_postgresql_flexible_server_database.main.name};Username=${var.postgres_admin_username};Password=${var.postgres_admin_password};SSL Mode=Require;"
  sensitive   = true
}

output "redis_hostname" {
  description = "Redis cache hostname"
  value       = azurerm_redis_cache.main.hostname
}

output "redis_primary_key" {
  description = "Redis primary access key"
  value       = azurerm_redis_cache.main.primary_access_key
  sensitive   = true
}

output "redis_connection_string" {
  description = "Redis connection string"
  value       = "${azurerm_redis_cache.main.hostname}:${azurerm_redis_cache.main.ssl_port},password=${azurerm_redis_cache.main.primary_access_key},ssl=True,abortConnect=False"
  sensitive   = true
}

output "application_insights_instrumentation_key" {
  description = "Application Insights instrumentation key"
  value       = azurerm_application_insights.main.instrumentation_key
  sensitive   = true
}

output "application_insights_connection_string" {
  description = "Application Insights connection string"
  value       = azurerm_application_insights.main.connection_string
  sensitive   = true
}

# =============================================================================
# Cost Estimate Summary
# =============================================================================

output "estimated_monthly_cost" {
  description = "Estimated monthly cost (USD)"
  value = {
    app_service_plan = var.app_service_sku == "B1" ? "$13" : var.app_service_sku == "S1" ? "$69" : "$0 (F1)"
    postgresql       = "$12-30"
    redis            = var.redis_capacity == 0 ? "$16" : "$50+"
    acr              = "$5"
    app_insights     = "$0-10 (first 5GB free)"
    total            = "$46-64/month (Basic tier)"
  }
}

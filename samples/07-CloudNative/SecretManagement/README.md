# Secret Management

> Secure secret handling with User Secrets (dev) and Azure Key Vault (production).

## Features
- **User Secrets** - Local development secrets (not in git)
- **Azure Key Vault** - Production secret storage
- **Managed Identity** - Secure authentication without keys
- **Secret Masking** - Never expose secrets in logs/responses

## Setup

### Development (User Secrets)

```bash
cd samples/07-CloudNative/SecretManagement

# Initialize User Secrets
dotnet user-secrets init

# Set secrets (stored locally, NOT in git)
dotnet user-secrets set "ApiKey" "dev-api-key-123"
dotnet user-secrets set "DatabasePassword" "dev-password-456"
dotnet user-secrets set "ExternalApiKey" "dev-external-789"

# List secrets
dotnet user-secrets list

# Run app
dotnet run
```

### Production (Azure Key Vault)

```bash
# 1. Create Azure Key Vault
az keyvault create \
  --name myapp-keyvault \
  --resource-group myapp-rg \
  --location eastus

# 2. Add secrets to Key Vault
az keyvault secret set \
  --vault-name myapp-keyvault \
  --name ApiKey \
  --value "prod-api-key-abc123xyz"

az keyvault secret set \
  --vault-name myapp-keyvault \
  --name DatabasePassword \
  --value "SecureP@ssw0rd!"

# 3. Enable Managed Identity for your app
az webapp identity assign \
  --name myapp \
  --resource-group myapp-rg

# 4. Grant access to Key Vault
az keyvault set-policy \
  --name myapp-keyvault \
  --object-id <managed-identity-object-id> \
  --secret-permissions get list

# 5. Configure Key Vault URL in app settings
az webapp config appsettings set \
  --name myapp \
  --settings KeyVaultUrl="https://myapp-keyvault.vault.azure.net/"
```

## Test Endpoints

```bash
# View all secrets (masked)
curl http://localhost:5000/secrets

# Get specific secret (masked)
curl http://localhost:5000/secrets/ApiKey
```

## Secret Storage by Environment

| Environment | Storage | Access Method |
|-------------|---------|---------------|
| **Development** | User Secrets (`~/.microsoft/usersecrets/`) | Local file system |
| **Staging** | Azure Key Vault | Managed Identity |
| **Production** | Azure Key Vault | Managed Identity |

## How User Secrets Work

### File Location (NOT in project directory)
```
Windows:   %APPDATA%\Microsoft\UserSecrets\<UserSecretsId>\secrets.json
macOS:     ~/.microsoft/usersecrets/<UserSecretsId>/secrets.json
Linux:     ~/.microsoft/usersecrets/<UserSecretsId>/secrets.json
```

### secrets.json Example
```json
{
  "ApiKey": "dev-api-key-123",
  "DatabasePassword": "dev-password-456",
  "ConnectionStrings:DefaultConnection": "Server=localhost;Database=DevDb;..."
}
```

### In Code
```csharp
// Automatically loaded in Development
var apiKey = builder.Configuration["ApiKey"];
```

## How Azure Key Vault Works

### 1. Store Secrets in Key Vault
```bash
az keyvault secret set --vault-name myapp-keyvault --name ApiKey --value "secret123"
```

### 2. Configure in Code
```csharp
if (builder.Environment.IsProduction())
{
    var keyVaultUrl = builder.Configuration["KeyVaultUrl"];
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultUrl),
        new DefaultAzureCredential());  // Uses Managed Identity
}
```

### 3. Access Like Normal Config
```csharp
var apiKey = builder.Configuration["ApiKey"];  // Fetched from Key Vault
```

## Managed Identity

### What is Managed Identity?
- Azure automatically manages credentials for your app
- No need to store connection strings or keys
- App authenticates to Azure services without passwords

### Enable for Azure App Service
```bash
az webapp identity assign --name myapp --resource-group myapp-rg
```

### Enable for Azure Functions
```bash
az functionapp identity assign --name myfunc --resource-group myapp-rg
```

### Enable for AKS (Kubernetes)
```yaml
apiVersion: v1
kind: Pod
metadata:
  name: myapp
  labels:
    aadpodidbinding: myapp-identity
```

## Security Best Practices

### ✅ DO
- Use User Secrets for local development
- Use Azure Key Vault for staging/production
- Use Managed Identity (no passwords in code)
- Mask secrets in logs and responses
- Rotate secrets regularly
- Use separate Key Vaults per environment

### ❌ DON'T
- Commit secrets to git (appsettings.json, .env files)
- Log secrets (even in development)
- Expose secrets in API responses
- Hardcode secrets in code
- Share production secrets via email/Slack
- Store secrets in environment variables (production)

## Secret Rotation

### Azure Key Vault Versioning
```bash
# Create new version of secret
az keyvault secret set \
  --vault-name myapp-keyvault \
  --name ApiKey \
  --value "new-secret-value"

# App automatically uses latest version
# No code changes needed!
```

### Monitoring Secret Access
```bash
# Enable diagnostic logs
az monitor diagnostic-settings create \
  --resource "/subscriptions/.../vaults/myapp-keyvault" \
  --logs '[{"category": "AuditEvent", "enabled": true}]'
```

## Common Mistakes

### ❌ Exposing Secrets in Logs
```csharp
// BAD - logs the secret!
logger.LogInformation($"API Key: {apiKey}");

// GOOD - mask the secret
logger.LogInformation($"API Key: {MaskSecret(apiKey)}");
```

### ❌ Committing secrets.json
```bash
# BAD - secrets in git
git add appsettings.json  # Contains real secrets

# GOOD - use User Secrets or Key Vault
dotnet user-secrets set "ApiKey" "secret123"
```

### ❌ Returning Secrets in API
```csharp
// BAD - exposes secret!
app.MapGet("/config", (IConfiguration config) => config["ApiKey"]);

// GOOD - mask the secret
app.MapGet("/config", (IConfiguration config) => MaskSecret(config["ApiKey"]));
```

## Environment-Specific Configuration

```csharp
var builder = WebApplication.CreateBuilder(args);

// Development: User Secrets (local)
if (builder.Environment.IsDevelopment())
{
    // Automatically loaded
}

// Staging: Azure Key Vault
if (builder.Environment.IsStaging())
{
    builder.Configuration.AddAzureKeyVault(
        new Uri("https://myapp-staging-kv.vault.azure.net/"),
        new DefaultAzureCredential());
}

// Production: Azure Key Vault
if (builder.Environment.IsProduction())
{
    builder.Configuration.AddAzureKeyVault(
        new Uri("https://myapp-prod-kv.vault.azure.net/"),
        new DefaultAzureCredential());
}
```

## Kubernetes Secrets (Alternative)

```bash
# Create secret
kubectl create secret generic app-secrets \
  --from-literal=api-key="secret123" \
  --from-literal=db-password="password456"

# Use in deployment
env:
- name: ApiKey
  valueFrom:
    secretKeyRef:
      name: app-secrets
      key: api-key
```

## Integration with CI/CD

### GitHub Actions
```yaml
- name: Deploy to Azure
  env:
    AZURE_CREDENTIALS: ${{ secrets.AZURE_CREDENTIALS }}
  run: |
    az login --service-principal ...
    az webapp deploy ...
```

### Azure DevOps
```yaml
- task: AzureKeyVault@2
  inputs:
    azureSubscription: 'My Azure Subscription'
    KeyVaultName: 'myapp-keyvault'
    SecretsFilter: '*'
```

**Use Cases:** Production applications, cloud-native apps, microservices, containerized apps, CI/CD pipelines.
